using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Components;
using MonoGameProject.Core;

namespace MonoGameProject.Entities
{
    /// <summary>
    /// Flying Boss - Achtervolgt de speler met constante snelheid
    /// Kan alleen damage krijgen van cannonballs
    /// Gebruikt: FLYING.png, BOSSHURT.png (4 frames), BOSSDEATH.png (7 frames)
    /// </summary>
    public class FlyingBoss : Entity
    {
        private Animation _fly;
        private Animation _hurt;
        private Animation _death;

        private const float ConstantSpeed = 120f; // Constante snelheid naar rechts
        private float _verticalSpeed = 150f; // Voor up/down beweging naar player

        public int MaxHP { get; private set; }
        public int HP { get; private set; }

        private bool _isHurt;
        private float _hurtTimer;
        private const float HurtDuration = 0.4f; // Hurt animatie duurt 0.4 sec (4 frames * 0.1)

        private bool _isDead;
        private float _deathTimer;
        private const float DeathDuration = 2.5f; // ✅ 2.5 seconden (was 4.0)
        private bool _deathAnimationFinished;

        public bool IsAlive => !_isDead;
        public bool IsDeathAnimationComplete => _isDead && _deathTimer <= 0;

        // Boss bounds - instant kill zone
        public override Rectangle Bounds
        {
            get
            {
                // ✅ Jouw hitbox settings
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
            _fly = new Animation(flyTexture, 4, 0.12f, loop: true); // FLYING.png - 4 frames
            _hurt = new Animation(hurtTexture, 4, 0.1f, loop: false, freezeLastFrame: false); // BOSSHURT - 4 frames
            _death = new Animation(deathTexture, 7, 0.3f, loop: false, freezeLastFrame: true); // ✅ DEATH: loop=false, freeze op laatste frame

            Position = startPos;
            MaxHP = maxHP;
            HP = maxHP;
            _isHurt = false;
            _isDead = false;
            _deathAnimationFinished = false;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // ✅ DEATH ANIMATION - blijf updaten
            if (_isDead)
            {
                // Update animatie ALTIJD tijdens death (loop = true dus blijft spelen)
                _death.Update(gameTime);

                // Countdown timer
                _deathTimer -= dt;

                // ✅ DEBUG
                System.Diagnostics.Debug.WriteLine($"Boss Death: Timer={_deathTimer:F2}s, Frame={_death.CurrentFrame}");

                return; // Stop alle andere updates tijdens death
            }

            // ✅ HURT ANIMATION - speel af en countdown
            if (_isHurt)
            {
                _hurt.Update(gameTime);
                _hurtTimer -= dt;

                if (_hurtTimer <= 0)
                {
                    _isHurt = false;
                    _hurt.Reset();
                }

                // Boss blijft bewegen tijdens hurt
            }
            else
            {
                // Normal animation
                _fly.Update(gameTime);
            }

            // Boss beweegt ALTIJD naar rechts met constante snelheid
            Velocity = new Vector2(ConstantSpeed, Velocity.Y);
            ApplyVelocity(gameTime);
        }

        /// <summary>
        /// Boss volgt player verticaal (up/down) maar niet horizontaal
        /// </summary>
        public void FollowPlayerVertically(Vector2 playerPos, GameTime gameTime)
        {
            if (_isDead) return; // Stop volgen tijdens death

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Bereken verticale afstand tot player
            float targetY = playerPos.Y; // Center op player
            float deltaY = targetY - Position.Y;

            // Smooth vertical movement
            if (deltaY > 5)
                Velocity = new Vector2(Velocity.X, _verticalSpeed);
            else if (deltaY < -5)
                Velocity = new Vector2(Velocity.X, -_verticalSpeed);
            else
                Velocity = new Vector2(Velocity.X, 0);

            // Clamp Y binnen scherm grenzen
            if (Position.Y < 50)
                Position = new Vector2(Position.X, 50);
            if (Position.Y > 380)
                Position = new Vector2(Position.X, 380);
        }

        public void TakeDamage(int damage)
        {
            if (_isDead || _isHurt) return; // Kan geen damage nemen tijdens hurt of death

            HP -= damage;

            System.Diagnostics.Debug.WriteLine($"Boss took {damage} damage! HP: {HP}/{MaxHP}"); // ✅ DEBUG

            if (HP <= 0)
            {
                HP = 0;
                _isDead = true;
                _deathTimer = DeathDuration;
                _deathAnimationFinished = false; // ✅ Reset flag
                _death.Reset();

                System.Diagnostics.Debug.WriteLine("Boss DIED! Starting death animation..."); // ✅ DEBUG
            }
            else
            {
                // Trigger hurt animation
                _isHurt = true;
                _hurtTimer = HurtDuration;
                _hurt.Reset();
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            // ✅ Als death timer compleet is, teken NIETS (boss is opgelost!)
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

            // Debug hitbox (alleen tijdens leven)
            if (!_isDead)
                sb.Draw(TextureFactory.Pixel, Bounds, Color.Purple * 0.4f);
        }

        /// <summary>
        /// Reset boss naar start positie (voor respawn)
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
            _deathAnimationFinished = false; // ✅ Reset flag
            _fly.Reset();
            _hurt.Reset();
            _death.Reset();
        }
    }
}