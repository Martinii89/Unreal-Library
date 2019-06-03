using UELib.Core;

namespace UELib.Engine.Classes
{
    [UnrealRegisterClass]
    public class UStaticMeshActor : UObject, IExtract
    {
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
