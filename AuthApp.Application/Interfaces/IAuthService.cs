using AuthApp.Application.DTOs;
using AuthApp.Domain.Entities;

namespace AuthApp.Application.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<List<User>> GetAllUsersAsync();
}
