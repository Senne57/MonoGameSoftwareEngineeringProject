using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Components;
using MonoGameProject.Core;
using System.Collections.Generic;

namespace MonoGameProject.Entities
{
    /// <summary>
    /// Abstract base class for all enemy types
    /// Subclasses must implement their own hitboxes and behaviors
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

        // Subclasses define their own collision boxes
        public abstract Rectangle Bounds { get; }
        public abstract Rectangle HeadHitbox { get; }
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

            Velocity = new Vector2(_speed, Velocity.Y);

            // Simple patrol AI
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
                    Position = new Vector2(Position.X, p.Bounds.Top - GetGroundOffset());
                    Velocity = new Vector2(Velocity.X, 0);
                }
            }
        }

        protected abstract float GetGroundOffset();

        // Virtual - can be overridden for special damage handling (e.g. armor)
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
            if (_facingRight)
            {
                sb.Draw(TextureFactory.Pixel,
                    new Rectangle((int)Position.X + 30, (int)Position.Y + 30, 48, 5),
                    Color.Red);
                sb.Draw(TextureFactory.Pixel,
                    new Rectangle((int)Position.X + 30, (int)Position.Y + 30, (int)(48 * (HP / (float)MaxHP)), 5),
                    Color.Lime);
            }
            else
            {
                sb.Draw(TextureFactory.Pixel,
                    new Rectangle((int)Position.X + 50, (int)Position.Y + 30, 48, 5),
                    Color.Red);
                sb.Draw(TextureFactory.Pixel,
                    new Rectangle((int)Position.X + 50, (int)Position.Y + 30, (int)(48 * (HP / (float)MaxHP)), 5),
                    Color.Lime);
            }
        }

        protected virtual void DrawDebugHitboxes(SpriteBatch sb)
        {
            sb.Draw(TextureFactory.Pixel, Bounds, Color.Red * 0.3f);
            sb.Draw(TextureFactory.Pixel, HeadHitbox, Color.Blue * 0.5f);
        }
    }
}