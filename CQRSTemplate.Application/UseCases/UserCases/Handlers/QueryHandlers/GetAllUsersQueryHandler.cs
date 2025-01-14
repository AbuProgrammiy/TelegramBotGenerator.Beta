using CQRSTemplate.Application.Abstractions;
using CQRSTemplate.Application.UseCases.UserCases.Queries;
using CQRSTemplate.Domain.Entities.Models.PrimaryModels;
using CQRSTemplate.Domain.Entities.Views;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRSTemplate.Application.UseCases.UserCases.Handlers.QueryHandlers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ResponseModel>
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public GetAllUsersQueryHandler(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<ResponseModel> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<User> users = await _applicationDbContext.Users.ToListAsync(cancellationToken);

                return new ResponseModel
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Response = users
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Response = ex.Message
                };
            }
        }
    }
}
