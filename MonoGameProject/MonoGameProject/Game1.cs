using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Core;
using MonoGameProject.Scenes;

namespace MonoGameProject
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SceneManager _sceneManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Nodig voor HP-balken
            TextureFactory.Init(GraphicsDevice);

            _sceneManager = new SceneManager();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Start altijd met StartScene
            _sceneManager.ChangeScene(new StartScene(Content, _sceneManager));
        }



        protected override void Update(GameTime gameTime)
        {
            _sceneManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _sceneManager.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
