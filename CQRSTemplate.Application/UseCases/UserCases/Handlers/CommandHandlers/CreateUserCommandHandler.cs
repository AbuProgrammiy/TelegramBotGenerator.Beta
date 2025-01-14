using CQRSTemplate.Application.Abstractions;
using CQRSTemplate.Application.UseCases.UserCases.Commands;
using CQRSTemplate.Domain.Entities.Models.PrimaryModels;
using CQRSTemplate.Domain.Entities.Views;
using Mapster;
using MediatR;

namespace CQRSTemplate.Application.UseCases.UserCases.Handlers.CommandHandlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ResponseModel>
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public CreateUserCommandHandler(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<ResponseModel> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                User user = request.Adapt<User>();

                await _applicationDbContext.Users.AddAsync(user);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                return new ResponseModel
                {
                    IsSuccess = true,
                    StatusCode = 201,
                    Response = "User created successfully!"
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
