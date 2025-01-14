using CQRSTemplate.Domain.Entities.Views;
using MediatR;

namespace CQRSTemplate.Application.UseCases.UserCases.Queries
{
    public class GetAllUsersQuery:IRequest<ResponseModel>
    {
    }
}
