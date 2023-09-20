using System;
using System.Linq;
using System.Threading.Tasks;
using BulletSharp;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Physics;

namespace StrideSurvivor
{
    public class SimpleProjectile : SyncScript
    {
        private static readonly Vector3 Up = new(0, 0, 1);
        private RigidbodyComponent _rigidbody;
        private SpriteAnimator _animator;
        private TransformComponent _parentTransform;

        public int Damage = 10;
        public float Cooldown = 2f;
        public bool Active = false;

        [DataMemberIgnore]
        public Vector3 Target;

        public static Action<SimpleProjectile> Finished;

        public override void Start()
        {
            _rigidbody = Entity.Get<RigidbodyComponent>();
            _parentTransform = Entity.GetParent().Transform;

            var direction = Target - _parentTransform.WorldMatrix.TranslationVector;
            direction.Normalize();
            DebugText.Print(direction.ToString(), new Int2(200, 50), Color.Red, TimeSpan.FromMilliseconds(100)); // TODO!!!
            _parentTransform.Rotation = Quaternion.LookRotation(direction, Up);
            _rigidbody.UpdatePhysicsTransformation();

            _animator = Entity.Get<SpriteAnimator>();
            _animator.Play("Charge");
            Active = true;
        }

        public override void Update()
        {
            if (!Active)
                return;

            _parentTransform.Position = Vector3.Zero;
            _rigidbody.UpdatePhysicsTransformation();

            CheckCollisions();

            if (!_animator.IsPlaying)
            {
                Active = false;
                _animator.Stop();
                Finished?.Invoke(this);
            }
        }

        private void CheckCollisions()
        {
            foreach (Collision collision in _rigidbody.Collisions)
            {
                var otherCollider = collision.ColliderA == _rigidbody ? collision.ColliderB : collision.ColliderA;

                if (otherCollider != null)
                {
                    var damageable = (IDamageable)otherCollider.Entity.Components.FirstOrDefault(x => x is IDamageable);
                    if (damageable != null)
                        damageable.TakeDamage(Damage);
                }
            }
        }
    }
}
