using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Components;
using MonoGameProject.Core;
using System.Collections.Generic;

namespace MonoGameProject.Entities
{
    /// <summary>
    /// Enemy - Open/Closed Principle
    /// OPEN voor uitbreiding: Ik kan nieuwe enemy types maken (NormalEnemy, ArmoredKnight, FlyingBoss)
    /// CLOSED voor modificatie: Ik hoef deze base class niet te wijzigen voor nieuwe enemies
    /// Voordeel: Nieuwe enemy toevoegen zonder bestaande code te breken
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

        // OCP: Abstract properties - subclasses implementeren hun eigen versie
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

        // OCP: Virtual method - subclasses kunnen override voor custom behavior
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
    }
}