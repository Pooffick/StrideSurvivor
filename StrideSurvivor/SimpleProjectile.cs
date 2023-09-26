using System;
using System.Linq;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace StrideSurvivor
{
    public class SimpleProjectile : SyncScript
    {
        private bool _initialized = false;
        private Vector3 _offsetPosition;
        private Quaternion _offsetRotation;
        private RigidbodyComponent _rigidbody;
        private SpriteAnimator _animator;
        private TransformComponent _parentTransform;

        public int Damage = 10;
        public float Cooldown = 2f;
        public bool Active = false;

        [DataMemberIgnore]
        public Vector2 Target;

        public static Action<SimpleProjectile> Finished;

        public override void Start()
        {
            if (!_initialized)
            {
                _offsetPosition = Entity.Transform.Position;
                _offsetRotation = Entity.Transform.Rotation;
                _rigidbody = Entity.Get<RigidbodyComponent>();
                _parentTransform = Entity.GetParent().Transform;
                _initialized = true;
            }

            var direction = new Vector3(Target, 0f);
            direction.Normalize();
            _parentTransform.Rotation = Quaternion.BetweenDirections(-Vector3.UnitX, direction);
            Entity.Transform.Rotation = _offsetRotation;
            Entity.Transform.Position = _offsetPosition;
            _rigidbody.UpdatePhysicsTransformation();

            _animator = Entity.Get<SpriteAnimator>();
            _animator.Play("Charge");
            Active = true;
        }

        public override void Update()
        {
            if (!Active)
                return;

            DebugText.Print(Entity.Transform.Position.ToString(), new Int2(200, 20), Color.Wheat);

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
