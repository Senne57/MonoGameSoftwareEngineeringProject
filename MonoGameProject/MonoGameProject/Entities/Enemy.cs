using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Components;
using MonoGameProject.Core;
using System.Collections.Generic;

namespace MonoGameProject.Entities
{
    /// <summary>
    /// Abstract base class voor alle enemy types
    /// Volgt SOLID principes: Single Responsibility en Open/Closed
    /// </summary>
    public abstract class Enemy : Entity
    {
        protected Animation _run;
        protected Animation _death;
        protected bool _isDead;

        public int MaxHP { get; protected set; }
        public int HP { get; protected set; }

        protected const float Gravity = 1100f;
        protected float _speed = 80f;
        protected bool _facingRight = true;

        // Abstract properties - elke enemy type implementeert zijn eigen hitboxes
        public abstract Rectangle Bounds { get; }
        public abstract Rectangle HeadHitbox { get; }

        // Template method - bepaalt of deze enemy gestomped kan worden
        public abstract bool CanBeStomped { get; }

        protected Enemy(Texture2D runTexture, Texture2D deathTexture, Vector2 startPos)
        {
            Position = startPos;
            _run = new Animation(runTexture, 8, 0.1f);
            _death = new Animation(deathTexture, 3, 0.2f, loop: false, freezeLastFrame: true);
        }

        public override void Update(GameTime gameTime)
        {
            if (_isDead)
            {
                _death.Update(gameTime);
                return;
            }

            // ✅ FIX: Vervang hele Velocity property voor X component
            Velocity = new Vector2(_speed, Velocity.Y);

            // Patrol beweging
            if (Position.X < 100)
            {
                _speed = 80f;
                _facingRight = true;
            }
            if (Position.X > 700)
            {
                _speed = -80f;
                _facingRight = false;
            }

            // Gebruik Entity helper methods
            ApplyGravity(Gravity, gameTime);
            ApplyVelocity(gameTime);

            _run.Update(gameTime);
        }

        public void HandlePlatformCollision(List<Platform> platforms)
        {
            foreach (var p in platforms)
            {
                bool horizontal =
                    Bounds.Right > p.Bounds.Left &&
                    Bounds.Left < p.Bounds.Right;
                if (horizontal &&
                    Bounds.Bottom >= p.Bounds.Top &&
                    Velocity.Y >= 0)
                {
                    // ✅ FIX: Vervang hele Position en Velocity properties
                    Position = new Vector2(Position.X, p.Bounds.Top - GetGroundOffset());
                    Velocity = new Vector2(Velocity.X, 0);
                }
            }
        }

        // Template method - subclasses bepalen hun eigen ground offset
        protected abstract float GetGroundOffset();

        // Virtual method - kan worden override voor speciale damage handling
        public virtual void TakeDamage(int dmg)
        {
            if (_isDead) return;
            HP -= dmg;
            if (HP <= 0)
                _isDead = true;
        }

        public bool IsAlive => !_isDead;

        public override void Draw(SpriteBatch sb)
        {
            var anim = _isDead ? _death : _run;

            SpriteEffects flip = _facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            anim.Draw(sb, Position, flip);

            DrawHealthBar(sb);
            DrawDebugHitboxes(sb);
        }

        protected virtual void DrawHealthBar(SpriteBatch sb)
        {
            sb.Draw(TextureFactory.Pixel, new Rectangle((int)Position.X, (int)Position.Y - 8, 48, 5), Color.Red);
            sb.Draw(TextureFactory.Pixel,
                new Rectangle((int)Position.X, (int)Position.Y - 8, (int)(48 * (HP / (float)MaxHP)), 5),
                Color.Lime);
        }

        protected virtual void DrawDebugHitboxes(SpriteBatch sb)
        {
            sb.Draw(TextureFactory.Pixel, Bounds, Color.Red * 0.3f);
            sb.Draw(TextureFactory.Pixel, HeadHitbox, Color.Blue * 0.5f);
        }
    }
}