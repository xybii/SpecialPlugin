using AutoMapper;
using SpecialPlugin.AutoMapper;
using SpecialPlugin.DapperTwoDemo.Dtos;
using SpecialPlugin.DapperTwoDemo.Models;
using System;

namespace SpecialPlugin.DapperTwoDemo.AutoMappingMap
{
    public class BookTagAutoMap : AutoMappingConfiguration
    {
        public override void Map(IMapperConfigurationExpression cfg)
        {
            Console.WriteLine($"DapperTwoDemoJob,RegisterAutoMapper");

            cfg.CreateMap<BookTag, BookTagDto>();
        }
    }
}
