using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Wbn.GestaoAdm.Api.Authentication;
using Wbn.GestaoAdm.Application.DependencyInjection;
using Wbn.GestaoAdm.Infrastructure.DependencyInjection;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

var builder = WebApplication.CreateBuilder(args);

var jwtSecretKey        = builder.Configuration["JWT_SECRET_KEY"]          ?? throw new InvalidOperationException("A variavel JWT_SECRET_KEY nao foi configurada.");
var jwtExpirationStr    = builder.Configuration["JWT_EXPIRATION_MINUTES"]  ?? "120";

builder.Services.Configure<JwtOptions>(options =>
{
    options.SecretKey           = jwtSecretKey;
    options.ExpirationInMinutes = int.TryParse(jwtExpirationStr, out var exp) ? exp : 120;
});

var jwtOptions = new JwtOptions
{
    SecretKey           = jwtSecretKey,
    ExpirationInMinutes = int.TryParse(jwtExpirationStr, out var expiration) ? expiration : 120
};

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
