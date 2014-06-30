namespace Core
{
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.Conventions;
    using Nancy.Diagnostics;
    using Nancy.TinyIoc;
    using Nancy.ViewEngines;
    using UI;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            ResourceViewLocationProvider.RootNamespaces.Add(typeof(Hooker).Assembly, "UI");
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                // we have chosen to keep our static html/cshtml/sshtml views in an assembly separate from Nancy (this is sort of uncommon) because of 
                // this we override the default mechanism that Nancy uses to locate views and tell Nancy to locate views that are embedded as resources 
                // within an assembly. Up above we further define the UI assembly as the container for these embedded resource views.
                return NancyInternalConfiguration.WithOverrides(c => c.ViewLocationProvider = typeof(ResourceViewLocationProvider));
            }
        }
    }
}