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

            // ✅ UPDATED: Mix van NormalEnemy en ArmoredKnight
            _enemies = new List<Enemy>
            {
                // ArmoredKnight - gebruikt Walk.png voor run, DeadKnight.png voor death
                new ArmoredKnight(
                    content.Load<Texture2D>("Walk"),       // Run animatie
                    content.Load<Texture2D>("DeadKnight"), // Death animatie
                    new Vector2(400, 100)
                ),
                // NormalEnemy - gebruikt Enemy1Run.png
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
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currentKeyState = Keyboard.GetState();

            _player.Update(gameTime);
            _player.HandlePlatformCollision(_platforms);

            // ✅ FIX: Vector2 is een struct, dus moet je hele property vervangen
            if (_player.Position.X < 0)
                _player.Position = new Vector2(0, _player.Position.Y);
            if (_player.Position.X > MapWidth - _player.Bounds.Width)
                _player.Position = new Vector2(MapWidth - _player.Bounds.Width, _player.Position.Y);

            if (_player.Position.Y > MapHeight)
                _player.TakeDamage(999);

            _camera.Follow(_player.Position);

            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime);
                enemy.HandlePlatformCollision(_platforms);

                if (!enemy.IsAlive) continue;

                bool playerStomped = false;

                // ✅ UPDATED: Check CanBeStomped property
                if (enemy.CanBeStomped && _player.Velocity.Y > 0)
                {
                    int xOverlap = Math.Min(_player.Bounds.Right, enemy.HeadHitbox.Right) -
                                   Math.Max(_player.Bounds.Left, enemy.HeadHitbox.Left);
                    int yOverlap = Math.Min(_player.Bounds.Bottom, enemy.HeadHitbox.Bottom) -
                                   Math.Max(_player.Bounds.Top, enemy.HeadHitbox.Top);
                    bool fromAbove = _player.PreviousBounds.Bottom <= enemy.HeadHitbox.Top + 5;

                    if (xOverlap > 10 && yOverlap > 0 && fromAbove)
                    {
                        // ✅ UPDATED: Type-safe stomp handling
                        if (enemy is NormalEnemy normalEnemy)
                        {
                            normalEnemy.HandleStompDamage(20);
                            // ✅ FIX: Vervang hele Velocity property (Vector2 is struct)
                            _player.Velocity = new Vector2(_player.Velocity.X, -300f);
                            playerStomped = true;
                        }
                    }
                }
                else if (!enemy.CanBeStomped && _player.Velocity.Y > 0)
                {
                    // ✅ NIEUW: Player probeert te stompen op armored knight = damage
                    int xOverlap = Math.Min(_player.Bounds.Right, enemy.HeadHitbox.Right) -
                                   Math.Max(_player.Bounds.Left, enemy.HeadHitbox.Left);
                    int yOverlap = Math.Min(_player.Bounds.Bottom, enemy.HeadHitbox.Bottom) -
                                   Math.Max(_player.Bounds.Top, enemy.HeadHitbox.Top);
                    bool fromAbove = _player.PreviousBounds.Bottom <= enemy.HeadHitbox.Top + 5;

                    if (xOverlap > 10 && yOverlap > 0 && fromAbove)
                    {
                        _player.TakeDamage(15); // Player krijgt damage bij stompen op armor
                        // ✅ FIX: Vervang hele Velocity property (Vector2 is struct)
                        _player.Velocity = new Vector2(_player.Velocity.X, -200f);
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