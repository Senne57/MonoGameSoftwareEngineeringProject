using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Core
{
    /// <summary>
    /// Animated background voor menu screens (Start, GameOver, Victory)
    /// Gebruikt: Start.png (36 frames 6x6), GameOver.png (36 frames 6x6), Victory.png (36 frames 6x6)
    /// </summary>
    public class AnimatedBackground : Background
    {
        private Texture2D _texture;
        private int _currentFrame;
        private float _timer;
        private const float FrameTime = 0.1f;
        private const int TotalFrames = 36;
        private const int FramesPerRow = 6;
        private float _scale; // ✅ Scale factor

        // ✅ Constructor met optionele scale parameter (default = 1.0 = volledig scherm)
        public AnimatedBackground(Texture2D texture, int frameCount, int mapWidth, int mapHeight, float scale = 1.0f)
            : base(mapWidth, mapHeight)
        {
            _texture = texture;
            _currentFrame = 0;
            _timer = 0f;
            _scale = scale;
        }

        public override void Update(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer >= FrameTime)
            {
                _timer -= FrameTime;
                _currentFrame++;

                if (_currentFrame >= TotalFrames)
                    _currentFrame = 0;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix? cameraTransform = null)
        {
            // Bereken frame positie in 6x6 grid
            int frameWidth = _texture.Width / FramesPerRow;
            int frameHeight = _texture.Height / FramesPerRow;

            int row = _currentFrame / FramesPerRow;
            int col = _currentFrame % FramesPerRow;

            Rectangle sourceRect = new Rectangle(
                col * frameWidth,
                row * frameHeight,
                frameWidth,
                frameHeight
            );

            // ✅ Bereken geschaalde afmetingen
            int scaledWidth = (int)(800 * _scale);
            int scaledHeight = (int)(480 * _scale);

            // ✅ Centreer de geschaalde background
            int offsetX = (800 - scaledWidth) / 2;
            int offsetY = (480 - scaledHeight) / 2;

            // Teken geschaalde en gecentreerde background
            spriteBatch.Draw(
                _texture,
                new Rectangle(offsetX, offsetY, scaledWidth, scaledHeight),
                sourceRect,
                Color.White
            );
        }
    }
}