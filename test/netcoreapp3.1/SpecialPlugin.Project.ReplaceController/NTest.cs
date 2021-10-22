using AutoMapper;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using SpecialPlugin.Project.OldDapperDemo;
using SpecialPlugin.Project.OldDapperDemo.Dtos;
using SpecialPlugin.Project.OldDapperDemo.Models;

namespace SpecialPlugin.Project.ReplaceController
{
    public class NTest : Test
    {
        public override string Get()
        {
            return GetType().Namespace;
        }
    }

    //public class NTest : ITest
    //{
    //    private readonly IMapper _mapper;
    //    private readonly IOptions<OldDapperDemoOptions> _options;

    //    public NTest(IMapper mapper, IOptions<OldDapperDemoOptions> options)
    //    {
    //        _mapper = mapper;
    //        _options = options;
    //    }

    //    public string Get()
    //    {
    //        BookTagDto d = null;

    //        using (var connection = new MySqlConnection(_options.Value.DefaultConnection))
    //        {
    //            connection.Open();

    //            var t = connection.QueryFirstOrDefault<BookTag>("SELECT * FROM BookTag");

    //            d = _mapper.Map<BookTagDto>(t);
    //        }

    //        return $"{GetType().Namespace}:{d.Barcode}";
    //    }
    //}
}
