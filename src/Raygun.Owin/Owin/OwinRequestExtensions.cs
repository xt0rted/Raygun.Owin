namespace Raygun.Owin
{
    using System.IO;
    using System.Text;

    using Raygun.Owin.Infrastructure;

    internal static class OwinRequestExtensions
    {
        /// <summary>
        /// Reads and parses the request body as a form.
        /// </summary>
        /// <returns>The parsed form data.</returns>
        internal static IFormCollection ReadForm(this OwinRequest request)
        {
            var form = request.Get<IFormCollection>("Microsoft.Owin.Form#collection");
            if (form == null)
            {
                request.Body.Seek(0, SeekOrigin.Begin);

                string text;

                // Don't close, it prevents re-winding.
                using (var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4 * 1024, leaveOpen: true))
                {
                    text = reader.ReadToEnd();
                }

                // re-wind for the next caller
                request.Body.Seek(0, SeekOrigin.Begin);

                form = OwinHelpers.GetForm(text);
                request.Set("Microsoft.Owin.Form#collection", form);
            }

            return form;
        }

        internal static string BodyAsString(this OwinRequest request)
        {
            request.Body.Seek(0, SeekOrigin.Begin);

            string text;

            // Don't close, it prevents re-winding
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4 * 1024, leaveOpen: true))
            {
                text = reader.ReadToEnd();
            }

            // re-wind for the next caller
            request.Body.Seek(0, SeekOrigin.Begin);

            return text;
        }
    }
}