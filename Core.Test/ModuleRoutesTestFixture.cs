namespace Core.Test
{
    using Core;
    using Microsoft.Owin.Testing;
    using NUnit.Framework;
    using System;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using UI;

    [TestFixture]
    public class ModuleRouteTestFixture 
    {
        protected TestServer _server;

        string[] GetEmbeddedResourcePaths() { return typeof(Hooker).Assembly.GetManifestResourceNames(); }

        string[] GetNonEmbeddedContentPaths() { return new string[] { "UI.Hooker.cs" };}

        [TestFixtureSetUp]
        public virtual void TestFixtureSetup()
        {
            _server = TestServer.Create<Startup>();
        }

        [TestFixtureTearDown]
        public virtual void TestFixtureTearDown()
        {
            _server.Dispose();
        }

        [Category("Core.Microsoft.Owin.StaticFiles")]
        [Test, Description("Send an HTTP request to Microsoft.Owin.StaticFiles for a embedded resource file in a different assembly (UI.dll) and verify the response.")]
        public void CanRetrieveEmbeddedResourceFromSeparateAssemblyWithHttp([ValueSource("GetEmbeddedResourcePaths")]string embeddedResourcePath)
        {
            // request the embedded resource served via HTTP from Owin.StaticFiles
            var response = _server.HttpClient.GetAsync(embeddedResourcePath.Replace("UI.", "/")).Result;
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // compare content returned via HTTP with manually retrieved from assembly by the [TestFixture]
            Assert.That(typeof(Hooker).Assembly.GetManifestResourceStream(embeddedResourcePath).StreamAsString(),
                         Is.EqualTo(response.Content.ReadAsStringAsync().Result));
        }

        [Category("Core.Microsoft.Owin.StaticFiles")]
        [Test, Description("Send an HTTP request to Microsoft.Owin.StaticFiles for a file in a different assembly (UI.dll) that is not an embedded resource and verify the response.")]
        public void CannotRetrieveOtherFiles([ValueSource("GetNonEmbeddedContentPaths")]string nonEmbeddedResourcePath)
        {
            // request the embedded resource served via HTTP from Owin.StaticFiles
            var response = _server.HttpClient.GetAsync(nonEmbeddedResourcePath.Replace("UI.", "/")).Result;
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Category("Core.Module")]
        [Test, Description("Send an HTTP request to the /hello route configured in Core.Module verify that the view, an embedded resource file in a different assembly (UI.dll), is returned as expected.")]
        public void HelloRouteReturnsHelloHtml()
        {
            var response = _server.HttpClient.GetAsync("/hello").Result;
            Assert.AreEqual(true, response.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.That(response.Content.ReadAsStringAsync().Result, 
                Is.EqualTo("Hello World from Embedded Resource View in a separate assembly, hello.html"));
        }

        [Category("Core.Module")]
        [Test, Description("Send an HTTP request to the /time route configured in Core.Module verify that the view is returned as expected and the contents represents a scripted DateTime.")]
        public void TimeRouteReturnsViewOfScriptedTime()
        {
            var response = _server.HttpClient.GetAsync("/time").Result;
            Assert.AreEqual(true, response.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            //now verify that the view's AJAX request to load the embedded moment.min.js also succeeded
            Match parsedScriptedDateFromView = new Regex(@"(?<=\[)(.*?)(?=\])").Match(response.Content.ReadAsStringAsync().Result);
            Assert.IsTrue(parsedScriptedDateFromView.Success);//if this succeeds then the returned via asynchronously requested and received the embedded .js library, moment.js served from Microsoft.Owin.StaticFiles
            DateTime dt;
            Assert.IsTrue(DateTime.TryParse(parsedScriptedDateFromView.Value, out dt));
            Assert.AreEqual(DateTime.Today, dt.Date);
        }
    }

    static class Extensions
    {
        public static string StreamAsString(this Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
