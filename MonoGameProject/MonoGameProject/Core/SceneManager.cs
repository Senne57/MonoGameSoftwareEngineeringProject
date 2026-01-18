using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Core
{
    /// <summary>
    /// Manages scene transitions and current active scene
    /// Works with IScene interface for flexibility
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