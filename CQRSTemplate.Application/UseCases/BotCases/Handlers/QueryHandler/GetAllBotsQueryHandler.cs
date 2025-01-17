using CQRSTemplate.Application.Abstractions;
using CQRSTemplate.Application.UseCases.BotCases.Queries;
using CQRSTemplate.Domain.Entities.Models.PrimaryModels;
using CQRSTemplate.Domain.Entities.Views;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRSTemplate.Application.UseCases.BotCases.Handlers.QueryHandler
{
    public class GetAllBotsQueryHandler : IRequestHandler<GetAllBotsQuery, ResponseModel>
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public GetAllBotsQueryHandler(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<ResponseModel> Handle(GetAllBotsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<Bot> bots=await _applicationDbContext.Bots.ToListAsync(cancellationToken);

                return new ResponseModel
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Response = bots
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Response = "Something went wrong: {ex.Message}"
                };
            }
        }
    }
}
