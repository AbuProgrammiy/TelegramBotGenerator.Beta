using CQRSTemplate.Domain.Entities.Enums;

namespace CQRSTemplate.Domain.Entities.Models.PrimaryModels
{
    public class User
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Status Status { get; set; }
        public string? Username { get; set; }
    }
}
