using CQRSTemplate.Application.UseCases.BotCases.Queries;
using CQRSTemplate.Domain.Entities.Views;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CQRSTemplate.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IMediator _mediatr;

        public BotController(IMediator mediatr)
        {
            _mediatr = mediatr;
        }

        [HttpGet]
        public async Task<ResponseModel> GetAll()
        {
            return await _mediatr.Send(new GetAllBotsQuery());
        }
    }
}
