namespace AuthApp.Application.DTOs;

public record LoginRequest(string Email, string Password);

public record LoginResponse(bool Success, string Token, string Message);
