using Infraestructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infraestructure.Auth;

public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtProvider _jwtProvider;
    private readonly RefreshTokenService _refreshTokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        JwtProvider jwtProvider,
        RefreshTokenService refreshTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtProvider = jwtProvider;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<(string accessToken, string refreshToken)> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            throw new Exception("Invalid credentials");

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
            throw new Exception("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtProvider.GenerateToken(user, roles);

        var refreshToken = await _refreshTokenService.CreateAsync(user.Id);

        return (accessToken, refreshToken.Token);
    }

    public async Task RegisterAsync(string email, string password)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task<(string accessToken, string refreshToken)> RefreshAsync(string token)
    {
        var refreshToken = await _refreshTokenService.GetAsync(token);

        if (refreshToken is null || refreshToken.ExpiresAt < DateTime.UtcNow)
            throw new Exception("Invalid refresh token");

        var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
        if (user is null)
            throw new Exception("User not found");

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtProvider.GenerateToken(user, roles);

        await _refreshTokenService.RevokeAsync(token);
        var newRefresh = await _refreshTokenService.CreateAsync(user.Id);

        return (accessToken, newRefresh.Token);
    }
}
