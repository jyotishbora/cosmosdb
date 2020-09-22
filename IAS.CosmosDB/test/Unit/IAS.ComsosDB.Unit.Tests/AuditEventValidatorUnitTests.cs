using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentValidation.TestHelper;
using IAS.Audit;
using IAS.CosmosDB.Validation;
using Xunit;

namespace IAS.CosmosDB.Unit.Tests
{
    public class AuditEventValidatorUnitTests
    {
        private readonly AuditEventValidator _auditEventValidator;

        public AuditEventValidatorUnitTests()
        {
            _auditEventValidator = new AuditEventValidator();
        }

        [Fact]
        public void AuditEventValidator_Should_Throw_Error_When_EntityId_Empty()
        {
            var fixture = new Fixture();
            var auditEvent = fixture.Build<AuditEvent>()
                .With(a => a.EntityId, 0)
                .Create();
            var result = _auditEventValidator.TestValidate(auditEvent);
            result.ShouldHaveValidationErrorFor(e => e.EntityId);

        }

        [Fact]
        public void AuditEventValidator_Should_Throw_Error_When_TargetEmpty_Mutation_Audit()
        {
            var fixture = new Fixture();
            var auditEvent = fixture.Build<AuditEvent>()
                .With(e => e.EventType, AuditEventType.EntityMutation)
                .With(e => e.Target, () => null)
                .Create();
            var result = _auditEventValidator.TestValidate(auditEvent);
            result.ShouldHaveValidationErrorFor(e => e.Target);
            result.Errors.First().ErrorMessage.Should().Contain("Target can not be null for Audit type entity mutation");
        }
    }
}
