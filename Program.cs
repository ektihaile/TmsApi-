using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Services;
using Scalar.AspNetCore; 

var builder = WebApplication.CreateBuilder(args);

// 1. የቪው (API) እና የFramework አገልግሎቶችን መመዝገቢያ
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// 2. የዳታቤዝ ግንኙነት መመዝገቢያ
builder.Services.AddDbContext<TmsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TmsDatabase")));

// 3. የእኛን CourseService ምዝገባ (በእያንዳንዱ ጥያቄ አዲስ እንዲሆን Scoped ተደርጓል)
builder.Services.AddScoped<ICourseService, CourseService>();

builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

var app = builder.Build();

// 4. የስህተትና የስታተስ ኮድ መቆጣጠሪያዎች (Middlewares)
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // Scalar API ዶክመንቴሽን ገፅ
}

app.MapControllers();

app.Run();