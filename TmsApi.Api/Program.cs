using TmsApi.Infrastructure.Persistence;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;

using TmsApi.Application.Dtos;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Services;
using TmsApi.Infrastructure.Repositories;

using Asp.Versioning;
using MediatR;
using FluentValidation;

using TmsApi.Application.Behaviors;
using TmsApi.Application.Enrollments.Commands;


var builder = WebApplication.CreateBuilder(args);


// MediatR

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(EnrollStudentHandler).Assembly);

    // IMPORTANT:
    // LoggingBehavior must be registered before ValidationBehavior.
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});



// FluentValidation


builder.Services.AddValidatorsFromAssembly(
    typeof(EnrollStudentValidator).Assembly);



// ProblemDetails + Global Exception Handler


builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<
    TmsApi.Api.ExceptionHandlers.GlobalExceptionHandler>();


// OpenAPI


builder.Services.AddOpenApi();


// API Versioning


builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader =
        new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});



// Controllers


builder.Services.AddControllers(options =>
{
    // options.Filters.Add<AuditLogFilter>();
});



// DbContext


builder.Services.AddDbContext<TmsDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("TmsDatabase")));



// Application Services


builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

// Repository registrations
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();


var app = builder.Build();



// Middleware


app.UseMiddleware<TmsApi.Api.Middleware.V1DeprecationMiddleware>();

app.UseExceptionHandler();

app.UseStatusCodePages();



// OpenAPI / Scalar


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}



// Controllers


app.MapControllers();


// Database Seeding


if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var context =
        scope.ServiceProvider.GetRequiredService<TmsDbContext>();

    await DataSeeder.SeedAsync(context);
}


app.Run();