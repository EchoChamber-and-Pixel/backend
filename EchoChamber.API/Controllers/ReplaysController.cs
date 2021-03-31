using EchoChamber.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EchoChamber.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReplaysController : ControllerBase
    {
        private readonly EchoContext _db;
        private readonly ILogger _logger;

        public ReplaysController(EchoContext db, ILogger<ReplaysController> logger)
        {
            _db = db;
            _logger = logger;
        }
        
        [HttpGet]
        public List<string> Get()
        {
            return new List<string>() { "hi" };
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var replay = await _db.Replays.Include(r => r.Record).ThenInclude(r => r.Map).FirstOrDefaultAsync(r => r.Id == id);
            if (replay == null)
                return NotFound("No such replay found");
            return File(replay.Data, "application/octet-stream", $"{replay.Record.Map.Name}_{replay.Record.SteamID64}_{replay.Record.Mode}_{replay.Record.Course}.replay");
        }
    }
}
