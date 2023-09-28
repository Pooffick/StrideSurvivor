using Stride.Core.Mathematics;

namespace StrideSurvivor.Projectile
{
    public class PossibleTargets
    {
        public Vector3 MouseDirection { get; set; }
        public Vector3 ClosestEnemyDirection { get; set; }
        public Vector3 ClosestEnemyPosition { get; set; }
    }
}
