using FluentValidation;

namespace AuthApp.Application.DTOs;

public record RegisterRequest(
    string FullName,
    string Username,
    string Email,
    string Password,
    DateTime BirthDate,
    string Gender
);

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100).WithMessage("Maksimal 100 karakter");
        RuleFor(x => x.Username).NotEmpty().MinimumLength(5).WithMessage("Minimal 5 karakter");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);

        // Validasi minimal 18 tahun
        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .Must(date => date <= DateTime.Today.AddYears(-18))
            .WithMessage("Minimal berusia 18 tahun untuk mendaftar");

        RuleFor(x => x.Gender).Must(g => g == "L" || g == "P").WithMessage("Gender harus L atau P");
    }
}
