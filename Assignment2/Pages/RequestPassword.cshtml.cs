using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assignment2.Services; // Make sure this matches the namespace of your IEmailSender
using Assignment2.ViewModels; // This is where your ViewModel is located
using System.Threading.Tasks;
using Assignment2.Model;

namespace Assignment2.Pages
{
    public class RequestPasswordModel : PageModel
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public PasswordResetRequestModel PasswordResetRequest { get; set; }


        public RequestPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(PasswordResetRequest.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToPage("./ResetRequestConfirmation");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            System.Diagnostics.Debug.WriteLine($"Password reset token for {user.Email}: {code}");

            var callbackUrl = Url.Page(
                "/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                PasswordResetRequest.Email,
                "Reset Password",
                $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

            return RedirectToPage("./ResetRequestConfirmation");
        }
    }
}
