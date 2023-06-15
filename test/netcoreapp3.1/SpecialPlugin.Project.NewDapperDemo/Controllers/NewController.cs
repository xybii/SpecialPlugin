using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using SpecialPlugin.Project.NewDapperDemo.Dtos;
using SpecialPlugin.Project.NewDapperDemo.Models;
using System.Threading.Tasks;

namespace SpecialPlugin.Project.NewDapperDemo.Controllers
{
    [Route("New")]
    public class NewController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOptions<NewDapperDemoOptions> _options;

        public NewController(IMapper mapper, IOptions<NewDapperDemoOptions> options)
        {
            _mapper = mapper;
            _options = options;
        }

        [HttpGet(Order = 10)]
        public async Task<IActionResult> Get()
        {
            BookTagDto d = null;

            using (MySqlConnection connection = new MySqlConnection(_options.Value.DefaultConnection))
            {
                connection.Open();

                var t = await connection.QueryFirstOrDefaultAsync<BookTag>("SELECT * FROM BookTag");

                d = _mapper.Map<BookTagDto>(t);
            }

            return Ok(d);
        }
    }
}
