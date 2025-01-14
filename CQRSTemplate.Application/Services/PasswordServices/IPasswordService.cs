using CQRSTemplate.Domain.Entities.Models.OtherModels;

namespace CQRSTemplate.Application.Services.PasswordServices
{
    public interface IPasswordService
    {
        public bool CheckPassword(string password, PasswordModel passwordModel);

        public PasswordModel HashPassword(string password);
    }
}
