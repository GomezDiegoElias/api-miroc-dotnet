using FluentValidation;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.app.Validations
{
    public class ConceptValidation : AbstractValidator<ConceptRequest>
    {
        public ConceptValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido")
                .MaximumLength(100).WithMessage("El nombre no debe exceder los 100 caracteres");
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("El tipo es requerido")
                .MaximumLength(50).WithMessage("El tipo no debe de exceder los 50 caracteres");
            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("La descripcion no debe de exceder los 250 caracteres");
        }
    }
}
