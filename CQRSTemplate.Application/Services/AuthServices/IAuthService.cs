using CQRSTemplate.Domain.Entities.Models.PrimaryModels;

namespace CQRSTemplate.Application.Services.AuthServices
{
    public interface IAuthService
    {
        public string GenerateToken(User user);
    }
}
