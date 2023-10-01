using System;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Engine;
using System.Collections.Generic;
using Stride.Physics;
using System.Linq;
using StrideSurvivor.Player;
using Stride.Core;

namespace StrideSurvivor.Enemy
{
    public class CrowdController : AsyncScript
    {
        private readonly List<BaseEnemy> _enemies = new();
        private readonly List<TransformComponent> _experiencePoints = new();
        private readonly Random _random = new(DateTime.Now.Second);
        private float _spawnTimer = 0;
        private float _deltatTime;
        private Vector3 _playerPosition;

        public Prefab SimpleEnemyPrefab;
        public Prefab ExperiencePrefab;

        public PlayerController Player;
        public int SpawnTime = 10;
        public Int2 SpawnRange = new(1, 5);
        public Vector2 SpawnBox = new(10, 5);
        public float ExperienceCollectSpeed = 10f;

        [DataMemberIgnore]
        public IList<BaseEnemy> Enemies => _enemies;

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
                ExperienceCollector();

                await Script.NextFrame();
            }
        }

        public async void DestroyEnemy(BaseEnemy enemy, bool withExperience = true)
        {
            await enemy.Die();
            _enemies.Remove(enemy);
            Entity.RemoveChild(enemy.Entity);

            if (withExperience)
                SpawnExperience(enemy.Position, enemy.Experience);
        }

        private void SpawnExperience(Vector3 centerPosition, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var entity = ExperiencePrefab.Instantiate()[0]; // Should be single
                entity.Transform.Position = centerPosition + new Vector3((float)_random.NextDouble() / 2f, (float)_random.NextDouble() / 2f, -0.1f);
                Entity.AddChild(entity);
                _experiencePoints.Add(entity.Transform);
            }
        }

        private void ExperienceCollector()
        {
            // TODO: maybe chunks better?
            _experiencePoints.ToArray().AsParallel().ForAll(experience =>
            {
                var position = experience.Position;
                float distanceToPlayer = Vector3.Distance(position, _playerPosition);
                if (distanceToPlayer > Player.ExperienceCollectDistance)
                    return;

                var direction = _playerPosition - position;
                direction.Normalize();
                direction *= _deltatTime * ExperienceCollectSpeed;

                position += direction;

                experience.Position = position;

                if (Vector3.Distance(position, _playerPosition) <= 0.1f)
                {
                    _experiencePoints.Remove(experience);
                    Entity.RemoveChild(experience.Entity);
                    Player.LevelManager.AddExperience();
                }
            });
        }

        private void Mover()
        {
            // TODO: maybe chunks better?
            _enemies.AsParallel().ForAll(enemy =>
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
                var entity = SimpleEnemyPrefab.Instantiate()[0]; // Should be single

                bool horizontal = _random.Next(2) == 0;
                float x = horizontal ? MathUtil.Lerp(-SpawnBox.X, SpawnBox.X, _random.NextSingle())
                    : (_random.Next(2) == 0 ? -SpawnBox.X : SpawnBox.X);
                float y = horizontal ? (_random.Next(2) == 0 ? -SpawnBox.Y : SpawnBox.Y)
                    : MathUtil.Lerp(-SpawnBox.Y, SpawnBox.Y, _random.NextSingle());

                var position = _playerPosition + new Vector3(x, y, 0);

                Entity.AddChild(entity);
                entity.Transform.Position = position;
                entity.Get<RigidbodyComponent>().UpdatePhysicsTransformation();
                _enemies.Add(entity.Get<BaseEnemy>());
            }
        }
    }
}
