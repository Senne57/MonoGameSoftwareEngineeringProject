using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Core
{
    /// <summary>
    /// IScene - Interface Segregation Principle
    /// Kleine, gefocuste interface met alleen Update en Draw
    /// Scenes implementeren ALLEEN wat ze nodig hebben - geen onnodige methods
    /// Voordeel: Geen bloated interface met methods die scenes niet gebruiken
    /// </summary>
    public interface IScene
    {
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
