using FluentValidation;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.app.Validations
{
    public class UserUpdateValidation : AbstractValidator<UserUpdateRequest>
    {

        public UserUpdateValidation()
        {
            RuleFor(x => x.Dni)
                .GreaterThan(0).WithMessage("El DNI debe ser un número positivo.")
                .InclusiveBetween(10_000_000, 99_999_999).WithMessage("El DNI debe tener exactamente 8 dígitos");
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("El correo electrónico no es válido.");
            //RuleFor(x => x.Password)
            //    .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.")
            //    .MaximumLength(100).WithMessage("La contraseña no puede tener más de 100 caracteres.");
            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage("El nombre no puede tener más de 50 caracteres.");
            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("El apellido no puede tener más de 50 caracteres.");
            RuleFor(x => x.Role)
                .MaximumLength(50).WithMessage("El rol no puede tener más de 50 caracteres.");
        }

    }
}
