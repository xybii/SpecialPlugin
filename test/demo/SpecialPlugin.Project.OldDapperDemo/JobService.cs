using AutoMapper;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SpecialPlugin.Project.OldDapperDemo.Dtos;
using SpecialPlugin.Project.OldDapperDemo.Models;
using System;
using System.Threading.Tasks;

namespace SpecialPlugin.Project.OldDapperDemo
{
    public class JobService : IJobService
    {
        private bool _disposedValue;
        private readonly IMapper _mapper;
        private readonly IOptions<OldDapperDemoOptions> _options;

        public JobService(IMapper mapper, IOptions<OldDapperDemoOptions> options)
        {
            _mapper = mapper;
            _options = options;
        }

        public async Task Execute()
        {
            var connection = new MySqlConnection(_options.Value.DefaultConnection);

            connection.Open();

            var t = await connection.QueryFirstOrDefaultAsync<BookTag>("SELECT * FROM BookTag");

            var d = _mapper.Map<BookTagDto>(t);

            Console.WriteLine($"OldDapperDemo,Json:{JsonConvert.SerializeObject(d)}");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                _disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~JobService()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
