using CQRSTemplate.Domain.Entities.DTOs;
using CQRSTemplate.Domain.Entities.Views;

namespace CQRSTemplate.Application.Services.EmailServices
{
    public interface IEmailService
    {
        public Task<ResponseModel> SendEmailAsync(EmailDTO emailDTO);
    }
}
