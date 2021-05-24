using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using SpecialPlugin.Project.OldDapperDemo.Dtos;
using SpecialPlugin.Project.OldDapperDemo.Models;
using System.Threading.Tasks;

namespace SpecialPlugin.Project.OldDapperDemo.Controllers
{
    [Route("Old")]
    public class OldController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOptions<OldDapperDemoOptions> _options;

        public OldController(IMapper mapper, IOptions<OldDapperDemoOptions> options)
        {
            _mapper = mapper;
            _options = options;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var connection = new MySqlConnection(_options.Value.DefaultConnection);

            connection.Open();

            var t = await connection.QueryFirstOrDefaultAsync<BookTag>("SELECT * FROM BookTag");

            var d = _mapper.Map<BookTagDto>(t);

            return Ok(d);
        }
    }
}
