using AssetExtraction.JSON;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UELib.Core;

namespace AssetExtraction
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class DataNode
    {
        public DataNode parent;
        public UObject nodeData;

        [JsonProperty("name", Order = 1)]
        public string Name => nodeData.Name;

        [JsonProperty("Location", Order = 2)]
        [JsonConverter(typeof(VectorConverter))]
        public string Location => TryGetPropertyValue("Location");

        [JsonProperty("Translation", Order = 3)]
        [JsonConverter(typeof(VectorConverter))]
        public string Translation => TryGetPropertyValue("Translation");

        [JsonProperty("Rotation", Order = 4)]
        [JsonConverter(typeof(RotationConverter))]
        public string Rotation => TryGetPropertyValue("Rotation");



        //[JsonProperty("DrawScale", Order = 5)]
        //[JsonConverter(typeof(FloatConverter))]
        //public string DrawScale => TryGetPropertyValue("DrawScale");

        //[JsonProperty("DrawScale3D", Order = 6)]
        //[JsonConverter(typeof(VectorConverter))]
        //public string DrawScale3D => TryGetPropertyValue("DrawScale3D");

        [JsonProperty("Scale", Order = 5)]
        public List<float> Scale
        {
            get
            {
                var scale3D_s = TryGetPropertyValue("Scale3D");
                var DrawScale3D_s = TryGetPropertyValue("DrawScale3D");
                var scale_s = TryGetPropertyValue("Scale");
                var DrawScale_s = TryGetPropertyValue("DrawScale");

                var Scale3D = scale3D_s != null ? GetVectorValues(scale3D_s) : new List<float>() { 1, 1, 1 };
                var DrawScale3D = DrawScale3D_s != null ? GetVectorValues(DrawScale3D_s) : new List<float>() { 1, 1, 1 };
                var Scale = scale_s != null ? GetFloatValue(scale_s) : 1;
                var DrawScale = DrawScale_s != null ? GetFloatValue(DrawScale_s) : 1;
                List<float> res = (from scale3D in Scale3D.Zip(DrawScale3D, (v1, v2) => v1 * v2)
                                   select Scale * DrawScale * scale3D).ToList();
                return res;
            }
        }
        public bool ShouldSerializeScale()
        {
            if (TryGetPropertyValue("Scale3D") != null) return true;
            if (TryGetPropertyValue("DrawScale3D") != null) return true;
            if (TryGetPropertyValue("Scale") != null) return true;
            if (TryGetPropertyValue("DrawScale") != null) return true;
            return false;
        }

        [JsonProperty("StaticMesh", Order = 7)]
        [JsonConverter(typeof(ObjectConverter))]
        public string StaticMesh => TryGetPropertyValue("StaticMesh");

        [JsonProperty("Materials", Order = 8)]
        [JsonConverter(typeof(ListObjectConverter))]
        public string Materials => TryGetPropertyValue("Materials");

        [JsonProperty("InvisiTekMaterials", Order = 9)]
        [JsonConverter(typeof(ListObjectConverter))]
        public string InvisiTekMaterials => TryGetPropertyValue("InvisiTekMaterials");

        


        [JsonProperty("subNodes", Order = 10)]
        public List<DataNode> subNodes = new List<DataNode>();

        public bool ShouldSerializesubNodes() => subNodes.Count > 0;

        public DataNode(UObject nodeData)
        {
            this.nodeData = nodeData;
        }

        public void AddChild(DataNode node)
        {
            if (!subNodes.Contains(node))
            {
                subNodes.Add(node);
                node.parent = this;
            }
        }

        private string TryGetPropertyValue(string propName)
        {
            string propValue = nodeData?.Properties?.Find(propName)?.Decompile();
            if (propValue == null)
            {
                return null;
            }
            return propValue;
        }

        private List<float> GetVectorValues(string vectorValue)
        {
            Regex regex = new Regex(@"(?<==)[-\d.]+");
            var matches = regex.Matches(vectorValue);
            var values = matches.Cast<Match>()
                                    .Select(m => float.Parse(m.Value))
                                    .ToList();
            return values;
        }

        private float GetFloatValue(string floatValue)
        {
            return float.Parse(floatValue.Split(new[] { '=' }, 2)[1]);
        }
    }
}

//object regex 