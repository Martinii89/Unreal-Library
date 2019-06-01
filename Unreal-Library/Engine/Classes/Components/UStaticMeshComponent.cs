using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UELib.Core;

namespace UELib.Engine.Classes.Components
{
    [UnrealRegisterClass]
    public class UStaticMeshComponent: UObject
    {
        public UStaticMeshComponent()
        {
            
            ShouldDeserializeOnDemand = true;
        }
        protected override void Deserialize()
        {
            //could it be this simple?
            _Buffer.Position += sizeof(int);
            base.Deserialize();
        }
    }
}
