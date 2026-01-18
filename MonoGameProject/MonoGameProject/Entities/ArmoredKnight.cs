using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Core;

namespace MonoGameProject.Entities
{
    /// <summary>
    /// Armored enemy - cannot be stomped, has damage reduction
    /// </summary>
    public class ArmoredKnight : Enemy
    {
        public ArmoredKnight(Texture2D runTexture, Texture2D deathTexture, Vector2 startPos)
            : base(runTexture, deathTexture, startPos)
        {
            MaxHP = 100;
            HP = 100;
            _speed = 60f;

            _run = new MonoGameProject.Components.Animation(runTexture, 8, 0.1f);
            _death = new MonoGameProject.Components.Animation(deathTexture, 6, 0.15f, loop: false, freezeLastFrame: true);
        }

        public override Rectangle Bounds
        {
            get
            {
                if (_facingRight)
                    return new Rectangle((int)Position.X + 15, (int)Position.Y + 75, 40, 55);
                else
                    return new Rectangle((int)Position.X + 75, (int)Position.Y + 75, 40, 55);
            }
        }

        public override Rectangle HeadHitbox
        {
            get
            {
                if (_facingRight)
                    return new Rectangle((int)Position.X + 26, (int)Position.Y + 65, 15, 10);
                else
                    return new Rectangle((int)Position.X + 86, (int)Position.Y + 65, 15, 10);
            }
        }

        protected override float GetGroundOffset() => 100f;
        public override bool CanBeStomped => false;

        // 30% damage reduction from armor
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
            if (_facingRight)
            {
                sb.Draw(TextureFactory.Pixel,
                    new Rectangle((int)Position.X + 10, (int)Position.Y + 45, 48, 5),
                    Color.DarkRed);
                sb.Draw(TextureFactory.Pixel,
                    new Rectangle((int)Position.X + 10, (int)Position.Y + 45, (int)(48 * (HP / (float)MaxHP)), 5),
                    Color.Orange);
            }
            else
            {
                sb.Draw(TextureFactory.Pixel,
                    new Rectangle((int)Position.X + 70, (int)Position.Y + 45, 48, 5),
                    Color.DarkRed);
                sb.Draw(TextureFactory.Pixel,
                    new Rectangle((int)Position.X + 70, (int)Position.Y + 45, (int)(48 * (HP / (float)MaxHP)), 5),
                    Color.Orange);
            }
        }
    }
}