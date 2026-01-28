using AuthApp.Application.DTOs;

namespace AuthApp.Application.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterRequest request);
}
