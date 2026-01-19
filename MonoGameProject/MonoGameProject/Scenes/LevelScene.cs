using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameProject.Core;
using MonoGameProject.Entities;
using System.Collections.Generic;

namespace MonoGameProject.Scenes
{
    public class LevelScene : IScene
    {
        private Player _player;

        // Liskov Substitution Principle:
        // List<Enemy> kan alle enemy types bevatten (NormalEnemy, ArmoredKnight)
        // Ze zijn allemaal vervangbaar - ik hoef niet te checken welk type het is
        // Voordeel: foreach loop werkt met alle enemy types zonder type checks
        private List<Enemy> _enemies;
        private List<Platform> _platforms;
        private Camera _camera;
        private GraphicsDevice _graphicsDevice;
        private SpriteFont _font;
        private ContentManager _content;
        private SceneManager _sceneManager;
        private Game _game;
        private Vector2 _spawnPoint = new Vector2(100, 100);

        private CollisionManager _collisionManager;
        private Background _background;

        private const int MapWidth = 1100;
        private const int MapHeight = 480;

        private KeyboardState _previousKeyState;

        public LevelScene(ContentManager content, GraphicsDevice graphicsDevice, SceneManager sceneManager, Game game)
        {
            _content = content;
            _graphicsDevice = graphicsDevice;
            _sceneManager = sceneManager;
            _game = game;
            _font = content.Load<SpriteFont>("DefaultFont");

            _player = new Player(
                content.Load<Texture2D>("Idle"),
                content.Load<Texture2D>("Run"),
                content.Load<Texture2D>("Jump"),
                content.Load<Texture2D>("Attack_1"),
                _spawnPoint
            );
            // LSP: Verschillende enemy types in zelfde lijst - allemaal vervangbaar
            _enemies = new List<Enemy>
            {
                new ArmoredKnight(
                    content.Load<Texture2D>("Walk"),
                    content.Load<Texture2D>("DeadKnight"),
                    new Vector2(400, 100)
                ),
                new NormalEnemy(
                    content.Load<Texture2D>("Enemy1Run"),
                    content.Load<Texture2D>("Enemy1Dead"),
                    new Vector2(800, 100)
                )
            };

            _platforms = new List<Platform>();
            Texture2D platformTex = content.Load<Texture2D>("platform");

            _platforms.Add(new Platform(platformTex, new Vector2(0, 400), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(250, 300), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(650, 200), platformTex.Width, 20));

            _camera = new Camera(_graphicsDevice.Viewport);
            _camera.SetBounds(0, MapWidth, 0, MapHeight);

            _collisionManager = new CollisionManager();

            // Creëer Level 1 background
            _background = BackgroundFactory.CreateLevel1Background(content, MapWidth, MapHeight);

            // ✅ NIEUW: Start Level 1 & 2 muziek (MainTheme)
            MusicManager.Instance.Play(MusicHelper.MusicNames.Main, repeat: true);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currentKeyState = Keyboard.GetState();

            _player.Update(gameTime);

            _collisionManager.CheckPlayerPlatformCollisions(_player, _platforms);

            if (_player.Position.X < 0)
                _player.Position = new Vector2(0, _player.Position.Y);
            if (_player.Position.X > MapWidth - _player.Bounds.Width)
                _player.Position = new Vector2(MapWidth - _player.Bounds.Width, _player.Position.Y);

            if (_player.Position.Y > MapHeight)
                _player.TakeDamage(999);

            _camera.Follow(_player.Position);

            // LSP: Dit werkt voor ALLE enemy types zonder type checks
            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime);
            }

            _collisionManager.CheckEnemyPlatformCollisions(_enemies, _platforms);
            _collisionManager.CheckPlayerEnemyCollisions(_player, _enemies);

            if (_player.HP <= 0)
            {
                if (_player.Lives > 0)
                {
                    if (currentKeyState.IsKeyDown(Keys.R) && _previousKeyState.IsKeyUp(Keys.R))
                        _player.Respawn(_spawnPoint);
                }
                else
                {
                    _sceneManager.ChangeScene(new GameOverScene(_content, _sceneManager, _game, false));
                }
            }

            bool allDead = true;
            foreach (var e in _enemies)
            {
                if (e.IsAlive)
                {
                    allDead = false;
                    break;
                }
            }

            if (allDead)
            {
                _sceneManager.ChangeScene(new Level2Scene(_content, _graphicsDevice, _sceneManager, _game));
            }

            _previousKeyState = currentKeyState;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(transformMatrix: _camera.Transform);

            _background.Draw(sb, _camera.Transform);

            foreach (var p in _platforms)
                p.Draw(sb);
            foreach (var e in _enemies)
                e.Draw(sb);
            _player.Draw(sb);

            sb.End();
            sb.Begin();

            sb.DrawString(_font, $"Lives: {_player.Lives}", new Vector2(10, 10), Color.White);
            sb.DrawString(_font, "LEVEL 1", new Vector2(10, 30), Color.Yellow);

            if (_player.HP <= 0 && _player.Lives > 0)
            {
                string msg = "Press R to Respawn";
                Vector2 size = _font.MeasureString(msg);
                sb.DrawString(_font, msg, new Vector2(400 - size.X / 2, 240), Color.Red);
            }
        }
    }
}