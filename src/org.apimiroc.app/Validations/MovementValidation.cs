using FluentValidation;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Request;

namespace org.apimiroc.app.Validations
{
    public class MovementValidation : AbstractValidator<MovementRequest>
    {
        public MovementValidation()
        {
            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("El monto no debe estar vacío.")
                .GreaterThan(0).WithMessage("El monto debe ser mayor que cero.");
            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("El método de pago no debe estar vacío.")
                .Must(pm => Enum.TryParse(typeof(PaymentMethod), pm, out _))
                .WithMessage("El método de pago no es válido.");
            RuleFor(x => x.ConceptId)
                .NotEmpty().WithMessage("El ID del concepto no debe estar vacío.")
                .GreaterThan(0).WithMessage("El ID del concepto debe ser mayor que cero.");
        }
    }
}
