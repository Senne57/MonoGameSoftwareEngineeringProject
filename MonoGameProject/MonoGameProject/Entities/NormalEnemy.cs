using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Entities
{
    /// <summary>
    /// Standard enemy - can be stomped for damage
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
                    return new Rectangle((int)Position.X + 35, (int)Position.Y + 65, 35, 70);
                else
                    return new Rectangle((int)Position.X + 58, (int)Position.Y + 65, 35, 70);
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

        protected override float GetGroundOffset() => 95f;
        public override bool CanBeStomped => true;

        public void HandleStompDamage(int damage = 20)
        {
            TakeDamage(damage);
        }
    }
}