using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Components;
using MonoGameProject.Core;
using System.Collections.Generic;

namespace MonoGameProject.Entities
{
    /// <summary>
    /// Flying boss for Level 3 - extends Enemy base class
    /// Flies horizontally and follows player vertically
    /// Can only be damaged by cannonballs, not player attacks
    /// </summary>
    public class FlyingBoss : Enemy
    {
        private Animation _hurt;
        private const float ConstantSpeed = 120f;
        private float _verticalSpeed = 150f;

        private bool _isHurt;
        private float _hurtTimer;
        private const float HurtDuration = 0.4f;

        private float _deathTimer;
        private const float DeathDuration = 2.5f;

        public bool IsDeathAnimationComplete => _isDead && _deathTimer <= 0;

        // Boss has no head hitbox - cannot be stomped
        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    64,
                    64
                );
            }
        }

        public override Rectangle HeadHitbox => Rectangle.Empty;
        public override bool CanBeStomped => false;
        protected override float GetGroundOffset() => 0f; // Boss flies, doesn't land

        public FlyingBoss(Texture2D flyTexture, Texture2D hurtTexture, Texture2D deathTexture, Vector2 startPos, int maxHP = 300)
            : base(flyTexture, deathTexture, startPos)
        {
            MaxHP = maxHP;
            HP = maxHP;

            // Override animations from base Enemy class
            _run = new Animation(flyTexture, 4, 0.12f, loop: true);
            _hurt = new Animation(hurtTexture, 4, 0.1f, loop: false, freezeLastFrame: false);
            _death = new Animation(deathTexture, 7, 0.3f, loop: false, freezeLastFrame: true);

            _speed = ConstantSpeed;
            _facingRight = true; // Boss always faces right
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_isDead)
            {
                _death.Update(gameTime);
                _deathTimer -= dt;
                return;
            }

            if (_isHurt)
            {
                _hurt.Update(gameTime);
                _hurtTimer -= dt;

                if (_hurtTimer <= 0)
                {
                    _isHurt = false;
                    _hurt.Reset();
                }
            }
            else
            {
                _run.Update(gameTime); // Use base class _run for fly animation
            }

            // Boss moves horizontally at constant speed
            Velocity = new Vector2(_speed, Velocity.Y);
            ApplyVelocity(gameTime);
        }

        /// <summary>
        /// Boss AI - follows player vertically only
        /// Horizontal movement is constant
        /// </summary>
        public void FollowPlayerVertically(Vector2 playerPos, GameTime gameTime)
        {
            if (_isDead) return;

            float targetY = playerPos.Y;
            float deltaY = targetY - Position.Y;

            if (deltaY > 5)
                Velocity = new Vector2(Velocity.X, _verticalSpeed);
            else if (deltaY < -5)
                Velocity = new Vector2(Velocity.X, -_verticalSpeed);
            else
                Velocity = new Vector2(Velocity.X, 0);

            // Keep boss within screen bounds
            if (Position.Y < 50)
                Position = new Vector2(Position.X, 50);
            if (Position.Y > 380)
                Position = new Vector2(Position.X, 380);
        }

        // Override TakeDamage to add hurt animation state
        public override void TakeDamage(int damage)
        {
            if (_isDead || _isHurt) return;

            HP -= damage;

            if (HP <= 0)
            {
                HP = 0;
                _isDead = true;
                _deathTimer = DeathDuration;
                _death.Reset();
            }
            else
            {
                _isHurt = true;
                _hurtTimer = HurtDuration;
                _hurt.Reset();
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            // Don't draw if death animation is complete
            if (_isDead && _deathTimer <= 0)
                return;

            Animation currentAnim;
            if (_isDead)
                currentAnim = _death;
            else if (_isHurt)
                currentAnim = _hurt;
            else
                currentAnim = _run; // Flying animation

            currentAnim.Draw(sb, Position, SpriteEffects.None);

        }

        // Boss doesn't use platform collision
        public new void HandlePlatformCollision(List<Platform> platforms)
        {
            // Boss flies - ignore platforms
        }

        /// <summary>
        /// Reset boss for respawn after player death
        /// </summary>
        public void Reset(Vector2 startPos)
        {
            Position = startPos;
            HP = MaxHP;
            Velocity = Vector2.Zero;
            _isDead = false;
            _isHurt = false;
            _hurtTimer = 0;
            _deathTimer = 0;
            _run.Reset();
            _hurt.Reset();
            _death.Reset();
        }

        // Override base class health bar - don't draw (boss has HP bar in UI)
        protected override void DrawHealthBar(SpriteBatch sb)
        {
            // Boss HP shown in UI instead of above sprite
        }
    }
}