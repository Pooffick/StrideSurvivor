using System;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using System.Collections.Generic;
using Stride.Physics;
using System.Linq;
using StrideSurvivor.Player;

namespace StrideSurvivor
{
    public class CrowdController : AsyncScript
    {
        private readonly List<SimpleEnemy> _simpleEnemies = new();
        private readonly Random _random = new(DateTime.Now.Second);
        private float _spawnTimer = 0;
        private float _deltatTime;
        private Vector3 _playerPosition;

        public Prefab SimpleEnemyPrefab;
        public PlayerController Player;
        public int SpawnTime = 10;
        public Int2 SpawnRange = new(1, 5);
        public Vector2 SpawnBox = new(10, 5);

        public static CrowdController Instance { get; private set; }

        public override async Task Execute()
        {
            Instance = this;

            while (Game.IsRunning && Player.CurrentState != PlayerState.Dead)
            {
                _deltatTime = (float)Game.UpdateTime.Elapsed.TotalSeconds;
                _playerPosition = Player.Entity.Transform.WorldMatrix.TranslationVector;

                if (_spawnTimer <= 0)
                {
                    Spawner();
                    _spawnTimer = SpawnTime;
                }
                else
                {
                    _spawnTimer -= _deltatTime;
                }

                Mover();

                await Script.NextFrame();
            }
        }

        public async void DestroyEnemy(SimpleEnemy enemy)
        {
            await enemy.Die();
            _simpleEnemies.Remove(enemy);
            Entity.RemoveChild(enemy.Entity);
        }

        private void Mover()
        {
            // TODO: maybe chunks better?
            _simpleEnemies.AsParallel().ForAll(enemy =>
            {
                if (enemy.Dying)
                    return;

                var direction = _playerPosition - enemy.Position;
                direction.Normalize();
                direction *= _deltatTime;

                enemy.MakeStep(direction);
            });
        }

        private void Spawner()
        {
            int count = _random.Next(SpawnRange.X, SpawnRange.Y);
            for (int i = 0; i <= count; i++)
            {
                var entities = SimpleEnemyPrefab.Instantiate();

                bool horizontal = _random.Next(2) == 0;
                float x = horizontal ? MathUtil.Lerp(-SpawnBox.X, SpawnBox.X, _random.NextSingle())
                    : (_random.Next(2) == 0 ? -SpawnBox.X : SpawnBox.X);
                float y = horizontal ? (_random.Next(2) == 0 ? -SpawnBox.Y : SpawnBox.Y)
                    : MathUtil.Lerp(-SpawnBox.Y, SpawnBox.Y, _random.NextSingle());

                var position = _playerPosition + new Vector3(x, y, 0);

                // Should be single
                foreach (Entity entity in entities)
                {
                    Entity.AddChild(entity);
                    entity.Transform.Position = position;
                    entity.Get<RigidbodyComponent>().UpdatePhysicsTransformation();
                    _simpleEnemies.Add(entity.Get<SimpleEnemy>());
                }
            }
        }
    }
}
