using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VirtoCommerce.CustomerReviews.Test
{
    public static class TestHelper
    {
        public static T LoadFromJsonFile<T>(string fileName)
        {
            var filePath = Path.Combine(@"..\..\..\TestData", fileName);
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
        }

        public static dynamic LoadArrayFromJsonFile(string fileName)
        {
            var filePath = Path.Combine(@"..\..\..\TestData", fileName);
            return JArray.Parse(File.ReadAllText(filePath));
        }
    }
}
