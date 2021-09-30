using SpecialPlugin.Project.ADemo;

namespace SpecialPlugin.Project.BDemo
{
    public class Test : ITest
    {
        public string Get()
        {
            return GetType().Namespace;
        }
    }
}
