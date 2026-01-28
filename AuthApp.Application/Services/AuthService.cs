using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthApp.Application.DTOs;
using AuthApp.Application.Interfaces;
using AuthApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;

    public AuthService(IApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
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

    private readonly IConfiguration _config;

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

        // Payload: Informasi yang ditanam di dalam token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role), // PENTING: Untuk Admin Page
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1), // Token hangus dalam 1 hari
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
