using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using CQRSTemplate.Domain.Entities.Views;
using CQRSTemplate.Domain.Entities.DTOs;

namespace CQRSTemplate.Application.Services.EmailServices
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ResponseModel> SendEmailAsync(EmailDTO emailDTO)
        {
            try
            {
                IConfigurationSection emailSettings = _configuration.GetSection("EmailSettings");

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(emailSettings["Sender"]!, emailSettings["SenderName"]),
                    Subject = emailDTO.Subject,
                    Body = emailDTO.Body,
                    IsBodyHtml = emailDTO.IsBodyHTML
                };

                mailMessage.To.Add(emailDTO.To);

                using var smtpClient = new SmtpClient(emailSettings["MailServer"], int.Parse(emailSettings["MailPort"]!))
                {
                    Port = Convert.ToInt32(emailSettings["MailPort"]),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(emailSettings["Sender"], emailSettings["Password"]),
                    EnableSsl = true,
                };

                await smtpClient.SendMailAsync(mailMessage);

                return new ResponseModel
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Response = "Email Succesfully sent to user!"
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Response = $"Something went wrong!: {ex.Message}"
                };
            }
        }

    }
}
