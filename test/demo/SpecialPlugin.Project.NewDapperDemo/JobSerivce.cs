using AutoMapper;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SpecialPlugin.Project.NewDapperDemo.Dtos;
using SpecialPlugin.Project.NewDapperDemo.Models;
using System;
using System.Threading.Tasks;

namespace SpecialPlugin.Project.NewDapperDemo
{
    public class JobService : IJobService
    {
        private readonly IMapper _mapper;
        private readonly IOptions<NewDapperDemoOptions> _options;

        public JobService(IMapper mapper, IOptions<NewDapperDemoOptions> options)
        {
            _mapper = mapper;
            _options = options;
        }

        public async Task Execute()
        {
            using (var connection = new MySqlConnection(_options.Value.DefaultConnection))
            {
                connection.Open();

                var t = await connection.QueryFirstOrDefaultAsync<BookTag>("SELECT * FROM BookTag");

                var d = _mapper.Map<BookTagDto>(t);

                Console.WriteLine($"NewDapperDemo,Json:{JsonConvert.SerializeObject(d)}");
            }
        }
    }
}
