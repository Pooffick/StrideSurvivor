﻿using System;
using System.Collections.Generic;
using Stride.Engine;
using Stride.Graphics;
using Stride.Core;
using System.Threading.Tasks;
using Stride.Rendering.Sprites;
using Stride.Core.Annotations;
using System.Threading;

namespace StrideSurvivor
{
    public class SpriteAnimator : StartupScript
    {
        private SpriteComponent _sprite;
        private string _currentAnimation;
        private CancellationTokenSource _cts = new();

        public Dictionary<string, SpriteAnimation> Animations { get; private set; } = new();

        [DataMemberRange(0.0, 10.0, 0.01, 1.0, 2)]
        public float AnimationSpeed = 1f;

        [DataMemberIgnore]
        public bool IsPlaying { get; private set; }

        public override void Start()
        {
            _sprite = Entity.Get<SpriteComponent>();
            _currentAnimation = null;
            IsPlaying = false;

            foreach (SpriteAnimation animation in Animations.Values)
            {
                if (animation.EndFrame == -1)
                    animation.EndFrame = animation.SpriteSheet.Sprites.Count - 1;
            }
        }

        public void Stop()
        {
            _cts.Cancel();
            _currentAnimation = null;
            IsPlaying = false;
        }

        public async void Play(string name)
        {
            if (!string.IsNullOrEmpty(_currentAnimation))
            {
                if (_currentAnimation.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return;

                _cts.Cancel();
            }

            _cts = new CancellationTokenSource();

            _currentAnimation = name;
            var animation = Animations[name];
            await PlayAnimation(animation, _cts.Token);
        }

        private async Task PlayAnimation(SpriteAnimation animation, CancellationToken token)
        {
            var sprite = _sprite.SpriteProvider as SpriteFromSheet;
            sprite.Sheet = animation.SpriteSheet;

            int delay = (int)(animation.Delay / AnimationSpeed);

            IsPlaying = true;
            do
            {
                for (int i = animation.StartFrame; i <= animation.EndFrame; i++)
                {
                    if (token.IsCancellationRequested)
                        break;

                    sprite.CurrentFrame = i;
                    await Task.Delay(delay);
                }
            } while (animation.Loop && !token.IsCancellationRequested);

            IsPlaying = animation.Loop; // Because I do not wait for the end of the old task, there is no need to change IsPlaying.
        }
    }

    [DataContract]
    public class SpriteAnimation
    {
        [DataMember(0)]
        public SpriteSheet SpriteSheet;
        [DataMember(1)]
        public int StartFrame = 0;
        [DataMember(2)]
        public int EndFrame = -1;
        [DataMember(3)]
        public int Delay = 100;
        [DataMember(4)]
        public bool Loop;
    }
}
