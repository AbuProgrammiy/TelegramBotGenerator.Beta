using CQRSTemplate.Domain.Entities.Models.PrimaryModels;
using Microsoft.EntityFrameworkCore;

namespace CQRSTemplate.Application.Abstractions
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Bot> Bots { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
