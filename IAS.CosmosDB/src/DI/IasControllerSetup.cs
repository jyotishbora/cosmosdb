using System;
using FluentValidation.AspNetCore;
using Ias.Extensions.DependencyInjection;
using Ias.Standards;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace IAS.CosmosDB.DI
{
    public static class IasControllerSetup
    {
        public static IMvcBuilder AddControllerConfiguration(this IMvcBuilder builder)
        {
            builder.AddFluentValidation(fvc =>
                    fvc.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddIasMvcControllerTracing()
                .ConfigureProblemDetailsExceptionMapping(opt =>
                {
                    opt.Map<ArgumentException>(ex => new ProblemDetails
                    {
                        Title = ex.Message,
                        Status = 400,
                        Detail = ex.StackTrace
                    });
                });
            return builder;
        }
    }
}