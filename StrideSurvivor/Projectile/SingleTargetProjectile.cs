using System.Linq;
using Stride.Core.Mathematics;
using Stride.Physics;

namespace StrideSurvivor.Projectile
{
    public class SingleTargetProjectile : BaseProjectile
    {
        public float Speed = 5f;

        public override void Update()
        {
            base.Update();

            float deltaTime = (float)Game.UpdateTime.Elapsed.TotalSeconds;
            _rigidbody.LinearVelocity = _parentTransform.WorldMatrix.Right * Speed * deltaTime;
        }

        protected override Vector3 GetTarget()
        {
            return Target.ClosestEnemyDirection;
        }

        protected override void CheckCollisions()
        {
            foreach (Collision collision in _rigidbody.Collisions)
            {
                var otherCollider = collision.ColliderA == _rigidbody ? collision.ColliderB : collision.ColliderA;

                if (otherCollider != null)
                {
                    var damageable = (IDamageable)otherCollider.Entity.Components.FirstOrDefault(x => x is IDamageable);
                    if (damageable != null)
                        damageable.TakeDamage(Damage);

                    Finish();
                }
            }
        }
    }
}
