using FluentValidation;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.app.Validations
{
    public class ClientValidation : AbstractValidator<ClientRequest>
    {
        public ClientValidation()
        {
            RuleFor(x => x.Dni)
                .NotEmpty().WithMessage("El DNI es obligatorio")
                .InclusiveBetween(10_000_000, 99_999_999)
                .WithMessage("El DNI debe tener exactamente 8 dígitos");
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres");
            RuleFor(x => x.Address)
                .MaximumLength(100).WithMessage("La dirección no puede exceder los 100 caracteres");
        }
    }
}
