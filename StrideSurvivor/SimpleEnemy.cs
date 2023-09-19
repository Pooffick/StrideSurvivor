using System.Net.NetworkInformation;
using System.Threading.Tasks;
using BulletSharp;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace StrideSurvivor
{
    public class SimpleEnemy : StartupScript
    {
        private RigidbodyComponent _rigidbody;
        private SpriteAnimator _animator;
        private bool _initialized = false;

        [DataMemberIgnore]
        public Vector3 Position => Entity.Transform.Position;

        public float MovementSpeed = 30f;

        public bool Dying { get; private set; } = false;

        public override void Start()
        {
            _rigidbody = Entity.Get<RigidbodyComponent>();
            _animator = Entity.Get<SpriteAnimator>();

            _initialized = true;
            _animator.Play("Run");
        }

        public async Task Die()
        {
            Dying = true;

            _rigidbody.LinearVelocity = Vector3.Zero;
            _rigidbody.AngularVelocity = Vector3.Zero;
            Entity.Transform.Rotation = Quaternion.Identity;

            _animator.Play("Dead");
            while (_animator.IsPlaying)
                await Task.Delay(10);
        }

        public void MakeStep(Vector3 velocity)
        {
            if (!_initialized)
                return;

            _rigidbody.LinearVelocity = velocity * MovementSpeed;
            _rigidbody.AngularVelocity = Vector3.Zero;
            Entity.Transform.Rotation = Quaternion.Identity;
        }
    }
}
