namespace CQRSTemplate.Domain.Entities.Models.PrimaryModels
{
    public class Bot
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public User User { get; set; }
    }
}
