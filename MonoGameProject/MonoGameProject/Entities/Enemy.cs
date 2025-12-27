using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Components;
using MonoGameProject.Core;
using System.Collections.Generic;

namespace MonoGameProject.Entities
{
    public class Enemy : Entity
    {
        private Animation _run;
        private Animation _death;

        private bool _isDead;
        private bool _armoredHead;

        public int MaxHP = 50;
        public int HP = 50;

        private const float Gravity = 1100f;
        private float _speed = 80f;

        public Rectangle Bounds =>
            new Rectangle((int)Position.X + 6, (int)Position.Y + 10, 36, 54);

        // STRIKTE HEAD HITBOX (klein + exact)
        public Rectangle HeadHitbox =>
            new Rectangle(Bounds.Left + 6, Bounds.Top, Bounds.Width - 12, 12);

        public Enemy(Texture2D run, Texture2D dead, Vector2 startPos, bool armoredHead)
        {
            Position = startPos;
            _armoredHead = armoredHead;

            _run = new Animation(run, 8, 0.1f);
            _death = new Animation(dead, 3, 0.2f, loop: false, freezeLastFrame: true); // freeze laatste frame
        }


        public override void Update(GameTime gameTime)
        {
            if (_isDead)
            {
                _death.Update(gameTime);
                return;
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Velocity.X = _speed;

            if (Position.X < 100) _speed = 80f;
            if (Position.X > 700) _speed = -80f;

            Velocity.Y += Gravity * dt;
            Position += Velocity * dt;

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
                    Position.Y = p.Bounds.Top - Bounds.Height;
                    Velocity.Y = 0;
                }
            }
        }

        public void TakeDamage(int dmg)
        {
            if (_isDead) return;

            HP -= dmg;
            if (HP <= 0)
                _isDead = true;
        }

        // Voeg een property toe
        public bool IsAlive => !_isDead;


        public bool CanBeStomped => !_armoredHead;

        public override void Draw(SpriteBatch sb)
        {
            var anim = _isDead ? _death : _run;
            anim.Draw(sb, Position, SpriteEffects.None);

            sb.Draw(TextureFactory.Pixel, new Rectangle((int)Position.X, (int)Position.Y - 8, 48, 5), Color.Red);
            sb.Draw(TextureFactory.Pixel,
                new Rectangle((int)Position.X, (int)Position.Y - 8, (int)(48 * (HP / (float)MaxHP)), 5),
                Color.Lime);
        }
    }
}
