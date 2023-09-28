using Stride.Input;
using Stride.Engine;
using Stride.Physics;
using Stride.Core.Mathematics;
using StrideSurvivor.Enemy;

namespace StrideSurvivor.Player
{
    public enum PlayerState
    {
        Alive,
        Hurt,
        Dead
    }

    public class PlayerController : SyncScript, IDamageable
    {
        private SpriteAnimator _animator;
        private CharacterComponent _character;

        public float MovementSpeed = 15f;
        public int HP = 10;

        public PlayerState CurrentState { get; private set; } = PlayerState.Alive;

        public override void Start()
        {
            _animator = Entity.Get<SpriteAnimator>();
            _character = Entity.Get<CharacterComponent>();

            _animator.Play("Idle");
        }

        public override void Update()
        {
            switch (CurrentState)
            {
                case PlayerState.Dead:
                    return;
                case PlayerState.Hurt:
                    if (!_animator.IsPlaying)
                        CurrentState = PlayerState.Alive;
                    break;
                default:
                    CheckCollisions();
                    break;
            }

            Move();
        }

        public void TakeDamage(int damage)
        {
            if (CurrentState != PlayerState.Alive)
                return;

            HP -= damage;
            if (HP <= 0)
            {
                CurrentState = PlayerState.Dead;
                _character.SetVelocity(Vector3.Zero);
                _animator.Play("Dead");
            }
            else
            {
                CurrentState = PlayerState.Hurt;
                _animator.Play("Hurt");
            }
        }

        private void Move()
        {
            float deltaTime = (float)Game.UpdateTime.Elapsed.TotalSeconds;
            var direction = new Vector3();

            // TODO: InputManager with correct bindings
            if (Input.IsKeyDown(Keys.W))
            {
                direction.Y = 1;
            }
            else
            if (Input.IsKeyDown(Keys.S))
            {
                direction.Y = -1;
            }

            if (Input.IsKeyDown(Keys.A))
            {
                direction.X = -1;
                Entity.Transform.Scale.X = -1; // Is there an easier way to flip the sprite?
            }
            else
            if (Input.IsKeyDown(Keys.D))
            {
                direction.X = 1;
                Entity.Transform.Scale.X = 1;
            }

            direction.Normalize();
            direction *= MovementSpeed * deltaTime;

            _character.SetVelocity(direction);

            if (CurrentState == PlayerState.Alive)
            {
                if (direction == Vector3.Zero)
                    _animator.Play("Idle");
                else
                    _animator.Play("Walk");
            }
        }

        private void CheckCollisions()
        {
            foreach (Collision collision in _character.Collisions)
            {
                var otherCollider = collision.ColliderA == _character ? collision.ColliderB : collision.ColliderA;

                var enemy = otherCollider.Entity.Get<BaseEnemy>();
                if (enemy != null && !enemy.Dying)
                {
                    CrowdController.Instance.DestroyEnemy(enemy);
                    TakeDamage(enemy.Damage);
                }
            }
        }
    }
}
