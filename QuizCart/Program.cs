using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QuizCart.Data;
using QuizCart.Data;
using QuizCart.Interfaces;
using QuizCart.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Associate service interfaces with their implementations
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IAssessmentService, AssessmentService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IBrainFoodService, BrainFoodService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QuizCart API",
        Version = "v1",
        Description = "API documentation for QuizCart project"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    // Enable Swagger UI in development mode
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QuizCart API v1");
        c.RoutePrefix = "swagger"; // Swagger UI available at /swagger
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();