namespace Core
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Owin;
    using Owin;
    using Microsoft.Owin.FileSystems;
    using Nancy.Owin;
    using Microsoft.Owin.StaticFiles;
    using Microsoft.Owin.Extensions;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileSystem = new EmbeddedResourceFileSystem(typeof(UI.Hooker).Assembly, "UI")
            });
            app.UseNancy();
            app.UseStageMarker(PipelineStage.MapHandler); // http://goo.gl/XrYGMh
        }
    }
}
