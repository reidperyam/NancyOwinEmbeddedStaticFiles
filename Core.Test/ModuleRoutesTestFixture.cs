namespace Core.Test
{
    using Core;
    using Microsoft.Owin.Testing;
    using NUnit.Framework;
    using System.IO;
    using System.Linq;
    using System.Net;
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

            MemoryStream assemblyMemoryStream = new MemoryStream();
            typeof(Hooker).Assembly.GetManifestResourceStream(embeddedResourcePath).CopyTo(assemblyMemoryStream);
            MemoryStream httpMemoryStream = new MemoryStream();
            response.Content.ReadAsStreamAsync().Result.CopyTo(httpMemoryStream);

            // compare content returned via HTTP with manually retrieved from assembly by the [TestFixture]
            Assert.IsTrue(CompareMemoryStreams(assemblyMemoryStream, httpMemoryStream));
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
        [Test, Description("Send an HTTP request to the /time route configured in Core.Module verify an associated view, an embedded resource file in a different assembly (UI.dll), is returned as expected.")]
        public void TimeRouteReturnsOK()
        {
            var response = _server.HttpClient.GetAsync("/time").Result;
            Assert.AreEqual(true, response.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Category("Core.Module")]
        [Test, Description("Send an HTTP request to the /image route configured in Core.Module verify an associated view, an embedded resource file in a different assembly (UI.dll), is returned as expected.")]
        public void ImageRouteReturnsOK()
        {
            var response = _server.HttpClient.GetAsync("/image").Result;
            Assert.AreEqual(true, response.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        private static bool CompareMemoryStreams(MemoryStream ms1, MemoryStream ms2)
        {
            if (ms1.Length != ms2.Length)
                return false;

            ms1.Position = 0;
            ms2.Position = 0;

            var msArray1 = ms1.ToArray();
            var msArray2 = ms2.ToArray();

            return msArray1.SequenceEqual(msArray2);
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
