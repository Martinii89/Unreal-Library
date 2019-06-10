

using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;

namespace AssetExtraction.JSON
{
    internal class ListObjectConverter : BaseConverter
    {
        private static Regex regex = new Regex(@"(?<=')([\w.]*)(?=')");

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var s = (string)value;
            var matches = regex.Matches(s);
            var values = matches.Cast<Match>()
                                    .Select(m => m.Value)
                                    .ToArray();


            if (values.Count() > 0)
            {
                writer.WriteStartArray();
                foreach (var val in values)
                {
                    writer.WriteValue(val);
                }
                writer.WriteEndArray();
            }
        }
    }
}

//object regex 