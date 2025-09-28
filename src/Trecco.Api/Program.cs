using Serilog;
using Trecco.Api.Extensions;
using Trecco.Api.Middleware;
using Trecco.Application;
using Trecco.Application.Common.Endpoints;
using Trecco.Aspire.ServiceDefaults;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecksConfiguration();

builder.Services.AddOpenApi();

builder.Host.AddSerilogLogging();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(corsPolicyBuilder =>
    {
        corsPolicyBuilder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin();
    });
});

WebApplication app = builder.Build();

app.UseCors();

app.UseSerilogRequestLogging();

app.MapDefaultEndpoints();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseDocumentation();
}

app.UseExceptionHandler(o => { });

app.UseHttpsRedirection();

await app.RunAsync();
