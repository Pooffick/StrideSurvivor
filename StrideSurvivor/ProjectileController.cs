using System.Collections.Generic;
using SharpDX.MediaFoundation;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
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
                    Entity.AddChild(projectile.Entity.GetParent());
                }
            }
        }

        private void OnProjectileFinished(SimpleProjectile projectile)
        {
            Entity.RemoveChild(projectile.Entity.GetParent());
        }

        private Vector3 GetMousePosition()
        {
            var backBuffer = GraphicsDevice.Presenter.BackBuffer;
            var viewport = new Viewport(0, 0, backBuffer.Width, backBuffer.Height);
            return viewport.Unproject(new Vector3(Input.AbsoluteMousePosition, Camera.Entity.Transform.Position.Z), Camera.ProjectionMatrix, Camera.ViewMatrix, Matrix.Identity);
        }
    }
}
