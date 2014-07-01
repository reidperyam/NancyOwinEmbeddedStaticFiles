namespace Core
{
    using Microsoft.Owin.Extensions;
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.StaticFiles;
    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                FileSystem = new EmbeddedResourceFileSystem(typeof(UI.Hooker).Assembly, "UI")
            });
            app.UseNancy();
            app.UseStageMarker(PipelineStage.MapHandler); // http://goo.gl/XrYGMh
        }
    }
}
