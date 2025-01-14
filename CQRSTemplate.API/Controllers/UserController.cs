using CQRSTemplate.Application.UseCases.UserCases.Commands;
using CQRSTemplate.Application.UseCases.UserCases.Queries;
using CQRSTemplate.Domain.Entities.Views;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CQRSTemplate.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
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
    }
}
