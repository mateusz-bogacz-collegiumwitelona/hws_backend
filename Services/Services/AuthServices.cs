using Domain.Common;
using Domain.Constants;
using Domain.Models;
using DTO.Request;
using DTO.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;

namespace Services.Services;

public class AuthServices : IAuthServices
{
    private readonly UserManager<User> _userManager;
    private readonly TokenServices _token;

    public AuthServices(
        UserManager<User> userManager,
        TokenServices token
    )
    {
        _userManager = userManager;
        _token = token;
    }
    
    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u =>
                u.Email == request.Name ||
                u.NormalizedUserName == request.Name.Trim().ToUpper()
            );
            
            bool isPasswordValidate = false;

            if (user != null)
            {
                isPasswordValidate = await _userManager.CheckPasswordAsync(user, request.Password);
            }

            if (user == null || !isPasswordValidate)
            {
                return Result<AuthResponse>.Failure(
                    "Invalid username or password.",
                    ErrorCodes.InvalidCredentials,
                    StatusCodes.Status401Unauthorized
                );
            }

            string token = _token.CreateJwtToken(user);

            return Result<AuthResponse>.Success(
                "Login successful.",
                StatusCodes.Status200OK,
                new AuthResponse
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? ""
                }
            );
        }
        catch (Exception)
        {
            return Result<AuthResponse>.Failure(
                "An error occurred during login.",
                ErrorCodes.InternalError,
                StatusCodes.Status500InternalServerError
            );
        }
    }
}