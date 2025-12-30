using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Core;

namespace MonoGameProject.Entities
{
    public class SpikeTrap : Entity
    {
        private Texture2D _texture;
        public int Damage = 30;
        private const int FixedFrame = 2; // Frame 3 (index 2) = omhoog

        public Rectangle Bounds
        {
            get
            {
                // Bereken frame grootte
                int frameWidth = _texture.Width / 7; // 7 frames in sprite sheet
                return new Rectangle((int)Position.X, (int)Position.Y + 25, frameWidth, _texture.Height - 25);
            }
        }

        public SpikeTrap(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;
        }

        public override void Update(GameTime gameTime)
        {
            // Spikes staan stil
        }

        public override void Draw(SpriteBatch sb)
        {
            // Teken alleen frame 3 (omhoog)
            int frameWidth = _texture.Width / 7; // 7 frames in sprite sheet
            Rectangle sourceRect = new Rectangle(frameWidth * FixedFrame, 0, frameWidth, _texture.Height);

            sb.Draw(_texture, Position, sourceRect, Color.White);
            sb.Draw(TextureFactory.Pixel, Bounds, Color.Lime * 0.3f);
        }
    }
}