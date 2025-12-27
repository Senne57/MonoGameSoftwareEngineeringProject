using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Entities
{
    public class Platform
    {
        private Texture2D _texture;

        // VISUELE POSITIE
        public Vector2 Position;

        // COLLISION BOX (HANDMATIG)
        public Rectangle Bounds;

        public Platform(Texture2D texture, Vector2 position, int width, int height)
        {
            _texture = texture;
            Position = position;

            // HITBOX EXACT OP VISUELE PLAATS
            Bounds = new Rectangle(
                (int)position.X,
                (int)position.Y,
                width,
                height
            );
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Color.White);
        }
    }
}
