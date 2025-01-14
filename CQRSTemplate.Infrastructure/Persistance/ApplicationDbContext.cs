using CQRSTemplate.Application.Abstractions;
using CQRSTemplate.Domain.Entities.Models.PrimaryModels;
using Microsoft.EntityFrameworkCore;

namespace CQRSTemplate.Infrastructure.Persistance
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            Database.Migrate();  
        }

        public DbSet<User> Users { get; set; }
    }
}
