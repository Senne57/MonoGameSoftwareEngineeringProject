using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Entities
{
    /// <summary>
    /// Normale enemy waarop je kan springen om damage te doen
    /// Gebruikt: Enemy1Run.png en Enemy1Dead.png
    /// Volgt SOLID: Liskov Substitution Principle - kan overal gebruikt worden waar Enemy verwacht wordt
    /// </summary>
    public class NormalEnemy : Enemy
    {
        public NormalEnemy(Texture2D runTexture, Texture2D deathTexture, Vector2 startPos)
            : base(runTexture, deathTexture, startPos)
        {
            MaxHP = 50;
            HP = 50;
            _speed = 80f;
        }

        public override Rectangle Bounds
        {
            get
            {
                if (_facingRight)
                    return new Rectangle((int)Position.X + 30, (int)Position.Y + 65, 45, 70);
                else
                    return new Rectangle((int)Position.X + 53, (int)Position.Y + 65, 45, 70);
            }
        }

        public override Rectangle HeadHitbox
        {
            get
            {
                if (_facingRight)
                    return new Rectangle((int)Position.X + 50, (int)Position.Y + 52, 18, 15);
                else
                    return new Rectangle((int)Position.X + 62, (int)Position.Y + 52, 18, 15);
            }
        }

        protected override float GetGroundOffset()
        {
            return 95f;
        }

        // Deze enemy KAN gestomped worden
        public override bool CanBeStomped => true;

        // Wanneer je op zijn hoofd springt, neemt hij schade
        public void HandleStompDamage(int damage = 20)
        {
            TakeDamage(damage);
        }
    }
}