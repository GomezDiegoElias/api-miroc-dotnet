using FluentValidation;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.app.Validations
{
    public class ProviderValidation : AbstractValidator<ProviderRequest>
    {
        public ProviderValidation()
        {
            RuleFor(x => x.Cuit)
                .NotEmpty().WithMessage("El Cuit es obligatorio")
                .InclusiveBetween(10_000_000_000, 99_999_999_999)
                .WithMessage("El Cuit debe tener exactamente 11 dígitos");
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres");
            RuleFor(x => x.Address)
                .MaximumLength(100).WithMessage("La dirección no puede exceder los 100 caracteres");
            RuleFor(x => x.Description)
                .MaximumLength(300).WithMessage("La descripción no puede exceder los 300 caracteres");
        }
    }
}
