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
        public int Damage = 25;
        public bool IsActive = true;

        private bool _isExploding = false;
        private int _currentFrame = 0;
        private float _animTimer = 0f;
        private const float FrameTime = 0.1f;
        private const int TotalFrames = 12; // Aantal frames in je sprite sheet

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

            if (_isExploding)
            {
                // Update explosie animatie
                _animTimer += dt;
                if (_animTimer >= FrameTime)
                {
                    _animTimer = 0f;
                    _currentFrame++;
                    if (_currentFrame >= TotalFrames) // Na laatste frame
                        IsActive = false;
                }
            }
            else
            {
                // Normale beweging
                Position += _direction * _speed * dt;

                // Deactiveer als buiten scherm
                if (Position.X < -100 || Position.X > 2000 || Position.Y < -100 || Position.Y > 700)
                    IsActive = false;
            }
        }

        public void Explode()
        {
            if (!_isExploding)
            {
                _isExploding = true;
                _currentFrame = 3;
                _animTimer = 5f;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (!IsActive) return;

            // Bereken source rectangle voor huidige frame
            int frameWidth = _texture.Width / TotalFrames;
            Rectangle sourceRect = new Rectangle(frameWidth * _currentFrame, 0, frameWidth, _texture.Height);

            sb.Draw(_texture, Position, sourceRect, Color.White);
            sb.Draw(TextureFactory.Pixel, Bounds, Color.Lime * 0.3f);
        }
    }
}