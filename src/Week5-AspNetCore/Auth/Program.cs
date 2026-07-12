using System.Security.Claims;
using System.Text;
using Auth.Dtos;
using Auth.Models;
using Auth.Options;
using Auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddSingleton<ITokenService, TokenService>();

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("Missing 'Jwt' configuration section.");

// ─── Authentication: validate JWT tokens in middleware, not in handlers ─────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

// ─── Authorization: named policies instead of attribute-per-endpoint sprawl ─
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AdultsOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "age") &&
            int.TryParse(context.User.FindFirstValue("age"), out var age) &&
            age >= 18));
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// ─── AuthN: issue a token ────────────────────────────────────────────────────
app.MapPost("/login", (LoginRequest request, ITokenService tokenService) =>
{
    var user = DemoUserStore.Validate(request.Username, request.Password);
    if (user is null)
    {
        return Results.Unauthorized();
    }

    var (accessToken, expiresAtUtc) = tokenService.CreateAccessToken(user);
    return Results.Ok(new LoginResponse(accessToken, expiresAtUtc));
}).WithName("Login");

// ─── Claims-based identity: [Authorize] validates the token, handler reads claims ──
app.MapGet("/me", (ClaimsPrincipal user) =>
{
    var claims = user.Claims.Select(c => new { c.Type, c.Value });
    return Results.Ok(new { Name = user.Identity?.Name, Claims = claims });
}).RequireAuthorization().WithName("GetCurrentUser");

// ─── Named policy applied to a route group via RequireAuthorization ─────────
var admin = app.MapGroup("/admin").RequireAuthorization("AdminOnly");
admin.MapGet("/secret", () => Results.Ok(new { Message = "Only Admins can see this." }))
    .WithName("GetAdminSecret");

var adult = app.MapGroup("/adult").RequireAuthorization("AdultsOnly");
adult.MapGet("/content", () => Results.Ok(new { Message = "Only users 18+ can see this." }))
    .WithName("GetAdultContent");

Console.WriteLine("""

  ╔══════════════════════════════════════════════════════════════════╗
  ║  Week 5 — Authentication & Authorization demo                     ║
  ╠══════════════════════════════════════════════════════════════════╣
  ║  POST /login          → admin/admin123, alice/alice123, teen/teen123 ║
  ║  GET  /me              → [Authorize] claims from the bearer token  ║
  ║  GET  /admin/secret    → policy "AdminOnly" (RequireRole)         ║
  ║  GET  /adult/content   → policy "AdultsOnly" (claim age >= 18)    ║
  ╚══════════════════════════════════════════════════════════════════╝
""");

app.Run();

