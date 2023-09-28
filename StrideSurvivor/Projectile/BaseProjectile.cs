using System;
using System.Linq;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Physics;

namespace StrideSurvivor.Projectile
{
    public class BaseProjectile : SyncScript
    {
        protected bool _initialized = false;
        protected Vector3 _offsetPosition;
        protected Quaternion _offsetRotation;
        protected RigidbodyComponent _rigidbody;
        protected SpriteAnimator _animator;
        protected TransformComponent _parentTransform;

        public int Damage = 10;
        public float Cooldown = 2f;

        [DataMemberIgnore]
        public bool Active = false;
        [DataMemberIgnore]
        public PossibleTargets Target;

        public static Action<BaseProjectile> Finished;

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

            _parentTransform.Rotation = Quaternion.BetweenDirections(-Vector3.UnitX, GetTarget());
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

            CheckCollisions();

            if (!_animator.IsPlaying)
            {
                Finish();
            }
        }

        protected void Finish()
        {
            Active = false;
            _animator.Stop();
            Finished?.Invoke(this);
        }

        protected virtual Vector3 GetTarget()
        {
            return Target.MouseDirection;
        }

        protected virtual void CheckCollisions()
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
