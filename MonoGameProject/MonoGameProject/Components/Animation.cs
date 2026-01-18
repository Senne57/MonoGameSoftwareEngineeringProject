using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Components
{
    /// <summary>
    /// Frame-based sprite animation
    /// Supports looping and freeze-frame options
    /// </summary>
    public class Animation
    {
        private Texture2D _texture;
        private int _frameCount;
        private float _frameTime;
        private int _currentFrame;
        private float _timer;
        private bool _loop;
        private bool _freezeLastFrame;

        public int CurrentFrame => _currentFrame;

        public Animation(Texture2D texture, int frameCount, float frameTime, bool loop = true, bool freezeLastFrame = false)
        {
            _texture = texture;
            _frameCount = frameCount;
            _frameTime = frameTime;
            _loop = loop;
            _freezeLastFrame = freezeLastFrame;
            _currentFrame = 0;
            _timer = 0f;
        }

        public void Update(GameTime gameTime)
        {
            if (_freezeLastFrame && _currentFrame == _frameCount - 1)
                return;

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer >= _frameTime)
            {
                _timer = 0f;
                _currentFrame++;

                if (_currentFrame >= _frameCount)
                {
                    if (_loop)
                        _currentFrame = 0;
                    else
                    {
                        if (_freezeLastFrame)
                            _currentFrame = _frameCount - 1;
                        else
                            _currentFrame = 0;
                    }
                }
            }
        }

        public void Reset()
        {
            _currentFrame = 0;
            _timer = 0f;
        }

        public void Draw(SpriteBatch sb, Vector2 position, SpriteEffects flip)
        {
            int frameWidth = _texture.Width / _frameCount;

            Rectangle sourceRect = new Rectangle(
                frameWidth * _currentFrame,
                0,
                frameWidth,
                _texture.Height
            );

            sb.Draw(_texture, position, sourceRect, Color.White, 0f, Vector2.Zero, 1f, flip, 0f);
        }
    }
}