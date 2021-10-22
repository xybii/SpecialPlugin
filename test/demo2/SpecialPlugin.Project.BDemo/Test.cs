using AutoMapper;
using SpecialPlugin.Project.ADemo;

namespace SpecialPlugin.Project.BDemo
{
    public class Test : ITest
    {
        private readonly IMapper _mapper;

        public Test(IMapper mapper)
        {
            _mapper = mapper;
        }

        public string Get()
        {
            return GetType().Namespace;
        }
    }
}
