using Microsoft.AspNetCore.Mvc;
using PropertyInspection.Shared;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegalController : ControllerBase
    {
        private const string PrivacyPolicyText = """
            Privacy Policy

            Last updated: June 2026

            1. Information We Collect
            We collect information you provide directly, such as account details, property data, and inspection records. We may also collect usage data automatically when you interact with our platform.

            2. How We Use Your Information
            Your information is used to provide, maintain, and improve our services, communicate with you, and ensure the security of your account and data.

            3. Data Sharing
            We do not sell your personal data. We may share information with third-party service providers who assist in operating our platform, subject to confidentiality agreements.

            4. Data Retention
            We retain your data for as long as your account is active or as needed to provide services. You may request deletion of your data by contacting us.

            5. Security
            We implement industry-standard security measures to protect your data against unauthorized access, alteration, or destruction.

            6. Your Rights
            Depending on your jurisdiction, you may have rights to access, correct, delete, or port your data. Contact us to exercise these rights.

            7. Changes to This Policy
            We may update this Privacy Policy from time to time. Continued use of the platform constitutes acceptance of any changes.

            8. Contact
            If you have questions about this Privacy Policy, please contact us at support@easeinspect.com.
            """;

        private const string TermsAndConditionsText = """
            Terms & Conditions

            Last updated: June 2026

            1. Acceptance of Terms
            By accessing or using EaseInspect, you agree to be bound by these Terms & Conditions. If you do not agree, do not use the platform.

            2. Description of Service
            EaseInspect provides a property inspection management platform allowing users to create, manage, and share property inspection reports.

            3. User Accounts
            You are responsible for maintaining the confidentiality of your account credentials and for all activities under your account. Notify us immediately of any unauthorized use.

            4. Acceptable Use
            You agree not to misuse the platform, including but not limited to uploading malicious content, attempting to breach security, or using the service for unlawful purposes.

            5. Intellectual Property
            All content, trademarks, and intellectual property on the platform are owned by or licensed to EaseInspect. You retain ownership of data you upload.

            6. Limitation of Liability
            EaseInspect is provided "as is" without warranties of any kind. We are not liable for any damages arising from your use of the platform, to the extent permitted by law.

            7. Termination
            We reserve the right to suspend or terminate accounts that violate these terms, without prior notice.

            8. Governing Law
            These Terms shall be governed by the laws of the jurisdiction in which EaseInspect operates.

            9. Changes to Terms
            We may modify these Terms at any time. Changes will be effective upon posting. Continued use constitutes acceptance.

            10. Contact
            For questions about these Terms, please contact us at support@easeinspect.com.
            """;

        /// <summary>
        /// Returns the Privacy Policy text.
        /// </summary>
        [HttpGet("privacy-policy")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public IActionResult GetPrivacyPolicy()
        {
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Privacy policy retrieved successfully",
                Data = PrivacyPolicyText
            });
        }

        /// <summary>
        /// Returns the Terms & Conditions text.
        /// </summary>
        [HttpGet("terms-and-conditions")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public IActionResult GetTermsAndConditions()
        {
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Terms & conditions retrieved successfully",
                Data = TermsAndConditionsText
            });
        }
    }
}