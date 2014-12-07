namespace Raygun
{
    using System.IO;

    using Raygun.Owin;

    internal class TestOwinContext : OwinContext
    {
        public TestOwinContext()
        {
            Request.Body = new MemoryStream();
        }
    }
}