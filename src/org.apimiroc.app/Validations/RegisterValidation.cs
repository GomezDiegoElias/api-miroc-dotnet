using FluentValidation;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.app.Validations
{
    public class RegisterValidation : AbstractValidator<RegisterRequest>
    {

        public RegisterValidation()
        {
            RuleFor(x => x.Dni)
                .NotEmpty().WithMessage("El DNI es obligatorio.")
                .GreaterThan(0).WithMessage("El DNI debe ser un número positivo.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electronico es obligatorio.")
                .EmailAddress().WithMessage("Formato invalido de correo electronico.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatorio.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres..");
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no debe exceder los 50 caracteres.");
        }

    }
}
