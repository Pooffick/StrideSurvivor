using System;
using System.Collections.Generic;
using SharpDX.MediaFoundation;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Input;
using Stride.Physics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace StrideSurvivor.Player
{
    public class ProjectileController : SyncScript
    {
        private readonly Dictionary<SimpleProjectile, float> _availableProjectiles = new();

        public Prefab DefaultProjectile;

        public CameraComponent Camera;
        public TransformComponent Target;

        public override void Start()
        {
            SimpleProjectile.Finished += OnProjectileFinished;

            var entity = DefaultProjectile.Instantiate()[0];
            var projectile = entity.GetChild(0).Get<SimpleProjectile>(); // TODO: change hierarchy
            _availableProjectiles.Add(projectile, projectile.Cooldown);
        }

        public override void Update()
        {
            Entity.Transform.Position = Target.WorldMatrix.TranslationVector;

            float time = (float)Game.UpdateTime.Total.TotalSeconds;

            CastSimpleProjectile(time);
        }

        private void CastSimpleProjectile(float time)
        {
            var mousePosition = GetMousePosition();
            foreach (SimpleProjectile projectile in _availableProjectiles.Keys)
            {
                if (projectile.Active)
                    continue;

                var lastTime = _availableProjectiles[projectile];
                if (lastTime <= time)
                {
                    projectile.Target = mousePosition;
                    _availableProjectiles[projectile] += projectile.Cooldown;
                    var parentEntity = projectile.Entity.GetParent();
                    parentEntity.Transform.Position = Entity.Transform.Position;
                    Entity.Scene.Entities.Add(parentEntity);
                }
            }
        }

        private void OnProjectileFinished(SimpleProjectile projectile)
        {
            Entity.Scene.Entities.Remove(projectile.Entity.GetParent());
        }

        private Vector2 GetMousePosition()
        {
            var backBuffer = GraphicsDevice.Presenter.BackBuffer;
            return new Vector2(backBuffer.Width / 2f - Input.AbsoluteMousePosition.X,
                Input.AbsoluteMousePosition.Y - backBuffer.Height / 2f);
        }
    }
}
