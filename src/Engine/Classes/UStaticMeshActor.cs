using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UELib.Core;

namespace UELib.Engine.Classes
{
    [UnrealRegisterClass]
    public class UStaticMeshActor : UObject
    {

        static UStaticMeshActor()
        {
            if (UnrealConfig.VariableTypes == null)
            {
                UnrealConfig.VariableTypes = new Dictionary<string, Tuple<string, Types.PropertyType>>();
            }
            UnrealConfig.VariableTypes.Add("Materials", new Tuple<string, Types.PropertyType>("material", Types.PropertyType.ObjectProperty));
        }
        public UStaticMeshActor()
        {
            ShouldDeserializeOnDemand = true;
        }

        protected override void Deserialize()
        {
            var initial_pos = _Buffer.Position;
            var first_val = _Buffer.ReadInt32();
            if (first_val == -1)
            {
                _Buffer.Position = initial_pos;
            }else
            {
                //Skipping some unknown data.. ugly hack..
                _Buffer.Position = initial_pos + 22;
            }
            base.Deserialize();
        }
    }
}
