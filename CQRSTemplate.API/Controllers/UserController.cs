using CQRSTemplate.Application.Abstractions;
using CQRSTemplate.Application.UseCases.UserCases.Commands;
using CQRSTemplate.Application.UseCases.UserCases.Queries;
using CQRSTemplate.Domain.Entities.Models.PrimaryModels;
using CQRSTemplate.Domain.Entities.Views;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CQRSTemplate.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IApplicationDbContext _applicationDbContext;

        public UserController(IMediator mediator, IApplicationDbContext applicationDbContext)
        {
            _mediator = mediator;
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<ResponseModel> GetAll()
        {
            return await _mediator.Send(new GetAllUsersQuery());
        }
        
        [HttpPost]
        public async Task<ResponseModel> Create(CreateUserCommand request)
        {
            return await _mediator.Send(request);
        }

        [HttpPost]
        public async Task<ResponseModel> CreateBot(Bot request)
        {
            await _applicationDbContext.Bots.AddAsync(request);
            await _applicationDbContext.SaveChangesAsync(new CancellationToken());

            return new ResponseModel
            {
                IsSuccess = true,
                StatusCode = 200,
                Response = "Maybe sucess"
            };
        }

        [HttpDelete]
        public async Task<ResponseModel> DeleteBot(Guid id)
        {
            Bot bot = await _applicationDbContext.Bots.FirstOrDefaultAsync(b=>b.Id==id);
            _applicationDbContext.Bots.Remove(bot);
            await _applicationDbContext.SaveChangesAsync(new CancellationToken());

            return new ResponseModel
            {
                IsSuccess = true,
                StatusCode = 200,
                Response = "Maybe sucess"
            };
        }
    }
}
