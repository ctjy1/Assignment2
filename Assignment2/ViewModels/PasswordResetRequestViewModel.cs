using System.ComponentModel.DataAnnotations;

public class PasswordResetRequestModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
