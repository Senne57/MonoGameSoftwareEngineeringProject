using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameProject.Core;

namespace MonoGameProject.Scenes
{
    public class StartScene : IScene
    {
        private SpriteFont _font;
        private ContentManager _content;
        private SceneManager _sceneManager;
        private Game _game;

        public StartScene(ContentManager content, SceneManager sceneManager, Game game)
        {
            _content = content;
            _sceneManager = sceneManager;
            _game = game;
            _font = content.Load<SpriteFont>("DefaultFont");
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                _sceneManager.ChangeScene(new LevelScene(_content, _game.GraphicsDevice, _sceneManager, _game));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_font, "PLATFORMER GAME", new Vector2(200, 150), Color.White);
            spriteBatch.DrawString(_font, "Press ENTER to Start", new Vector2(200, 200), Color.White);
            spriteBatch.DrawString(_font, "Controls: Q/D = Move, Z = Jump, E = Attack", new Vector2(100, 350), Color.Gray);
        }
    }
}