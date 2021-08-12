using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EchoChamber.API.Data;
using EchoChamber.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EchoChamber.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapsController : ControllerBase
    {
        private readonly EchoContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public MapsController(EchoContext db, IMapper mapper, ILogger<RecordsController> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<Map>> Get([FromQuery] string name)
        {
            var db = _db.Maps;
            var query = db.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(m => m.Name.Contains(name.ToLowerInvariant()));

            query = query.OrderBy(m => m.Name);

            return await query.ToArrayAsync();
        }
    }
}