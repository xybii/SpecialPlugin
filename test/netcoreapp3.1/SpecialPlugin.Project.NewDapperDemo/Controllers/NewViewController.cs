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
    [Route("NewView")]
    public class NewViewController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IOptions<NewDapperDemoOptions> _options;

        public NewViewController(IMapper mapper, IOptions<NewDapperDemoOptions> options)
        {
            _mapper = mapper;
            _options = options;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            BookTagDto d = null;

            using (var connection = new MySqlConnection(_options.Value.DefaultConnection))
            {
                connection.Open();

                var t = await connection.QueryFirstOrDefaultAsync<BookTag>("SELECT * FROM BookTag");

                d = _mapper.Map<BookTagDto>(t);
            }

            return View(d);
        }
    }
}
