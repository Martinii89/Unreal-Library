using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UELib.Core;

namespace UELib.Engine.Classes.Components
{
    [UnrealRegisterClass]
    public class UStaticMeshComponent: UObject, IExtract
    {
        public UStaticMeshComponent()
        {
            
            ShouldDeserializeOnDemand = true;
        }
        protected override void Deserialize()
        {
            if (Package.Version > 400 && _Buffer.Length >= 12)
            {
                // componentClassIndex
                _Buffer.Position += sizeof(int);
                if (Name == "FXActor_TA_28")
                {
                    Console.WriteLine("here");
                }
                var componentNameIndex = _Buffer.ReadNameIndex();
                if (componentNameIndex == (int)Table.ObjectName)
                {
                    base.Deserialize();
                    return;
                }
                _Buffer.Position -= 12;
            }
            var initial_position = _Buffer.Position;
            try
            {
                var oindex1 = _Buffer.ReadObjectIndex();
                var oindex2 = _Buffer.ReadObjectIndex();
                if (oindex1 == 0 && oindex2 == -1)
                {
                    _Buffer.Position = initial_position + 4;
                }
                else
                {
                    _Buffer.Position = initial_position;
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                _Buffer.Position = initial_position;
            }
            base.Deserialize();
        }
    }
}
