using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Core
{
    /// <summary>
    /// SceneManager - Dependency Inversion Principle
    /// Hangt af van IScene ABSTRACTIE, niet van concrete scenes (LevelScene, StartScene)
    /// Voordeel: Nieuwe scenes toevoegen zonder SceneManager te wijzigen
    /// High-level module (SceneManager) hangt niet af van low-level modules (concrete scenes)
    /// </summary>
    public class SceneManager
    {
        private IScene _currentScene;

        public void ChangeScene(IScene scene)
        {
            _currentScene = scene;
        }

        public void Update(GameTime gameTime)
        {
            _currentScene?.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _currentScene?.Draw(spriteBatch);
        }
    }
}