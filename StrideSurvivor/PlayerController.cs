using Stride.Input;
using Stride.Engine;
using Stride.Physics;
using Stride.Core.Mathematics;

namespace StrideSurvivor.Player
{
    public enum PlayerState
    {
        Alive,
        Hurt,
        Dead
    }

    public class PlayerController : SyncScript
    {
        private TransformComponent _transform;
        private SpriteAnimator _animator;
        private CharacterComponent _character;
        private PlayerState _currentState = PlayerState.Alive;

        public float MovementSpeed = 15f;

        public override void Start()
        {
            _transform = Entity.Transform;
            _animator = Entity.Get<SpriteAnimator>();
            _character = Entity.Get<CharacterComponent>();

            _animator.Play("Idle");
        }

        public override void Update()
        {
            if (_currentState == PlayerState.Dead)
                return;

            if (_currentState == PlayerState.Hurt)
            {
                if (!_animator.IsPlaying)
                    _currentState = PlayerState.Alive;
            }

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
                _transform.Scale.X = -1; // Is there an easier way to flip the sprite?
            }
            else
            if (Input.IsKeyDown(Keys.D))
            {
                direction.X = 1;
                _transform.Scale.X = 1;
            }

            ////////
            if (Input.IsKeyDown(Keys.E))
            {
                _currentState = PlayerState.Dead;
                _character.SetVelocity(Vector3.Zero);
                _animator.Play("Dead");
            }

            if (Input.IsKeyDown(Keys.Q))
            {
                _currentState = PlayerState.Hurt;
                _animator.Play("Hurt");
            }
            ////////

            direction.Normalize();
            direction *= MovementSpeed * deltaTime;

            _character.SetVelocity(direction);

            if (_currentState == PlayerState.Alive)
            {
                if (direction == Vector3.Zero)
                    _animator.Play("Idle");
                else
                    _animator.Play("Walk");
            }
        }
    }
}
