using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using SpecialPlugin.DapperOneDemo.Dtos;
using SpecialPlugin.DapperOneDemo.Models;
using System.Threading.Tasks;

namespace SpecialPlugin.DapperOneDemo.Controllers
{
    [Route("One")]
    public class OneController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOptions<DapperOneDemoOptions> _options;

        public OneController(IMapper mapper, IOptions<DapperOneDemoOptions> options)
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
