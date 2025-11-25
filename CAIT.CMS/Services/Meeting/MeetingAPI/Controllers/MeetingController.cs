using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MeetingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingController : ControllerBase
    {
        #region Fields
        private readonly IMediator _mediator;
        private readonly ILogger<MeetingController> _logger;
        #endregion

        #region Constructor
        public MeetingController(IMediator mediator
                                   , ILogger<MeetingController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        #endregion




    }
}
