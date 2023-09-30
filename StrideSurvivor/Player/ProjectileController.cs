using System;
using System.Collections.Generic;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using StrideSurvivor.Enemy;
using StrideSurvivor.Projectile;

namespace StrideSurvivor.Player
{
    public class ProjectileController : SyncScript
    {
        private readonly Dictionary<BaseProjectile, float> _availableProjectiles = new();

        public Prefab DefaultProjectile;
        public Prefab SingleTargetProjectile;

        public CameraComponent Camera;
        public TransformComponent Player;

        public override void Start()
        {
            BaseProjectile.Finished += OnProjectileFinished;
            LevelManager.LevelChanged += OnPlayerLevelChanged;

            var entity = DefaultProjectile.Instantiate()[0];
            var projectile = entity.GetChild(0).Get<BaseProjectile>(); // TODO: change hierarchy
            _availableProjectiles.Add(projectile, projectile.Cooldown);
        }

        public override void Update()
        {
            Entity.Transform.Position = Player.WorldMatrix.TranslationVector;

            float time = (float)Game.UpdateTime.Total.TotalSeconds;

            CastSimpleProjectile(time);
        }

        private void CastSimpleProjectile(float time)
        {
            var mousePosition = GetMousePositionRelativeToScreenCenter();
            var mouseDirection = new Vector3(mousePosition, 0);
            mouseDirection.Normalize();

            var enemyPosition = GetNearestEnemyPosition();
            var enemyDirection = Player.WorldMatrix.TranslationVector - enemyPosition;
            enemyDirection.Normalize();

            var possibleTargets = new PossibleTargets
            {
                MouseDirection = mouseDirection,
                ClosestEnemyDirection = enemyDirection,
                ClosestEnemyPosition = enemyPosition
            };

            foreach (BaseProjectile projectile in _availableProjectiles.Keys)
            {
                if (projectile.Active)
                    continue;

                var lastTime = _availableProjectiles[projectile];
                if (lastTime <= time)
                {
                    projectile.Target = possibleTargets;
                    _availableProjectiles[projectile] += projectile.Cooldown;
                    var parentEntity = projectile.Entity.GetParent();
                    parentEntity.Transform.Position = Entity.Transform.Position;
                    Entity.Scene.Entities.Add(parentEntity);
                }
            }
        }

        private void OnProjectileFinished(BaseProjectile projectile)
        {
            Entity.Scene.Entities.Remove(projectile.Entity.GetParent());
        }

        private void OnPlayerLevelChanged()
        {
            // TODO: show screen with selection
            var entity = SingleTargetProjectile.Instantiate()[0];
            var projectile = entity.GetChild(0).Get<BaseProjectile>(); // TODO: change hierarchy
            _availableProjectiles.Add(projectile, projectile.Cooldown);
        }

        private Vector2 GetMousePositionRelativeToScreenCenter()
        {
            var backBuffer = GraphicsDevice.Presenter.BackBuffer;
            return new Vector2(backBuffer.Width / 2f - Input.AbsoluteMousePosition.X,
                Input.AbsoluteMousePosition.Y - backBuffer.Height / 2f);
        }

        private Vector3 GetNearestEnemyPosition()
        {
            float nearestDistance = float.MaxValue;
            BaseEnemy nearest = null;
            foreach (BaseEnemy enemy in CrowdController.Instance.Enemies)
            {
                var distance = Vector3.Distance(Player.WorldMatrix.TranslationVector, enemy.Position);
                if (nearest == null || distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = enemy;
                }
            }

            return nearest.Position;
        }
    }
}
