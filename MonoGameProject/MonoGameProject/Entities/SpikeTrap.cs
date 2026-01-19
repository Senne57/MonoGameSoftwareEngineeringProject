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

        // ✅ Override Bounds from Entity base class
        public override Rectangle Bounds
        {
            get
            {
                int frameWidth = _texture.Width / 7; // 7 frames in sprite sheet
                return new Rectangle((int)Position.X + 15, (int)Position.Y + 35, frameWidth - 30, _texture.Height - 25);
            }
        }

        public SpikeTrap(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;
        }

        public override void Update(GameTime gameTime)
        {
            // Spikes staan stil - geen update nodig
        }

        public override void Draw(SpriteBatch sb)
        {
            // Teken alleen frame 3 (omhoog)
            int frameWidth = _texture.Width / 7;
            Rectangle sourceRect = new Rectangle(frameWidth * FixedFrame, 0, frameWidth, _texture.Height);

            sb.Draw(_texture, Position, sourceRect, Color.White);
        }
    }
}