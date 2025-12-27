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
    public class Level2Scene : IScene
    {
        private Player _player;
        private List<Enemy> _enemies;
        private List<SpikeTrap> _spikes;
        private List<Cannon> _cannons;
        private List<Platform> _platforms;
        private Camera _camera;
        private GraphicsDevice _graphicsDevice;
        private SpriteFont _font;
        private ContentManager _content;
        private SceneManager _sceneManager;
        private Game _game;
        private Vector2 _spawnPoint = new Vector2(100, 100);

        private const int MapWidth = 2000;
        private const int MapHeight = 480;

        private KeyboardState _previousKeyState;

        public Level2Scene(ContentManager content, GraphicsDevice graphicsDevice, SceneManager sceneManager, Game game)
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

            // Level 2: meer enemies
            _enemies = new List<Enemy>
            {
                new Enemy(
                    content.Load<Texture2D>("Enemy1Run"),
                    content.Load<Texture2D>("Enemy1Dead"),
                    new Vector2(600, 100),
                    armoredHead: false
                ),
                new Enemy(
                    content.Load<Texture2D>("Enemy1Run"),
                    content.Load<Texture2D>("Enemy1Dead"),
                    new Vector2(1100, 100),
                    armoredHead: false
                ),
                new Enemy(
                    content.Load<Texture2D>("Enemy1Run"),
                    content.Load<Texture2D>("Enemy1Dead"),
                    new Vector2(1500, 100),
                    armoredHead: true // Deze heeft armor
                )
            };

            // Spike traps - HOGER en GROTER voor betere collision
            Texture2D spikeTexture = CreateSpikeTexture(graphicsDevice);
            _spikes = new List<SpikeTrap>
            {
                new SpikeTrap(spikeTexture, new Vector2(300, 402)),   // Platform Y=450, spike 48px hoog
                new SpikeTrap(spikeTexture, new Vector2(900, 402)),
                new SpikeTrap(spikeTexture, new Vector2(1300, 402))
            };

            // Cannons
            Texture2D cannonTexture = CreateCannonTexture(graphicsDevice);
            Texture2D ballTexture = CreateBallTexture(graphicsDevice);

            _cannons = new List<Cannon>
            {
                new Cannon(cannonTexture, ballTexture, new Vector2(500, 380), new Vector2(1, 0)), // Schiet naar rechts
                new Cannon(cannonTexture, ballTexture, new Vector2(1400, 280), new Vector2(-1, 0)) // Schiet naar links
            };

            // Platforms
            _platforms = new List<Platform>();
            Texture2D platformTex = content.Load<Texture2D>("platform");

            _platforms.Add(new Platform(platformTex, new Vector2(0, 450), MapWidth, 20)); // Grond
            _platforms.Add(new Platform(platformTex, new Vector2(250, 350), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(600, 280), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(900, 350), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(1200, 280), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(1600, 350), platformTex.Width, 20));

            _camera = new Camera(_graphicsDevice.Viewport);
            _camera.SetBounds(0, MapWidth, 0, MapHeight);
        }

        // PLACEHOLDER TEXTURES
        private Texture2D CreateSpikeTexture(GraphicsDevice gd)
        {
            Texture2D tex = new Texture2D(gd, 32, 48); // HOGER gemaakt (was 32, nu 48)
            Color[] data = new Color[32 * 48];
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.DarkRed;
            tex.SetData(data);
            return tex;
        }

        private Texture2D CreateCannonTexture(GraphicsDevice gd)
        {
            Texture2D tex = new Texture2D(gd, 48, 48);
            Color[] data = new Color[48 * 48];
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Gray;
            tex.SetData(data);
            return tex;
        }

        private Texture2D CreateBallTexture(GraphicsDevice gd)
        {
            Texture2D tex = new Texture2D(gd, 16, 16);
            Color[] data = new Color[16 * 16];
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Black;
            tex.SetData(data);
            return tex;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currentKeyState = Keyboard.GetState();

            _player.Update(gameTime);
            _player.HandlePlatformCollision(_platforms);

            // Boundaries
            if (_player.Position.X < 0) _player.Position.X = 0;
            if (_player.Position.X > MapWidth - _player.Bounds.Width)
                _player.Position.X = MapWidth - _player.Bounds.Width;

            if (_player.Position.Y > MapHeight)
                _player.TakeDamage(999);

            _camera.Follow(_player.Position);

            // Enemy updates
            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime);
                enemy.HandlePlatformCollision(_platforms);

                if (!enemy.IsAlive) continue;

                // 1. Check stomp EERST (hoogste prioriteit)
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

                // 2. Attack collision (als niet gestompt)
                if (!playerStomped && _player.AttackHitbox != Rectangle.Empty &&
                    !_player.AttackHitEnemies.Contains(enemy) &&
                    _player.AttackHitbox.Intersects(enemy.Bounds))
                {
                    enemy.TakeDamage(10);
                    _player.AttackHitEnemies.Add(enemy);
                }

                // 3. Enemy damage player - ALLEEN als NIET gestompt
                if (!playerStomped && _player.Bounds.Intersects(enemy.Bounds))
                    _player.TakeDamage(20);
            }

            // Spike collision
            foreach (var spike in _spikes)
            {
                if (_player.Bounds.Intersects(spike.Bounds))
                    _player.TakeDamage(spike.Damage);
            }

            // Cannon updates en projectile collision
            foreach (var cannon in _cannons)
            {
                cannon.Update(gameTime);

                // Check alle cannonballs
                for (int i = cannon.CannonBalls.Count - 1; i >= 0; i--)
                {
                    var ball = cannon.CannonBalls[i];
                    if (!ball.IsActive) continue;

                    // Simpele collision check
                    if (_player.Bounds.Intersects(ball.Bounds))
                    {
                        _player.TakeDamage(ball.Damage);
                        ball.IsActive = false; // Destroy cannonball
                    }
                }
            }

            // Death/respawn check
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

            // Victory condition: alle enemies dood
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
                _sceneManager.ChangeScene(new GameOverScene(_content, _sceneManager, _game, true));
            }

            _previousKeyState = currentKeyState;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(transformMatrix: _camera.Transform);

            foreach (var p in _platforms)
                p.Draw(sb);

            foreach (var spike in _spikes)
                spike.Draw(sb);

            foreach (var cannon in _cannons)
                cannon.Draw(sb);

            foreach (var e in _enemies)
                e.Draw(sb);

            _player.Draw(sb);

            sb.End();
            sb.Begin();

            // UI
            sb.DrawString(_font, $"Lives: {_player.Lives}", new Vector2(10, 10), Color.White);
            sb.DrawString(_font, "LEVEL 2", new Vector2(10, 30), Color.Yellow);

            if (_player.HP <= 0 && _player.Lives > 0)
            {
                string msg = "Press R to Respawn";
                Vector2 size = _font.MeasureString(msg);
                sb.DrawString(_font, msg, new Vector2(400 - size.X / 2, 240), Color.Red);
            }
        }
    }
}