using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Core;
using MonoGameProject.Scenes;

namespace MonoGameProject
{
    /// <summary>
    /// Main game class - entry point for MonoGame
    /// Handles initialization, content loading, and game loop
    /// </summary>
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
            TextureFactory.Init(GraphicsDevice);
            _sceneManager = new SceneManager();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            MusicHelper.LoadAllMusic(Content);
            _sceneManager.ChangeScene(new StartScene(Content, _sceneManager, this));
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