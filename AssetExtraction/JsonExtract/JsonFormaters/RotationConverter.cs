using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;

namespace AssetExtraction.JSON
{
    internal class RotationConverter : BaseConverter
    {
        private static float URotToDegreeFactor = 0.005493f;
        private static Regex regex = new Regex(@"(-?\d+)");

        private float URotToDegree(int val)
        {
            return val * URotToDegreeFactor;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var s = (string)value;

            var matches = regex.Matches(s);
            var values = matches.Cast<Match>()
                                    .Select(m => URotToDegree(int.Parse(m.Value)))
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