using FluentValidation;
using IAS.Audit;

namespace IAS.CosmosDB.Validation
{
    public class AuditEventValidator: AbstractValidator<AuditEvent>
    {
        public AuditEventValidator()
        {
            RuleFor(e => e.Target)
                .NotNull()
                .Unless(a => a.EventType == AuditEventType.UserAction)
                .WithMessage("Target can not be null for Audit type entity mutation");

            RuleFor(a => a.EntityId).NotEmpty().NotEqual(0).WithMessage("EntityId can not be 0 or empty");
        }
    }
}
