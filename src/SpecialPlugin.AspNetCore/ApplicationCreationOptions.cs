namespace SpecialPlugin.AspNetCore
{
    public class ApplicationCreationOptions
    {
        public PlugInSourceList PlugInSources { get; }

        public ApplicationCreationOptions()
        {
            PlugInSources = new PlugInSourceList();
        }
    }
}
