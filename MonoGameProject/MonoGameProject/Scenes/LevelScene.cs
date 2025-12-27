using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameProject.Core;
using MonoGameProject.Entities;
using System;
using System.Collections.Generic;

namespace MonoGameProject.Scenes
{
    public class LevelScene : IScene
    {
        private Player _player;
        private List<Enemy> _enemies;
        private List<Platform> _platforms;
        private Camera _camera;
        private GraphicsDevice _graphicsDevice;
        private SpriteFont _font;
        private ContentManager _content;
        private SceneManager _sceneManager;
        private Game _game;
        private Vector2 _spawnPoint = new Vector2(100, 100);

        // Map size
        private const int MapWidth = 1600;
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

            _enemies = new List<Enemy>
            {
                new Enemy(
                    content.Load<Texture2D>("Enemy1Run"),
                    content.Load<Texture2D>("Enemy1Dead"),
                    new Vector2(400, 100),
                    armoredHead: false
                ),
                new Enemy(
                    content.Load<Texture2D>("Enemy1Run"),
                    content.Load<Texture2D>("Enemy1Dead"),
                    new Vector2(800, 100),
                    armoredHead: false
                )
            };

            _platforms = new List<Platform>();
            Texture2D platformTex = content.Load<Texture2D>("platform");

            // GROND (hele map breed)
            _platforms.Add(new Platform(platformTex, new Vector2(0, 450), MapWidth, 20));

            // Platforms voor level design
            _platforms.Add(new Platform(platformTex, new Vector2(200, 350), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(500, 280), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(900, 350), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(1200, 280), platformTex.Width, 20));

            // Camera setup
            _camera = new Camera(_graphicsDevice.Viewport);
            _camera.SetBounds(0, MapWidth, 0, MapHeight);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currentKeyState = Keyboard.GetState();

            _player.Update(gameTime);
            _player.HandlePlatformCollision(_platforms);

            // Boundarie checks (niet uit map vallen)
            if (_player.Position.X < 0) _player.Position.X = 0;
            if (_player.Position.X > MapWidth - _player.Bounds.Width)
                _player.Position.X = MapWidth - _player.Bounds.Width;

            // Dood als uit scherm valt
            if (_player.Position.Y > MapHeight)
            {
                _player.TakeDamage(999); // Instant death
            }

            // Camera volgt player
            _camera.Follow(_player.Position);

            // Enemy updates en collision
            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime);
                enemy.HandlePlatformCollision(_platforms);

                if (!enemy.IsAlive) continue;

                // Attack collision
                if (_player.AttackHitbox != Rectangle.Empty &&
                    !_player.AttackHitEnemies.Contains(enemy) &&
                    _player.AttackHitbox.Intersects(enemy.Bounds))
                {
                    enemy.TakeDamage(10);
                    _player.AttackHitEnemies.Add(enemy);
                }

                // Stomp collision
                if (enemy.CanBeStomped)
                {
                    int xOverlap = Math.Min(_player.Bounds.Right, enemy.HeadHitbox.Right) -
                                   Math.Max(_player.Bounds.Left, enemy.HeadHitbox.Left);
                    int yOverlap = Math.Min(_player.Bounds.Bottom, enemy.HeadHitbox.Bottom) -
                                   Math.Max(_player.Bounds.Top, enemy.HeadHitbox.Top);
                    bool verticalFall = _player.PreviousBounds.Bottom <= enemy.HeadHitbox.Top &&
                                        _player.Bounds.Bottom >= enemy.HeadHitbox.Top;

                    if (xOverlap > 10 && yOverlap > 0 && verticalFall)
                    {
                        enemy.TakeDamage(20);
                        _player.Velocity.Y = -300f;
                    }
                }

                // Enemy damage player
                if (_player.Bounds.Intersects(enemy.Bounds))
                {
                    _player.TakeDamage(20); // Was 10, nu 20
                }
            }

            // Check player death/respawn
            if (_player.HP <= 0)
            {
                if (_player.Lives > 0)
                {
                    // Respawn met R
                    if (currentKeyState.IsKeyDown(Keys.R) && _previousKeyState.IsKeyUp(Keys.R))
                    {
                        _player.Respawn(_spawnPoint);
                    }
                }
                else
                {
                    // Game Over - naar lose screen
                    _sceneManager.ChangeScene(new GameOverScene(_content, _sceneManager, _game, false));
                }
            }

            // Check alle enemies dood -> naar level 2
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
            // Draw met camera transform
            sb.End();
            sb.Begin(transformMatrix: _camera.Transform);

            foreach (var p in _platforms)
                p.Draw(sb);
            foreach (var e in _enemies)
                e.Draw(sb);
            _player.Draw(sb);

            sb.End();
            sb.Begin(); // UI zonder camera

            // Lives UI (top left)
            sb.DrawString(_font, $"Lives: {_player.Lives}", new Vector2(10, 10), Color.White);

            // Respawn message
            if (_player.HP <= 0 && _player.Lives > 0)
            {
                string msg = "Press R to Respawn";
                Vector2 size = _font.MeasureString(msg);
                sb.DrawString(_font, msg, new Vector2(400 - size.X / 2, 240), Color.Red);
            }
        }
    }
}