using PropertyInspection.Application.IServices;
using PropertyInspection.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Application.Services
{
    public class WhitelabelIntegrationService : IWhitelabelIntegrationService
    {
        private readonly IAgencyWhitelabelService _whitelabelService;

        public WhitelabelIntegrationService(IAgencyWhitelabelService whitelabelService)
        {
            _whitelabelService = whitelabelService;
        }

        // Report generation with branding
        public async Task<WhitelabelReportSettingsDto> GetReportBrandingAsync(Guid agencyId)
        {
            return await _whitelabelService.GetReportSettingsAsync(agencyId);
        }

        //public async Task<string> GenerateReportHeaderHtmlAsync(int agencyId, string reportTitle)
        //{
        //    var settings = await _whitelabelService.GetReportSettingsAsync(agencyId);
        //    var branding = await _whitelabelService.GetBrandingAsync(agencyId);

        //    var headerHtml = new StringBuilder();
        //    headerHtml.AppendLine("<div class=\"report-header\" style=\"text-align: center; margin-bottom: 30px;\">");

        //    // Logo
        //    if (!string.IsNullOrWhiteSpace(settings.LogoUrl))
        //    {
        //        headerHtml.AppendLine($"<img src=\"{settings.LogoUrl}\" alt=\"Agency Logo\" style=\"max-height: 80px; margin-bottom: 20px;\" />");
        //    }

        //    // Header text
        //    var headerText = !string.IsNullOrWhiteSpace(settings.ReportHeaderText) 
        //        ? settings.ReportHeaderText 
        //        : reportTitle;

        //    headerHtml.AppendLine($"<h1 style=\"color: {branding.PrimaryColor}; font-family: {branding.FontFamily}; margin: 0;\">{headerText}</h1>");

        //    headerHtml.AppendLine("</div>");

        //    return headerHtml.ToString();
        //}

        //public async Task<string> GenerateReportFooterHtmlAsync(int agencyId)
        //{
        //    var settings = await _whitelabelService.GetReportSettingsAsync(agencyId);
        //    var branding = await _whitelabelService.GetBrandingAsync(agencyId);

        //    var footerHtml = new StringBuilder();
        //    footerHtml.AppendLine("<div class=\"report-footer\" style=\"margin-top: 30px; padding-top: 20px; border-top: 1px solid #e5e7eb;\">");

        //    // Contact details
        //    if (settings.ContactInfo != null)
        //    {
        //        footerHtml.AppendLine("<div style=\"margin-bottom: 15px;\">");
        //        if (!string.IsNullOrWhiteSpace(settings.ContactInfo.Address))
        //            footerHtml.AppendLine($"<p style=\"margin: 5px 0; font-size: 12px;\">Address: {settings.ContactInfo.Address}</p>");
        //        if (!string.IsNullOrWhiteSpace(settings.ContactInfo.Phone))
        //            footerHtml.AppendLine($"<p style=\"margin: 5px 0; font-size: 12px;\">Phone: {settings.ContactInfo.Phone}</p>");
        //        if (!string.IsNullOrWhiteSpace(settings.ContactInfo.Email))
        //            footerHtml.AppendLine($"<p style=\"margin: 5px 0; font-size: 12px;\">Email: {settings.ContactInfo.Email}</p>");
        //        footerHtml.AppendLine("</div>");
        //    }

        //    // Footer text
        //    if (!string.IsNullOrWhiteSpace(settings.ReportFooterText))
        //    {
        //        footerHtml.AppendLine($"<p style=\"font-size: 11px; color: {branding.TextColor}; text-align: center; margin: 10px 0;\">{settings.ReportFooterText}</p>");
        //    }

        //    // Terms and conditions
        //    if (!string.IsNullOrWhiteSpace(settings.TermsAndConditions))
        //    {
        //        footerHtml.AppendLine($"<p style=\"font-size: 10px; color: #6b7280; text-align: center; margin: 10px 0;\">{settings.TermsAndConditions}</p>");
        //    }

        //    footerHtml.AppendLine("</div>");

        //    return footerHtml.ToString();
        //}

        //public async Task<string> GenerateReportCssAsync(int agencyId)
        //{
        //    var branding = await _whitelabelService.GetBrandingAsync(agencyId);

        //    var css = new StringBuilder();
        //    css.AppendLine("<style>");
        //    css.AppendLine($"body {{ font-family: {branding.FontFamily}; color: {branding.TextColor}; background-color: {branding.BackgroundColor}; }}");
        //    css.AppendLine($".primary-color {{ color: {branding.PrimaryColor}; }}");
        //    css.AppendLine($".secondary-color {{ color: {branding.SecondaryColor}; }}");
        //    css.AppendLine($".accent-color {{ color: {branding.AccentColor}; }}");
        //    css.AppendLine($".primary-bg {{ background-color: {branding.PrimaryColor}; }}");
        //    css.AppendLine($".secondary-bg {{ background-color: {branding.SecondaryColor}; }}");
        //    css.AppendLine($".accent-bg {{ background-color: {branding.AccentColor}; }}");
        //    css.AppendLine($".border-primary {{ border-color: {branding.PrimaryColor}; }}");
        //    css.AppendLine("table { width: 100%; border-collapse: collapse; margin: 20px 0; }");
        //    css.AppendLine("th, td { padding: 12px; text-align: left; border-bottom: 1px solid #e5e7eb; }");
        //    css.AppendLine($"th {{ background-color: {branding.PrimaryColor}; color: white; }}");
        //    css.AppendLine("</style>");

        //    return css.ToString();
        //}

        // Email template with branding
        public async Task<string> GenerateEmailHeaderHtmlAsync(Guid agencyId)
        {
            var settings = await _whitelabelService.GetReportSettingsAsync(agencyId);
            var branding = await _whitelabelService.GetBrandingAsync(agencyId);

            var headerHtml = new StringBuilder();
            headerHtml.AppendLine($"<div style=\"background-color: {branding.PrimaryColor}; padding: 20px; text-align: center;\">");

            if (!string.IsNullOrWhiteSpace(settings.LogoUrl))
            {
                headerHtml.AppendLine($"<img src=\"{settings.LogoUrl}\" alt=\"Agency Logo\" style=\"max-height: 60px;\" />");
            }

            headerHtml.AppendLine("</div>");

            return headerHtml.ToString();
        }

        //public async Task<string> GenerateEmailFooterHtmlAsync(int agencyId)
        //{
        //    var settings = await _whitelabelService.GetReportSettingsAsync(agencyId);
        //    var branding = await _whitelabelService.GetBrandingAsync(agencyId);

        //    var footerHtml = new StringBuilder();
        //    footerHtml.AppendLine($"<div style=\"background-color: {branding.BackgroundColor}; padding: 20px; margin-top: 20px; border-top: 1px solid {branding.PrimaryColor};\">");

        //    if (settings.ContactInfo != null)
        //    {
        //        footerHtml.AppendLine("<div style=\"text-align: center; font-size: 12px; color: #6b7280;\">");
        //        if (!string.IsNullOrWhiteSpace(settings.ContactInfo.Address))
        //            footerHtml.AppendLine($"<p style=\"margin: 5px 0;\">{settings.ContactInfo.Address}</p>");
        //        if (!string.IsNullOrWhiteSpace(settings.ContactInfo.Phone))
        //            footerHtml.AppendLine($"<p style=\"margin: 5px 0;\">Phone: {settings.ContactInfo.Phone}</p>");
        //        if (!string.IsNullOrWhiteSpace(settings.ContactInfo.Email))
        //            footerHtml.AppendLine($"<p style=\"margin: 5px 0;\">Email: {settings.ContactInfo.Email}</p>");
        //        footerHtml.AppendLine("</div>");
        //    }

        //    footerHtml.AppendLine("</div>");

        //    return footerHtml.ToString();
        //}

        //public async Task<WhitelabelContactDetailsDto?> GetAgencyContactDetailsAsync(int agencyId)
        //{
        //    var settings = await _whitelabelService.GetReportSettingsAsync(agencyId);
        //    return settings.ContactInfo;
        //}

        // UI branding
        public async Task<WhitelabelBrandingDto> GetUIBrandingAsync(Guid agencyId)
        {
            return await _whitelabelService.GetBrandingAsync(agencyId);
        }

        //public async Task<string> GenerateUIThemeCssAsync(int agencyId)
        //{
        //    var branding = await _whitelabelService.GetBrandingAsync(agencyId);

        //    var css = new StringBuilder();
        //    css.AppendLine(":root {");
        //    css.AppendLine($"  --primary-color: {branding.PrimaryColor};");
        //    css.AppendLine($"  --secondary-color: {branding.SecondaryColor};");
        //    css.AppendLine($"  --accent-color: {branding.AccentColor};");
        //    css.AppendLine($"  --background-color: {branding.BackgroundColor};");
        //    css.AppendLine($"  --text-color: {branding.TextColor};");
        //    css.AppendLine($"  --font-family: {branding.FontFamily};");
        //    css.AppendLine("}");

        //    return css.ToString();
        //}

        // Domain-based branding lookup
        //public async Task<WhitelabelBrandingDto> GetBrandingByDomainAsync(string domain)
        //{
        //    var whitelabel = await _whitelabelService.GetByCustomDomainAsync(domain);
        //    if (whitelabel != null)
        //    {
        //        return await _whitelabelService.GetBrandingAsync(whitelabel.AgencyId);
        //    }

        //    // Return default branding
        //    var defaultBranding = await _whitelabelService.GetDefaultBrandingAsync();
        //    return new WhitelabelBrandingDto
        //    {
        //        LogoUrl = defaultBranding.LogoUrl,
        //        PrimaryColor = defaultBranding.PrimaryColor,
        //        SecondaryColor = defaultBranding.SecondaryColor,
        //        FontFamily = defaultBranding.FontFamily,
        //        AccentColor = defaultBranding.AccentColor,
        //        BackgroundColor = defaultBranding.BackgroundColor,
        //        TextColor = defaultBranding.TextColor,
        //        FaviconUrl = defaultBranding.FaviconUrl,
        //        IsActive = false
        //    };
        //}

        //public async Task<int?> GetAgencyIdByDomainAsync(string domain)
        //{
        //    var whitelabel = await _whitelabelService.GetByCustomDomainAsync(domain);
        //    return whitelabel?.AgencyId;
        //}

        // Asset URLs with fallbacks
        //public async Task<string> GetLogoUrlAsync(int agencyId)
        //{
        //    var branding = await _whitelabelService.GetBrandingAsync(agencyId);
        //    return !string.IsNullOrWhiteSpace(branding.LogoUrl) 
        //        ? branding.LogoUrl 
        //        : "/assets/default-logo.png";
        //}

        //public async Task<string> GetFaviconUrlAsync(int agencyId)
        //{
        //    var branding = await _whitelabelService.GetBrandingAsync(agencyId);
        //    return !string.IsNullOrWhiteSpace(branding.FaviconUrl) 
        //        ? branding.FaviconUrl 
        //        : "/assets/default-favicon.ico";
        //}
    }
}
