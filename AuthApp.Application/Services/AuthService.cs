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

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // 1. Cari user berdasarkan email
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            return new LoginResponse(false, "", "Email atau Password salah.");

        // 2. Verifikasi Password (BCrypt akan bandingkan plain text dengan hash)
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
            return new LoginResponse(false, "", "Email atau Password salah.");

        // 3. Generate Token (Kita akan panggil helper method di sini)
        var token = GenerateJwtToken(user);

        return new LoginResponse(true, token, "Login Berhasil!");
    }

    private string GenerateJwtToken(User user)
    {
        // Di sini biasanya kita pakai library System.IdentityModel.Tokens.Jwt
        // Untuk saat ini, kita asumsikan return string token dummy
        // sampai kita setup Jwt di Program.cs API
        return "dummy-token-untuk-" + user.Username;
    }
}
