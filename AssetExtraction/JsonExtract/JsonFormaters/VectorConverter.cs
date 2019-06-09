using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;

namespace AssetExtraction.JSON
{
    internal class VectorConverter : BaseConverter
    {
        private static Regex regex = new Regex(@"(?<==)[-\d.]+");

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var s = (string)value;
            var matches = regex.Matches(s);
            var values = matches.Cast<Match>()
                                    .Select(m => float.Parse(m.Value))
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