namespace SpecialPlugin.Project.ADemo
{
    public class Test : ITest
    {
        public string Get()
        {
            return GetType().Namespace;
        }
    }
}
