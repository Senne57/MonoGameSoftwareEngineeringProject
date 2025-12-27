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
        private bool _facingRight = true;

        // Bounds - Moeten meebewegen met flip
        public Rectangle Bounds
        {
            get
            {
                if (_facingRight)
                    return new Rectangle((int)Position.X + 30, (int)Position.Y + 65, 45, 70);
                else
                    return new Rectangle((int)Position.X + 53, (int)Position.Y + 65, 45, 70); // Offset iets anders
            }
        }

        // HeadHitbox - Moet ook meebewegen met flip
        public Rectangle HeadHitbox
        {
            get
            {
                if (_facingRight)
                    return new Rectangle((int)Position.X + 50, (int)Position.Y + 52, 18, 15);
                else
                    return new Rectangle((int)Position.X + 62, (int)Position.Y + 52, 18, 15); // Offset iets anders
            }
        }

        public Enemy(Texture2D run, Texture2D dead, Vector2 startPos, bool armoredHead)
        {
            Position = startPos;
            _armoredHead = armoredHead;
            _run = new Animation(run, 8, 0.1f);
            _death = new Animation(dead, 3, 0.2f, loop: false, freezeLastFrame: true);
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
                    // FIX: Minder aftrekken zodat enemy lager staat
                    Position.Y = p.Bounds.Top - 95; // Was 135, nu 128
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

        public bool IsAlive => !_isDead;
        public bool CanBeStomped => !_armoredHead;

        public override void Draw(SpriteBatch sb)
        {
            var anim = _isDead ? _death : _run;

            SpriteEffects flip = _facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            anim.Draw(sb, Position, flip);

            // HP bar
            sb.Draw(TextureFactory.Pixel, new Rectangle((int)Position.X, (int)Position.Y - 8, 48, 5), Color.Red);
            sb.Draw(TextureFactory.Pixel,
                new Rectangle((int)Position.X, (int)Position.Y - 8, (int)(48 * (HP / (float)MaxHP)), 5),
                Color.Lime);

            // DEBUG: Teken hitboxes
            sb.Draw(TextureFactory.Pixel, Bounds, Color.Red * 0.3f);
            sb.Draw(TextureFactory.Pixel, HeadHitbox, Color.Blue * 0.5f);
        }
    }
}