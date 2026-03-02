using Asp.Versioning;
using FigCommissionAnalyticsEngine.API.ExceptionHandling;
using FigCommissionAnalyticsEngine.Application;
using FigCommissionAnalyticsEngine.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddMvc();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string GhPagesCors = "GhPagesCors";

builder.Services.AddCors(options =>
{
    options.AddPolicy(GhPagesCors, policy =>
    {
        policy
            .WithOrigins(
                "https://masonwatson.github.io"
                // add custom domain too if you use one
            )
            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
            .WithHeaders("Content-Type", "Authorization");
    });
});

var app = builder.Build();

app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(GhPagesCors);

app.UseAuthorization();

app.MapControllers();

app.Run();
