using FluentValidation;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.app.Validations
{
    public class LoginValidation : AbstractValidator<LoginRequest>
    {
        public LoginValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electronico es obligatorio.")
                .EmailAddress().WithMessage("Formato invalido de correo electronico.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");
        }
    }
}
