using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Core;

namespace MonoGameProject.Entities
{
    public class CannonBall : Entity
    {
        private Texture2D _texture;
        private float _speed = 200f;
        private Vector2 _direction;
        public int Damage = 25; // Was 15, nu 25
        public bool IsActive = true;

        public Rectangle Bounds =>
            new Rectangle((int)Position.X, (int)Position.Y, 16, 16);

        public CannonBall(Texture2D texture, Vector2 startPos, Vector2 direction)
        {
            _texture = texture;
            Position = startPos;
            _direction = direction;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += _direction * _speed * dt;

            // Deactiveer als buiten scherm (ver genoeg)
            if (Position.X < -100 || Position.X > 2000 || Position.Y < -100 || Position.Y > 700)
                IsActive = false;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (!IsActive) return;

            sb.Draw(_texture, Position, Color.White);

            // DEBUG: Teken hitbox (rood)
            sb.Draw(MonoGameProject.Core.TextureFactory.Pixel, Bounds, Color.Red * 0.3f);
        }
    }
}