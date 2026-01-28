using AuthApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
