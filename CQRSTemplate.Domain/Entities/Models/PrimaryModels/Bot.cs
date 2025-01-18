namespace CQRSTemplate.Domain.Entities.Models.PrimaryModels
{
    public class Bot
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public long BotId { get; set; }
        public string FirstName { get; set; }
        public string Username { get; set; }
        public User User { get; set; }
        public List<Vocabulary> Vocabulary { get; set; }
    }
}
