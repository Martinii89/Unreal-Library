using Newtonsoft.Json;

namespace AssetExtraction.JSON
{
    internal class RightOfEqualSign : BaseConverter
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var s = (string)value;

            writer.WriteValue(s.Split(new[] { '=' }, 2)[1]);
        }
    }
}

//object regex 