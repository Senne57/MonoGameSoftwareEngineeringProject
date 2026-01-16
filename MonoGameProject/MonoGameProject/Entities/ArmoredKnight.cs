using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Entities
{
    /// <summary>
    /// Armored Knight - je kan NIET op zijn hoofd springen door zijn armor
    /// Gebruikt: Walk.png (run) en DeadKnight.png (death)
    /// Volgt SOLID: Open/Closed Principle - extends Enemy zonder base class te wijzigen
    /// </summary>
    public class ArmoredKnight : Enemy
    {
        public ArmoredKnight(Texture2D runTexture, Texture2D deathTexture, Vector2 startPos)
            : base(runTexture, deathTexture, startPos)
        {
            MaxHP = 100; // Sterker dan normale enemy
            HP = 100;
            _speed = 60f; // Iets langzamer door zware armor
        }

        public override Rectangle Bounds
        {
            get
            {
                // Iets groter dan normale enemy door armor
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
                // Helm hitbox - maar deze is beschermd!
                if (_facingRight)
                    return new Rectangle((int)Position.X + 20, (int)Position.Y + 55, 22, 18);
                else
                    return new Rectangle((int)Position.X + 81, (int)Position.Y + 55, 22, 18);
            }
        }

        protected override float GetGroundOffset()
        {
            return 100f; // Iets hoger omdat hij groter is
        }

        // Deze enemy kan NIET gestomped worden - heeft armor op zijn hoofd
        public override bool CanBeStomped => false;

        // Override TakeDamage om armor damage reductie toe te voegen
        // Volgt SOLID: Open/Closed - extends gedrag zonder base te wijzigen
        public override void TakeDamage(int dmg)
        {
            if (_isDead) return;

            // Armor absorbeert 30% van de damage
            int reducedDamage = (int)(dmg * 0.7f);
            HP -= reducedDamage;

            if (HP <= 0)
                _isDead = true;
        }

        // Override health bar kleur voor visual feedback dat dit een armored enemy is
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