using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameProject.Components;
using MonoGameProject.Core;
using System;
using System.Collections.Generic;

namespace MonoGameProject.Entities
{
    /// <summary>
    /// Player character - extends Entity base class
    /// Handles player input, animations, and state management
    /// </summary>
    public class Player : Entity
    {
        private Animation _idle, _run, _jump, _attack;
        private Animation _current;

        private const float Speed = 220f;
        private const float JumpForce = -520f;
        private const float Gravity = 1100f;
        private const float AttackDuration = 0.25f;
        private const float InvincibilityDuration = 1.5f;

        private bool _isGrounded;
        private bool _isAttacking;
        private bool _facingRight = true;
        private float _attackTimer;
        private Vector2 _previousPosition;
        private HashSet<Enemy> _attackHitEnemies = new HashSet<Enemy>();

        public int MaxHP = 100;
        public int HP = 100;
        public int Lives = 3;
        public bool IsInvincible { get; private set; }
        private float _invincibilityTimer;
        private float _flickerTimer;

        public override Rectangle Bounds =>
            new Rectangle((int)Position.X + 48, (int)Position.Y + 45, 29, 85);

        public Rectangle PreviousBounds =>
            new Rectangle((int)_previousPosition.X + 40, (int)_previousPosition.Y + 45, 45, 85);

        public Rectangle PlatformPreviousBounds =>
            new Rectangle((int)_previousPosition.X, (int)_previousPosition.Y, 48, 64);

        public HashSet<Enemy> AttackHitEnemies => _attackHitEnemies;

        public void SetGrounded(bool grounded)
        {
            _isGrounded = grounded;
        }

        public Player(Texture2D idle, Texture2D run, Texture2D jump, Texture2D attack, Vector2 start)
        {
            Position = start;
            _idle = new Animation(idle, 6, 0.15f);
            _run = new Animation(run, 8, 0.1f);
            _jump = new Animation(jump, 10, 0.08f);
            _attack = new Animation(attack, 4, 0.08f, false);
            _current = _idle;
        }

        public override void Update(GameTime gameTime)
        {
            if (HP <= 0) return;

            _previousPosition = Position;
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle invincibility frames
            if (IsInvincible)
            {
                _invincibilityTimer -= dt;
                _flickerTimer += dt;
                if (_invincibilityTimer <= 0)
                    IsInvincible = false;
            }

            KeyboardState k = Keyboard.GetState();

            // Movement input - supports both QWERTY and arrow keys
            float velocityX = 0;
            if (k.IsKeyDown(Keys.Q) || k.IsKeyDown(Keys.Left))
            {
                velocityX = -Speed;
                _facingRight = false;
            }
            if (k.IsKeyDown(Keys.D) || k.IsKeyDown(Keys.Right))
            {
                velocityX = Speed;
                _facingRight = true;
            }
            Velocity = new Vector2(velocityX, Velocity.Y);

            // Jump input - only when grounded
            if ((k.IsKeyDown(Keys.Z) || k.IsKeyDown(Keys.Up)) && _isGrounded)
            {
                Velocity = new Vector2(Velocity.X, JumpForce);
                _isGrounded = false;
            }

            // Attack input
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
                if (_attackTimer <= 0)
                    _isAttacking = false;
            }

            ApplyGravity(Gravity, gameTime);
            ApplyVelocity(gameTime);

            // Animation state selection
            if (_isAttacking)
                _current = _attack;
            else if (!_isGrounded)
                _current = _jump;
            else if (Velocity.X != 0)
                _current = _run;
            else
                _current = _idle;

            _current.Update(gameTime);
        }

        public void HandlePlatformCollision(List<Platform> platforms)
        {
            // Deprecated - use CollisionManager.CheckPlayerPlatformCollisions() instead
            _isGrounded = false;
            foreach (var p in platforms)
            {
                bool horizontal = Bounds.Right > p.Bounds.Left && Bounds.Left < p.Bounds.Right;
                bool landing = PlatformPreviousBounds.Bottom <= p.Bounds.Top &&
                              Bounds.Bottom >= p.Bounds.Top && Velocity.Y >= 0;
                if (horizontal && landing)
                {
                    Position = new Vector2(Position.X, p.Bounds.Top - 90);
                    Velocity = new Vector2(Velocity.X, 0);
                    _isGrounded = true;
                }
            }
        }

        public void TakeDamage(int damage)
        {
            if (IsInvincible || HP <= 0) return;

            HP -= damage;
            if (HP <= 0)
            {
                HP = 0;
                Lives--;
            }
            else
            {
                IsInvincible = true;
                _invincibilityTimer = InvincibilityDuration;
                _flickerTimer = 0;
            }
        }

        public void Respawn(Vector2 spawnPoint)
        {
            Position = spawnPoint;
            HP = MaxHP;
            Velocity = Vector2.Zero;
            IsInvincible = true;
            _invincibilityTimer = InvincibilityDuration;
            _flickerTimer = 0;
        }

        // Attack hitbox is only active during specific frames of attack animation
        public Rectangle AttackHitbox
        {
            get
            {
                if (!_isAttacking) return Rectangle.Empty;
                if (_attack.CurrentFrame < 2) return Rectangle.Empty;

                return new Rectangle(
                    _facingRight ? Bounds.Right : Bounds.Left - 28,
                    Bounds.Top + 12,
                    28,
                    40
                );
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            // Flicker effect during invincibility
            if (IsInvincible && (int)(_flickerTimer * 10) % 2 == 0)
                return;

            SpriteEffects flip = _facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            _current.Draw(sb, Position, flip);

            // Health bar
            sb.Draw(TextureFactory.Pixel,
                new Rectangle((int)Position.X + 40, (int)Position.Y + 25, 48, 5),
                Color.Red);
            sb.Draw(TextureFactory.Pixel,
                new Rectangle((int)Position.X + 40, (int)Position.Y + 25, (int)(48 * (HP / (float)MaxHP)), 5),
                Color.Lime);
        }
    }
}