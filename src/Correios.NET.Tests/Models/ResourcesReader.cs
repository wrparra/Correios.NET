using System.IO;
using System.Reflection;

namespace Correios.NET.Tests.Models
{
    public class ResourcesReader
    {
        internal static string GetResourceAsString(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var path = string.Format("Correios.NET.Tests.Resources.{0}", fileName);

            string result;
            
            using (var stream = assembly.GetManifestResourceStream(path))
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }

            return result;
        }
    }
}