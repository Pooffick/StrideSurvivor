using Stride.Core.Mathematics;
using Stride.Engine;

namespace StrideSurvivor
{
    public class CameraController : SyncScript
    {
        private TransformComponent _transform;

        public TransformComponent Player;
        public Vector3 Offset = new(0, 0, 5);

        public override void Start()
        {
            _transform = Entity.Transform;
        }

        public override void Update()
        {
            _transform.Position = Player.WorldMatrix.TranslationVector + Offset;
        }
    }
}
