using CQRSTemplate.Domain.Entities.Views;
using MediatR;

namespace CQRSTemplate.Application.UseCases.BotCases.Queries
{
    public class GetAllBotsQuery:IRequest<ResponseModel>
    {
    }
}
