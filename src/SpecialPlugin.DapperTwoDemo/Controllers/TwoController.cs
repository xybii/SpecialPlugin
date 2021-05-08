using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using SpecialPlugin.DapperTwoDemo.Dtos;
using SpecialPlugin.DapperTwoDemo.Models;
using System.Threading.Tasks;

namespace SpecialPlugin.DapperTwoDemo.Controllers
{
    [Route("Two")]
    public class TwoController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOptions<DapperTwoDemoOptions> _options;

        public TwoController(IMapper mapper, IOptions<DapperTwoDemoOptions> options)
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
