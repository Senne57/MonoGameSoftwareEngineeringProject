using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Core
{
    /// <summary>
    /// Abstract base class voor alle backgrounds
    /// Volgt SOLID: Single Responsibility - alleen background rendering
    /// </summary>
    public abstract class Background
    {
        protected int _mapWidth;
        protected int _mapHeight;

        public Background(int mapWidth, int mapHeight)
        {
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
        }

        // Template method - subclasses implementeren hun eigen update logica
        public abstract void Update(GameTime gameTime);

        // Template method - subclasses implementeren hun eigen draw logica
        public abstract void Draw(SpriteBatch spriteBatch, Matrix? cameraTransform = null);
    }
}