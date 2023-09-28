using Stride.Core.Mathematics;
using Stride.Engine;

namespace StrideSurvivor.Player
{
    public class CameraController : SyncScript
    {
        public TransformComponent Player;
        public Vector3 Offset = new(0, 0, 5);

        public override void Start()
        {
        }

        public override void Update()
        {
            Entity.Transform.Position = Player.WorldMatrix.TranslationVector + Offset;
        }
    }
}
