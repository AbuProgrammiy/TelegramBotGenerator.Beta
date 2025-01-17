namespace CQRSTemplate.Domain.Entities.Models.PrimaryModels
{
    public class Vocabulary
    {
        public Guid Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public Bot Bot { get; set; }
    }
}
