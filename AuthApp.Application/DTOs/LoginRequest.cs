namespace AuthApp.Application.DTOs;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
};

public class LoginResponse
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public LoginResponse() { }

    public LoginResponse(bool success, string token, string message)
    {
        Success = success;
        Token = token;
        Message = message;
    }
};
