using FluentValidation;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.app.Validations
{
    public class ConstructionValidation : AbstractValidator<ConstructionRequest>
    {
        public ConstructionValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio");
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("La fecha de inicio es obligatoria");
            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("La fecha de finalización es obligatoria")
                .GreaterThan(x => x.StartDate).WithMessage("La fecha de finalización debe ser posterior a la fecha de inicio");
            RuleFor(x => x.Address)
                .MaximumLength(100).WithMessage("La dirección no puede exceder los 100 caracteres");
            RuleFor(x => x.Description)
                .MaximumLength(300).WithMessage("La descripción no puede exceder los 300 caracteres");
        }
    }
}
