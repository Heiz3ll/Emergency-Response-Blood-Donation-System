using BloodDonorSystem.Infrastructure;
using BloodDonorSystem.Infrastructure.Identity;
using BloodDonorSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        IConfigurationSection jwt =
            builder.Configuration.GetSection("Jwt");

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwt["Issuer"],
                ValidAudience = jwt["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwt["SecretKey"]!))
            };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "SystemAdminOnly",
        policy => policy.RequireRole("System Admin"));

    options.AddPolicy(
        "DonorOnly",
        policy => policy.RequireRole("Donor"));

    options.AddPolicy(
        "HospitalAdminOnly",
        policy => policy.RequireRole("Hospital Admin"));
});

builder.Services.AddOpenApi();

var app = builder.Build();

using (IServiceScope scope =
       app.Services.CreateScope())
{
    ApplicationDbContext db =
        scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

    db.Database.Migrate();
}

using (IServiceScope scope =
       app.Services.CreateScope())
{
    RoleManager<ApplicationRole> roleManager =
        scope.ServiceProvider
            .GetRequiredService<RoleManager<ApplicationRole>>();

    string[] roles =
    [
        "Donor",
        "Hospital Admin",
        "System Admin"
    ];

    foreach (string role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(
                new ApplicationRole { Name = role });
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        Name = "Blood Donor System API",
        Status = "Running",
        Timestamp = DateTime.UtcNow
    });
});

app.MapControllers();

app.Run();