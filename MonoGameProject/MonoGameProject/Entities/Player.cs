using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameProject.Components;
using MonoGameProject.Core;
using System.Collections.Generic;

namespace MonoGameProject.Entities
{
    public class Player
    {
        public Vector2 Position;
        public Vector2 Velocity;
        private Animation _idle, _run, _jump, _attack;
        private Animation _current;

        private const float Speed = 220f;
        private const float JumpForce = -520f;
        private const float Gravity = 1100f;

        private bool _isGrounded;
        private bool _isAttacking;
        private bool _facingRight = true;
        private float _attackTimer;
        private const float AttackDuration = 0.25f;

        private Vector2 _previousPosition;
        private HashSet<Enemy> _attackHitEnemies = new HashSet<Enemy>();

        public int MaxHP = 100;
        public int HP = 100;

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, 48, 64);
        public Rectangle PreviousBounds => new Rectangle((int)_previousPosition.X, (int)_previousPosition.Y, 48, 64);
        public HashSet<Enemy> AttackHitEnemies => _attackHitEnemies;

        public Player(Texture2D idle, Texture2D run, Texture2D jump, Texture2D attack, Vector2 start)
        {
            Position = start;
            _idle = new Animation(idle, 6, 0.15f);
            _run = new Animation(run, 8, 0.1f);
            _jump = new Animation(jump, 10, 0.08f);
            _attack = new Animation(attack, 4, 0.08f, false);
            _current = _idle;
        }

        public void Update(GameTime gameTime)
        {
            _previousPosition = Position;
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState k = Keyboard.GetState();

            Velocity.X = 0;

            if (k.IsKeyDown(Keys.Q)) { Velocity.X = -Speed; _facingRight = false; }
            if (k.IsKeyDown(Keys.D)) { Velocity.X = Speed; _facingRight = true; }
            if (k.IsKeyDown(Keys.Z) && _isGrounded) { Velocity.Y = JumpForce; _isGrounded = false; }

            if (k.IsKeyDown(Keys.E) && !_isAttacking)
            {
                _isAttacking = true;
                _attack.Reset();
                _attackTimer = AttackDuration;
                _attackHitEnemies.Clear();
            }

            if (_isAttacking)
            {
                _attack.Update(gameTime);
                _attackTimer -= dt;
                if (_attackTimer <= 0) _isAttacking = false;
            }

            Velocity.Y += Gravity * dt;
            Position += Velocity * dt;

            // Update animation
            if (_isAttacking) _current = _attack;
            else if (!_isGrounded) _current = _jump;
            else if (Velocity.X != 0) _current = _run;
            else _current = _idle;
        }

        public void HandlePlatformCollision(List<Platform> platforms)
        {
            _isGrounded = false;
            foreach (var p in platforms)
            {
                bool horizontal = Bounds.Right > p.Bounds.Left && Bounds.Left < p.Bounds.Right;
                bool landing = PreviousBounds.Bottom <= p.Bounds.Top && Bounds.Bottom >= p.Bounds.Top && Velocity.Y >= 0;
                if (horizontal && landing)
                {
                    Position.Y = p.Bounds.Top - Bounds.Height;
                    Velocity.Y = 0;
                    _isGrounded = true;
                }
            }
        }

        public Rectangle AttackHitbox
        {
            get
            {
                if (!_isAttacking) return Rectangle.Empty;
                if (_attack.CurrentFrame < 2) return Rectangle.Empty; // alleen frame 3-4 damage
                return new Rectangle(_facingRight ? Bounds.Right : Bounds.Left - 28, Bounds.Top + 12, 28, 40);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            SpriteEffects flip = _facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            _current.Draw(sb, Position, flip);

            // HP bar
            sb.Draw(TextureFactory.Pixel, new Rectangle((int)Position.X, (int)Position.Y - 10, 48, 5), Color.Red);
            sb.Draw(TextureFactory.Pixel, new Rectangle((int)Position.X, (int)Position.Y - 10, (int)(48 * (HP / (float)MaxHP)), 5), Color.Lime);
        }
    }
}
