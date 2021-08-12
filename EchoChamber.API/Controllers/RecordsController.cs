using AutoMapper;
using EchoChamber.API.Data;
using EchoChamber.API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoChamber.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecordsController : ControllerBase
    {
        private readonly EchoContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RecordsController(EchoContext db, IMapper mapper, ILogger<RecordsController> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets all records.
        /// </summary>
        /// <returns>List of records.</returns>
        [HttpGet]
        public async Task<IEnumerable<RecordView>> Get([FromQuery] RecordParameter recordParameter)
        {
            var db = _db.Records.Include(r => r.Map).Include(r => r.Player).Include(r => r.Replay);
            var query = db.AsQueryable();

            if (recordParameter.Id != null)
                query = query.Where(r => recordParameter.Id.Contains(r.Id));

            if (recordParameter.SteamID64 != null)
                query = query.Where(r => recordParameter.SteamID64.Contains(r.SteamID64));

            if (recordParameter.MapName != null)
                query = query.Where(r => r.Map.Name.Contains(recordParameter.MapName.ToLowerInvariant()));

            if (recordParameter.Course != null)
                query = query.Where(r => recordParameter.Course.Contains(r.Course));
            else
                query = query.Where(r => r.Course >= 0);

            if (recordParameter.IsBonus.HasValue)
                query = query.Where(r => recordParameter.IsBonus.Value ? r.Course > 0 : r.Course == 0);

            if (recordParameter.Style != null)
                query = query.Where(r => recordParameter.Style.Contains(r.Style));

            if (recordParameter.IsPro.HasValue)
                query = query.Where(r => recordParameter.IsPro.Value ? r.Teleports == 0 : r.Teleports > 0);

            if (recordParameter.Mode != null)
            {
                List<int> modes = new List<int>();
                if (recordParameter.Mode.Contains("kztimer", StringComparer.OrdinalIgnoreCase)
                    || recordParameter.Mode.Contains("kzt", StringComparer.OrdinalIgnoreCase)
                    || recordParameter.Mode.Contains("2"))
                    modes.Add(2);
                if (recordParameter.Mode.Contains("simplekz", StringComparer.OrdinalIgnoreCase)
                    || recordParameter.Mode.Contains("skz", StringComparer.OrdinalIgnoreCase)
                    || recordParameter.Mode.Contains("1"))
                    modes.Add(1);
                if (recordParameter.Mode.Contains("vanilla", StringComparer.OrdinalIgnoreCase)
                    || recordParameter.Mode.Contains("vnl", StringComparer.OrdinalIgnoreCase)
                    || recordParameter.Mode.Contains("0"))
                    modes.Add(0);
                query = query.Where(r => modes.Contains(r.Mode));
            }

            if (recordParameter.MapName != null)
                query = query.OrderBy(r => r.Course).ThenBy(r => r.Course);
            else
                query = query.OrderByDescending(r => r.Created);

            if (recordParameter.After.HasValue)
                query = query.Where(r => r.Created > recordParameter.After.Value);

            if (recordParameter.Before.HasValue)
                query = query.Where(r => r.Created < recordParameter.Before.Value);

            query = query.Skip(recordParameter.Offset.GetValueOrDefault(0));
            query = query.Take(recordParameter.Limit.GetValueOrDefault(100));

            var dbRecords = await query.ToArrayAsync();

            return _mapper.Map<IEnumerable<RecordView>>(dbRecords);
        }
    }
}