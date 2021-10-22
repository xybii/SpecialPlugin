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
        private readonly ITest _test;

        public OldController(IMapper mapper, IOptions<OldDapperDemoOptions> options,
            ITest test)
        {
            _mapper = mapper;
            _options = options;
            _test = test;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Get()
        {
            BookTagDto d = null;

            using (var connection = new MySqlConnection(_options.Value.DefaultConnection))
            {
                connection.Open();

                var t = await connection.QueryFirstOrDefaultAsync<BookTag>("SELECT * FROM BookTag");

                d = _mapper.Map<BookTagDto>(t);
            }

            return Ok(d);
        }

        [HttpGet("test")]
        public virtual async Task<IActionResult> Test()
        {
            return Ok(_test.Get());
        }
    }
}
