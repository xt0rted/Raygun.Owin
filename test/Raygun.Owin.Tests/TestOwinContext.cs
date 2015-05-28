namespace Raygun
{
    using System.IO;

    using Raygun.LibOwin;

    internal class TestOwinContext : OwinContext
    {
        public TestOwinContext()
        {
            Request.Body = new MemoryStream();
        }
    }
}