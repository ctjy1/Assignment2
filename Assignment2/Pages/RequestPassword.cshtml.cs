using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Assignment2.Model;

namespace Assignment2.Pages
{
    [ValidateAntiForgeryToken]
    public class RequestPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailService emailService;

        public RequestPasswordModel(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            this.userManager = userManager;
            this.emailService = emailService;
        }

        [BindProperty]
        public string Email { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                // To avoid user enumeration, you might still want to redirect to a confirmation page
                return RedirectToPage("ResetRequestConfirmation");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = GenerateResetLink(user, token);
            if (resetLink == null)
            {
                // Handle the case where the reset link could not be generated
                ModelState.AddModelError("", "Error generating password reset link.");
                return Page();
            }
            await SendResetEmailAsync(Email, resetLink);

            return RedirectToPage("ResetRequestConfirmation");
        }

        private string GenerateResetLink(ApplicationUser user, string token)
        {
            return Url.Page(
                "/ResetPassword",
                pageHandler: null,
                values: new { email = user.Email, code = token }, // Ensure 'code' matches the expected query parameter in the OnGet
                protocol: Request.Scheme);
        }


        private async Task SendResetEmailAsync(string email, string link)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                throw new ArgumentNullException(nameof(link));
            }

            var encodedLink = HtmlEncoder.Default.Encode(link);
            var message = $"Please reset your password by clicking here: <a href='{encodedLink}'>link</a>";
            await emailService.SendEmailAsync("freshfarmmarket@mail.com", email, "Reset Password Link", message);
        }

        public void OnGet()
        {
        }
    }
}
