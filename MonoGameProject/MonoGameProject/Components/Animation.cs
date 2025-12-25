using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Components
{
    public class Animation
    {
        private Texture2D _texture;
        private int _frameCount;
        private float _frameTime;
        private float _timer;
        private int _currentFrame;

        public int FrameWidth => _texture.Width / _frameCount;
        public int FrameHeight => _texture.Height;

        public Animation(Texture2D texture, int frameCount, float frameTime)
        {
            _texture = texture;
            _frameCount = frameCount;
            _frameTime = frameTime;
        }

        public void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer >= _frameTime)
            {
                _timer = 0f;
                _currentFrame = (_currentFrame + 1) % _frameCount;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Rectangle source = new Rectangle(
                _currentFrame * FrameWidth,
                0,
                FrameWidth,
                FrameHeight
            );

            spriteBatch.Draw(_texture, position, source, Color.White);
        }
    }
}
