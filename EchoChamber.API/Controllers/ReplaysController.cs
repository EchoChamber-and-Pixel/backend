using EchoChamber.API.Data;
using EchoChamber.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private static long BaseSteamID64 = 76561197960265728;

        [HttpPost]
        public async Task<object> Post()
        {
            StringValues source;
            if (!Request.Headers.TryGetValue("Source", out source))
                source = new StringValues("web");
            _logger.LogInformation($"Replay from {source.ToString()}");

            try
            {
                using var ms = new MemoryStream();
                await Request.Body.CopyToAsync(ms);
                ms.Position = 0;
                var replay = new ReplayFile(ms);
                //  Map
                var map = new Map
                {
                    Name = replay.MapName.ToLowerInvariant()
                };
                if (!_db.Maps.Any(m => m.Name == map.Name))
                {
                    _logger.LogInformation($"New map added: {map.Name}");
                    await _db.Maps.AddAsync(map);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    map = await _db.Maps.FirstOrDefaultAsync(m => m.Name == map.Name);
                }

                // Player
                int Y = replay.SteamId / 2;
                int Z = replay.SteamId % 2;
                long steamId64 = BaseSteamID64 + (Y * 2) + Z;
                var player = new Player
                {
                    Name = replay.PlayerName,
                    SteamID64 = steamId64
                };
                if (!_db.Players.Any(p => p.SteamID64 == steamId64))
                {
                    _logger.LogInformation($"New player added: {player.Name} - {player.SteamID64}");
                    await _db.Players.AddAsync(player);
                    await _db.SaveChangesAsync();
                }

                // Record
                var record = new Record
                {
                    SteamID64 = steamId64,
                    MapId = map.Id,
                    Course = replay.Course,
                    Style = (int)replay.Style,
                    Mode = (int)replay.Mode,
                    Teleports = replay.TeleportsUsed,
                    Time = (long)Math.Floor(replay.Time.TotalMilliseconds),
                    Created = DateTime.Now
                };
                await _db.Records.AddAsync(record);
                await _db.SaveChangesAsync();

                using var newReplay = new MemoryStream();
                replay.Write(newReplay);
                newReplay.Position = 0;
                // Replay
                var replay2 = new Replay
                {
                    Id = record.Id,
                    Source = source.ToString(),
                    Data = newReplay.ToArray()
                };
                await _db.Replays.AddAsync(replay2);
                await _db.SaveChangesAsync();
                return new {
                    Id = record.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading replay");
                return new {
                    Id = -1,
                    Message = "Unable to upload replay"
                };
            }
        }
    }
}
