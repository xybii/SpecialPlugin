namespace SpecialPlugin.Project.OldDapperDemo
{
    public class Test : ITest
    {
        public virtual string Get()
        {
            return GetType().Namespace;
        }
    }
}
