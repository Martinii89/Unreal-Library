using Newtonsoft.Json;
using System;

namespace AssetExtraction.JSON
{
    internal abstract class BaseConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }
}

//object regex 