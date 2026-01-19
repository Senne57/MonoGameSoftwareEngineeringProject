using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Core;
using MonoGameProject.Scenes;

namespace MonoGameProject
{
    /// <summary>
    /// Game1 - Main game class
    /// 
    /// SOLID Principes in dit project:
    /// 
    /// S - Single Responsibility:
    ///     CollisionManager: alleen collision detection
    ///     MusicManager: alleen muziek management
    ///     
    /// O - Open/Closed:
    ///     Enemy base class: extend voor nieuwe enemies zonder base te wijzigen
    ///     BackgroundFactory: nieuwe factory methods toevoegen
    ///     
    /// L - Liskov Substitution:
    ///     List<Enemy> werkt met alle enemy types (NormalEnemy, ArmoredKnight, FlyingBoss)
    ///     CollisionManager accepteert alle Enemy subclasses
    ///     
    /// I - Interface Segregation:
    ///     IScene: kleine interface met alleen Update en Draw
    ///     Scenes implementeren alleen wat ze nodig hebben
    ///     
    /// D - Dependency Inversion:
    ///     SceneManager hangt af van IScene abstractie
    ///     LevelScene hangt af van Background abstractie
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