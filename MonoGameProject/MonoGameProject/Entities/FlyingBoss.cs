using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Components;
using MonoGameProject.Core;

namespace MonoGameProject.Entities
{
    /// <summary>
    /// Level 3 boss - flies horizontally and follows player vertically
    /// Can only be damaged by cannonballs
    /// </summary>
    public class FlyingBoss : Entity
    {
        private Animation _fly;
        private Animation _hurt;
        private Animation _death;

        private const float ConstantSpeed = 120f;
        private float _verticalSpeed = 150f;

        public int MaxHP { get; private set; }
        public int HP { get; private set; }

        private bool _isHurt;
        private float _hurtTimer;
        private const float HurtDuration = 0.4f;

        private bool _isDead;
        private float _deathTimer;
        private const float DeathDuration = 2.5f;

        public bool IsAlive => !_isDead;
        public bool IsDeathAnimationComplete => _isDead && _deathTimer <= 0;

        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)Position.X - 5,
                    (int)Position.Y - 5,
                    80,
                    80
                );
            }
        }

        public FlyingBoss(Texture2D flyTexture, Texture2D hurtTexture, Texture2D deathTexture, Vector2 startPos, int maxHP = 300)
        {
            _fly = new Animation(flyTexture, 4, 0.12f, loop: true);
            _hurt = new Animation(hurtTexture, 4, 0.1f, loop: false, freezeLastFrame: false);
            _death = new Animation(deathTexture, 7, 0.3f, loop: false, freezeLastFrame: true);

            Position = startPos;
            MaxHP = maxHP;
            HP = maxHP;
            _isHurt = false;
            _isDead = false;
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
                _fly.Update(gameTime);
            }

            Velocity = new Vector2(ConstantSpeed, Velocity.Y);
            ApplyVelocity(gameTime);
        }

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

        public void TakeDamage(int damage)
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
            if (_isDead && _deathTimer <= 0)
                return;

            Animation currentAnim;
            if (_isDead)
                currentAnim = _death;
            else if (_isHurt)
                currentAnim = _hurt;
            else
                currentAnim = _fly;

            currentAnim.Draw(sb, Position, SpriteEffects.None);

            if (!_isDead)
                sb.Draw(TextureFactory.Pixel, Bounds, Color.Purple * 0.4f);
        }

        public void Reset(Vector2 startPos)
        {
            Position = startPos;
            HP = MaxHP;
            Velocity = Vector2.Zero;
            _isDead = false;
            _isHurt = false;
            _hurtTimer = 0;
            _deathTimer = 0;
            _fly.Reset();
            _hurt.Reset();
            _death.Reset();
        }
    }
}