using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Core;

namespace MonoGameProject.Entities
{
    public class SpikeTrap : Entity
    {
        private Texture2D _texture;
        public int Damage = 20;

        public Rectangle Bounds =>
            new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);

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
            sb.Draw(_texture, Position, Color.White);
        }
    }
}