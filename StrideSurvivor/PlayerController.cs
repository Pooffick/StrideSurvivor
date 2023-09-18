using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;

namespace StrideSurvivor.Player
{
    public class PlayerController : SyncScript
    {
        private SpriteAnimator _animator;

        public override void Start()
        {
            _animator = Entity.Get<SpriteAnimator>();
            _animator.Play("Idle");
        }

        public override void Update()
        {
            if (Input.IsKeyDown(Keys.W))
            {
                _animator.Play("Walk");
            }
            else
            {
                _animator.Play("Idle");
            }
        }
    }
}
