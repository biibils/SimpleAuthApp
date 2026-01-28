using AuthApp.Application.DTOs;
using AuthApp.Application.Interfaces;
using AuthApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;

    public AuthService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new Exception("Email sudah terdaftar.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            FullName = request.FullName,
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            BirthDate = request.BirthDate,
            Gender = char.Parse(request.Gender),
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(); // Method ini juga ada di Interface

        return "Registrasi berhasil!";
    }
}
