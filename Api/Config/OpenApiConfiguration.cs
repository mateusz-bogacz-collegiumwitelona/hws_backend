using Microsoft.OpenApi;

namespace Api.Config;

public static class OpenApiConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {

            options.AddDocumentTransformer(static (document, context, cancellationToken) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                var jwtScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Wklej tutaj swój wygenerowany token JWT (sam ciąg znaków)."
                };

                document.Components.SecuritySchemes.Add("Bearer", jwtScheme);

                document.Security ??= new List<OpenApiSecurityRequirement>();

                var schemeReference = new OpenApiSecuritySchemeReference("Bearer", document);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    [schemeReference] = new List<string>()
                };

                document.Security.Add(securityRequirement);

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static WebApplication UseSwaggerUIConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "Esp Home");
            });
        }

        return app;
    }
}