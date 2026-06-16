using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Enums;
using PropertyInspection.Core.Interfaces.Services;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IInspectionService _inspectionService;
        private readonly IS3Service _s3Service;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEmailService _emailService;

        public ReportController(
            IReportService reportService,
            IInspectionService inspectionService,
            IS3Service s3Service,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IEmailService emailService)
        {
            _reportService = reportService;
            _inspectionService = inspectionService;
            _s3Service = s3Service;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _emailService = emailService;
        }
 
        [HttpGet("inspection/{inspectionId}")]
        public async Task<ActionResult<ApiResponse<ReportDto>>> GetReportByInspectionId(Guid inspectionId, Guid? agencyId)
        {
            var result = await _reportService.GetReportByInspectionIdAsync(inspectionId , agencyId);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Generates a PDF for a closed inspection report, stores it in S3, and returns the S3 URL.
        /// Uses smart caching: if a PDF already exists for this inspection, returns the existing URL.
        /// </summary>
        [HttpGet("inspection/{inspectionId}/pdf")]
        public async Task<ActionResult> GetReportPdf(Guid inspectionId, Guid? agencyId)
        {
            // 1. Fetch the inspection to check status and existing PdfUrl
            var inspectionResult = await _inspectionService.GetByIdAsync(inspectionId, agencyId);
            if (!inspectionResult.Success || inspectionResult.Data == null)
            {
                return NotFound(new { message = "Inspection not found." });
            }

            var inspection = inspectionResult.Data;

            // 2. Only allow PDF download for Closed inspections
            if (inspection.InspectionStatus != InspectionStatus.Closed)
            {
                return BadRequest(new { message = "PDF can only be generated for closed inspection reports." });
            }

            // 3. Generate PDF via the frontend Puppeteer endpoint
            var frontendBaseUrl =
                _configuration["Frontend:BaseUrl"] ??
                _configuration["FrontendBaseUrl"] ??
                Request.Headers["Origin"].ToString();

            if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            {
                return BadRequest("Frontend base URL is not configured.");
            }

            var trimmedBase = frontendBaseUrl.TrimEnd('/');
            var reportUrl = $"{trimmedBase}/inspections/{inspectionId}/report?pdf=1";

            // Append agency context to the report URL so the PDF route can inject it
            // into localStorage for the Puppeteer browser. This is critical for SuperAdmin
            // users whose API calls require agencyId as a query parameter (non-SuperAdmin
            // users have it embedded in their JWT claims).
            var isSuperAdmin = User.IsSuperAdmin();
            if (isSuperAdmin && agencyId.HasValue)
            {
                reportUrl += $"&agencyId={agencyId.Value}&isSuperAdmin=true";
            }

            byte[] pdfBytes;
            try
            {
                var httpClient = _httpClientFactory.CreateClient("PdfGenerator");

                // Forward the Authorization header from the incoming request so the
                // frontend PDF endpoint can pass it to Puppeteer for authenticated browsing.
                var requestMessage = new HttpRequestMessage(HttpMethod.Get,
                    $"/api/pdf?url={Uri.EscapeDataString(reportUrl)}&filename=inspection-report-{inspectionId}.pdf");

                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(authHeader))
                {
                    requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);
                }

                var pdfResponse = await httpClient.SendAsync(requestMessage);

                if (!pdfResponse.IsSuccessStatusCode)
                {
                    var errorBody = await pdfResponse.Content.ReadAsStringAsync();
                    return StatusCode(502, new { message = $"Failed to generate PDF from the report engine. Status: {(int)pdfResponse.StatusCode}, Detail: {errorBody}" });
                }

                pdfBytes = await pdfResponse.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(502, new { message = $"PDF generation failed: {ex.Message}" });
            }

            // 5. Upload PDF to S3
            var s3Folder = $"agencies/{inspection.AgencyId}/inspections/{inspectionId}/pdf";
            var s3FileName = $"inspection-report-{inspectionId}.pdf";

            using var pdfStream = new MemoryStream(pdfBytes);
            var uploadResult = await _s3Service.UploadFileAsync(pdfStream, s3Folder, s3FileName, "application/pdf");

            if (!uploadResult.Success)
            {
                return StatusCode(500, new { message = "Failed to upload PDF to S3." });
            }

            var s3Url = _s3Service.BuildFileUrl($"{s3Folder}/{s3FileName}");

            // 6. Save the S3 URL to the inspection record
            await _inspectionService.UpdatePdfUrlAsync(inspectionId, agencyId, s3Url);

            return Ok(new { pdfUrl = s3Url, cached = false });
        }

        /// <summary>
        /// Sends the inspection report PDF via email to the specified recipient.
        /// The report must be in Closed status.
        /// Supports optional custom subject, body, and template ID.
        /// </summary>
        [HttpPost("inspection/{inspectionId}/send-email")]
        public async Task<ActionResult<ApiResponse<bool>>> SendReportEmail(
            Guid inspectionId,
            [FromBody] SendReportEmailRequest request,
            Guid? agencyId)
        {
            if (string.IsNullOrWhiteSpace(request?.RecipientEmail))
            {
                return BadRequest(new { message = "Recipient email is required." });
            }

            // 1. Fetch the inspection
            var inspectionResult = await _inspectionService.GetByIdAsync(inspectionId, agencyId);
            if (!inspectionResult.Success || inspectionResult.Data == null)
            {
                return NotFound(new { message = "Inspection not found." });
            }

            var inspection = inspectionResult.Data;

            // 2. Only allow email for Closed inspections
            if (inspection.InspectionStatus != InspectionStatus.Closed)
            {
                return BadRequest(new { message = "Report email can only be sent for closed inspection reports." });
            }

            // 3. Build the frontend report URL
            var frontendBaseUrl =
                _configuration["Frontend:BaseUrl"] ??
                _configuration["FrontendBaseUrl"] ??
                Request.Headers["Origin"].ToString();

            if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            {
                return BadRequest("Frontend base URL is not configured.");
            }

            var trimmedBase = frontendBaseUrl.TrimEnd('/');
            var reportUrl = $"{trimmedBase}/inspections/{inspectionId}/report?pdf=1";

            // 4. Generate the PDF (same as GetReportPdf, but we don't always need S3)
            // If a cached PDF exists, use its URL; otherwise generate one inline
            string pdfUrl = inspection.PdfUrl;
            if (string.IsNullOrWhiteSpace(pdfUrl))
            {
                // We need to generate the PDF first — reuse the same logic as GetReportPdf
                // but inline the PDF generation without storing to S3 for the email case
                var isSuperAdmin = User.IsSuperAdmin();
                if (isSuperAdmin && agencyId.HasValue)
                {
                    reportUrl += $"&agencyId={agencyId.Value}&isSuperAdmin=true";
                }

                byte[] pdfBytes;
                try
                {
                    var httpClient = _httpClientFactory.CreateClient("PdfGenerator");
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get,
                        $"/api/pdf?url={Uri.EscapeDataString(reportUrl)}&filename=inspection-report-{inspectionId}.pdf");

                    var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(authHeader))
                    {
                        requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);
                    }

                    var pdfResponse = await httpClient.SendAsync(requestMessage);
                    if (!pdfResponse.IsSuccessStatusCode)
                    {
                        var errorBody = await pdfResponse.Content.ReadAsStringAsync();
                        return StatusCode(502, new { message = $"Failed to generate PDF for email. Status: {(int)pdfResponse.StatusCode}, Detail: {errorBody}" });
                    }

                    pdfBytes = await pdfResponse.Content.ReadAsByteArrayAsync();
                }
                catch (Exception ex)
                {
                    return StatusCode(502, new { message = $"PDF generation failed: {ex.Message}" });
                }

                // Upload to S3 and save URL
                var s3Folder = $"agencies/{inspection.AgencyId}/inspections/{inspectionId}/pdf";
                var s3FileName = $"inspection-report-{inspectionId}.pdf";

                using var pdfStream = new MemoryStream(pdfBytes);
                var uploadResult = await _s3Service.UploadFileAsync(pdfStream, s3Folder, s3FileName, "application/pdf");
                if (!uploadResult.Success)
                {
                    return StatusCode(500, new { message = "Failed to upload PDF to S3." });
                }

                pdfUrl = _s3Service.BuildFileUrl($"{s3Folder}/{s3FileName}");
                await _inspectionService.UpdatePdfUrlAsync(inspectionId, agencyId, pdfUrl);
            }

            // 5. Determine subject and body — use custom values when provided, otherwise fallback to default
            var subject = !string.IsNullOrWhiteSpace(request.Subject)
                ? request.Subject
                : $"Inspection Report — {inspection.PropertyAddress ?? inspectionId.ToString()}";

            var body = !string.IsNullOrWhiteSpace(request.Body)
                ? request.Body
                : $@"
<p>Hello,</p>
<p>Please find the inspection report attached below:</p>
<p><strong>Property Address:</strong> {inspection.PropertyAddress ?? "N/A"}<br />
<strong>Inspection Date:</strong> {inspection.InspectionDate:yyyy-MM-dd}<br />
<strong>Report:</strong> <a href=""{pdfUrl}"">Download PDF Report</a></p>
<p>Thank you,<br />Property Inspection Team</p>";

            // Replace the report link merge tag if present in the body
            body = body.Replace("{{ReportLink}}", $"<a href=\"{pdfUrl}\">Download PDF Report</a>");

            try
            {
                await _emailService.SendAsync(
                    request.RecipientEmail,
                    subject,
                    body,
                    request.SendFromEmail,
                    request.SendFromName);
                return Ok(new ApiResponse<bool> { Success = true, Data = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Failed to send email: {ex.Message}" });
            }
        }
    }
}
