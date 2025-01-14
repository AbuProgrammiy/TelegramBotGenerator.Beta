using CQRSTemplate.Domain.Entities.Views;
using MediatR;

namespace CQRSTemplate.Application.UseCases.UserCases.Commands
{
    public class CreateUserCommand:IRequest<ResponseModel>
    {
        public string Name { get; set; }
    }
}
