using FluentValidation;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.app.Validations
{
    public class EmployeeValidation : AbstractValidator<EmployeeRequest>
    {
        public EmployeeValidation()
        {
            RuleFor(x => x.Dni)
                .NotEmpty().WithMessage("El DNI es obligatorio")
                .InclusiveBetween(10_000_000, 99_999_999)
                .WithMessage("El DNI debe tener exactamente 8 digitos");
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres");
            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("El apellido no puede exceder los 50 caracteres");
            RuleFor(x => x.WorkStation)
                .NotEmpty().WithMessage("El puesto de trabajo es obligatorio")
                .MaximumLength(25).WithMessage("El nombre del puesto de trabajo no puede exceder los 10 caracteres");
        }
    }
}
