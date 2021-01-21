using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecordsController : ControllerBase
    {
        private readonly ILogger<RecordsController> _logger;

        public RecordsController(ILogger<RecordsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets all records.
        /// </summary>
        /// <returns>List of records.</returns>
        [HttpGet]
        public List<string> Get()
        {
            return new List<string>() { "EchoFrost - All WRs" };
        }
    }
}
