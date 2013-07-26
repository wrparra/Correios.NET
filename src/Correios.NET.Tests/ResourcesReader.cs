using System.IO;
using System.Reflection;
using System.Text;

namespace Correios.NET.Tests
{
    public class ResourcesReader
    {
        internal static string GetResourceAsString(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var path = string.Format("Correios.NET.Tests.Resources.{0}", fileName);

            string result;

            using (var stream = assembly.GetManifestResourceStream(path))
            {
                using (var reader = new StreamReader(stream, Encoding.GetEncoding("ISO-8859-1")))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }
    }
}