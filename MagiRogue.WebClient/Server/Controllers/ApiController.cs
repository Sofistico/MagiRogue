using Microsoft.AspNetCore.Mvc;

namespace MagiRogue.WebClient.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController<T> : ControllerBase
    {
        protected readonly ILogger<T> _logger;

        public ApiController(ILogger<T> logger)
        {
            _logger = logger;
        }
    }
}
