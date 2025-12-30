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

            _enemies = new List<Enemy>
            {
                new Enemy(
                    content.Load<Texture2D>("Enemy1Run"),
                    content.Load<Texture2D>("Enemy1Dead"),
                    new Vector2(400, 100),
                    armoredHead: true
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

            _platforms.Add(new Platform(platformTex, new Vector2(0, 400), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(250, 300), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(650, 200), platformTex.Width, 20));

            _camera = new Camera(_graphicsDevice.Viewport);
            _camera.SetBounds(0, MapWidth, 0, MapHeight);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currentKeyState = Keyboard.GetState();

            _player.Update(gameTime);
            _player.HandlePlatformCollision(_platforms);

            if (_player.Position.X < 0) _player.Position.X = 0;
            if (_player.Position.X > MapWidth - _player.Bounds.Width)
                _player.Position.X = MapWidth - _player.Bounds.Width;

            if (_player.Position.Y > MapHeight)
                _player.TakeDamage(999);

            _camera.Follow(_player.Position);

            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime);
                enemy.HandlePlatformCollision(_platforms);

                if (!enemy.IsAlive) continue;

                bool playerStomped = false;
                if (enemy.CanBeStomped && _player.Velocity.Y > 0)
                {
                    int xOverlap = Math.Min(_player.Bounds.Right, enemy.HeadHitbox.Right) -
                                   Math.Max(_player.Bounds.Left, enemy.HeadHitbox.Left);
                    int yOverlap = Math.Min(_player.Bounds.Bottom, enemy.HeadHitbox.Bottom) -
                                   Math.Max(_player.Bounds.Top, enemy.HeadHitbox.Top);
                    bool fromAbove = _player.PreviousBounds.Bottom <= enemy.HeadHitbox.Top + 5;

                    if (xOverlap > 10 && yOverlap > 0 && fromAbove)
                    {
                        enemy.TakeDamage(20);
                        _player.Velocity.Y = -300f;
                        playerStomped = true;
                    }
                }

                if (!playerStomped && _player.AttackHitbox != Rectangle.Empty &&
                    !_player.AttackHitEnemies.Contains(enemy) &&
                    _player.AttackHitbox.Intersects(enemy.Bounds))
                {
                    enemy.TakeDamage(10);
                    _player.AttackHitEnemies.Add(enemy);
                }

                if (!playerStomped && _player.Bounds.Intersects(enemy.Bounds))
                    _player.TakeDamage(20);
            }

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

            foreach (var p in _platforms)
                p.Draw(sb);
            foreach (var e in _enemies)
                e.Draw(sb);
            _player.Draw(sb);

            sb.End();
            sb.Begin();

            sb.DrawString(_font, $"Lives: {_player.Lives}", new Vector2(10, 10), Color.White);

            if (_player.HP <= 0 && _player.Lives > 0)
            {
                string msg = "Press R to Respawn";
                Vector2 size = _font.MeasureString(msg);
                sb.DrawString(_font, msg, new Vector2(400 - size.X / 2, 240), Color.Red);
            }
        }
    }
}