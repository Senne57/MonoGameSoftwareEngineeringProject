using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Entities
{
    /// <summary>
    /// Armored Knight - je kan NIET op zijn hoofd springen door zijn armor
    /// Gebruikt: Walk.png (8 frames) en DeadKnight.png (6 frames)
    /// </summary>
    public class ArmoredKnight : Enemy
    {
        public ArmoredKnight(Texture2D runTexture, Texture2D deathTexture, Vector2 startPos)
            : base(runTexture, deathTexture, startPos)
        {
            MaxHP = 100;
            HP = 100;
            _speed = 60f;

            // ✅ FIX: Correcte frame counts
            _run = new MonoGameProject.Components.Animation(runTexture, 8, 0.1f);  // 8 frames
            _death = new MonoGameProject.Components.Animation(deathTexture, 6, 0.15f, loop: false, freezeLastFrame: true); // 6 frames
        }

        public override Rectangle Bounds
        {
            get
            {
                if (_facingRight)
                    return new Rectangle((int)Position.X + 5, (int)Position.Y + 60, 55, 75);
                else
                    return new Rectangle((int)Position.X + 65, (int)Position.Y + 60, 55, 75);
            }
        }

        public override Rectangle HeadHitbox
        {
            get
            {
                if (_facingRight)
                    return new Rectangle((int)Position.X + 20, (int)Position.Y + 55, 22, 18);
                else
                    return new Rectangle((int)Position.X + 81, (int)Position.Y + 55, 22, 18);
            }
        }

        protected override float GetGroundOffset()
        {
            return 100f;
        }

        public override bool CanBeStomped => false;

        public override void TakeDamage(int dmg)
        {
            if (_isDead) return;
            int reducedDamage = (int)(dmg * 0.7f);
            HP -= reducedDamage;
            if (HP <= 0)
                _isDead = true;
        }

        protected override void DrawHealthBar(SpriteBatch sb)
        {
            sb.Draw(MonoGameProject.Core.TextureFactory.Pixel,
                new Rectangle((int)Position.X, (int)Position.Y - 8, 48, 5), Color.DarkRed);
            sb.Draw(MonoGameProject.Core.TextureFactory.Pixel,
                new Rectangle((int)Position.X, (int)Position.Y - 8, (int)(48 * (HP / (float)MaxHP)), 5),
                Color.Orange);
        }
    }
}