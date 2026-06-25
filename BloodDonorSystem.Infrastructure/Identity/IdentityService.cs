using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BloodDonorSystem.Application.DTOs;
using BloodDonorSystem.Application.Exceptions;
using BloodDonorSystem.Application.Services;
using BloodDonorSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BloodDonorSystem.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _settings;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;

        _settings =
            configuration
                .GetSection("Jwt")
                .Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                "Jwt configuration missing.");
    }

    public async Task<AuthDto> CreateUserAsync(
        Guid userId,
        string email,
        string password,
        string role,
        CancellationToken cancellationToken = default)
    {
        // validate role FIRST before touching the DB
        bool roleExists = await _roleManager.RoleExistsAsync(role);

        if (!roleExists)
            throw new NotFoundException($"Role '{role}' does not exist.");

        ApplicationUser? existing = await _userManager.FindByEmailAsync(email);

        if (existing is not null)
            throw new ConflictException("Email already exists.");

        ApplicationUser identityUser =
            new()
            {
                DomainUserId = userId,
                Email = email,
                UserName = email,
                CreatedAtUtc = DateTime.UtcNow
            };

        IdentityResult result =
            await _userManager.CreateAsync(identityUser, password);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(x => x.Description)));

        IdentityResult roleResult =
            await _userManager.AddToRoleAsync(identityUser, role);

        if (!roleResult.Succeeded)
            throw new InvalidOperationException(
                string.Join(", ", roleResult.Errors.Select(x => x.Description)));

        IList<string> roles = [role];

        string accessToken = GenerateAccessToken(identityUser, roles);
        string refreshToken = GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = identityUser.Id,
            Token = refreshToken,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays)
        });

        return new AuthDto
        {
            UserId = identityUser.DomainUserId,
            Email = identityUser.Email!,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes),
            Roles = roles.ToArray()
        };
    }

    public async Task<AuthDto> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        ApplicationUser? user =
            await _userManager.FindByEmailAsync(email);

        if (user is null)
            throw new UnauthorizedException("Invalid credentials.");

        bool valid =
            await _userManager.CheckPasswordAsync(user, password);

        if (!valid)
            throw new UnauthorizedException("Invalid credentials.");

        IList<string> roles =
            await _userManager.GetRolesAsync(user);

        string accessToken =
            GenerateAccessToken(user, roles);

        string refreshToken =
            GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays),
            IsRevoked = false
        });


        return new AuthDto
        {
            UserId = user.DomainUserId,
            Email = user.Email!,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes),
            Roles = roles.ToArray()
        };
    }
    
    public async Task<AuthDto> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        RefreshToken? storedToken =
            await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(
                    x => x.Token == refreshToken,
                    cancellationToken);

        if (storedToken is null)
            throw new UnauthorizedException("Invalid refresh token.");

        if (storedToken.IsRevoked)
            throw new UnauthorizedException("Refresh token has been revoked.");

        if (storedToken.ExpiresAtUtc < DateTime.UtcNow)
            throw new UnauthorizedException("Refresh token expired.");

        ApplicationUser user = storedToken.User;

        IList<string> roles =
            await _userManager.GetRolesAsync(user);

        // generate NEW token FIRST (fix bug)
        string newRefreshToken = GenerateRefreshToken();

        string newAccessToken =
            GenerateAccessToken(user, roles);

        // revoke old token
        storedToken.IsRevoked = true;
        storedToken.RevokedAtUtc = DateTime.UtcNow;
        storedToken.ReplacedByToken = newRefreshToken;

        // add new token
        RefreshToken newToken =
            new()
            {
                UserId = user.Id,
                Token = newRefreshToken,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays),
                IsRevoked = false
            };

        _context.RefreshTokens.Add(newToken);

        // ❌ NO SAVE CHANGES

        return new AuthDto
        {
            UserId = user.DomainUserId,
            Email = user.Email!,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes),
            Roles = roles.ToArray()
        };
    }
    
    public async Task LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        RefreshToken? storedToken =
            await _context.RefreshTokens
                .FirstOrDefaultAsync(
                    x => x.Token == refreshToken,
                    cancellationToken);

        if (storedToken is null)
            return;

        storedToken.IsRevoked = true;
        storedToken.RevokedAtUtc = DateTime.UtcNow;
    }
    private string GenerateAccessToken(
        ApplicationUser user,
        IEnumerable<string> roles)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new("DomainUserId", user.DomainUserId.ToString())
        ];

        claims.AddRange(
            roles.Select(role => new Claim(
                ClaimTypes.Role,
                role)));

        SymmetricSecurityKey key =
            new(
                Encoding.UTF8.GetBytes(
                    _settings.SecretKey));

        SigningCredentials credentials =
            new(
                key,
                SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token =
            new(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    _settings.AccessTokenMinutes),
                signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }
}
    