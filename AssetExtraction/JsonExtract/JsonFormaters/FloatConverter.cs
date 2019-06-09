using Newtonsoft.Json;

namespace AssetExtraction.JSON
{
    internal class FloatConverter : BaseConverter
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var s = (string)value;

            writer.WriteValue(float.Parse(s.Split(new[] { '=' }, 2)[1]));
        }
    }
}

//object regex 