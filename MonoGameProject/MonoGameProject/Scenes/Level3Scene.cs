using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameProject.Core;
using MonoGameProject.Entities;
using System.Collections.Generic;

namespace MonoGameProject.Scenes
{
    /// <summary>
    /// Level 3 - Boss Fight Parkour
    /// </summary>
    public class Level3Scene : IScene
    {
        private Player _player;
        private FlyingBoss _boss;
        private List<Cannon> _cannons;
        private List<Platform> _platforms;
        private Camera _camera;
        private GraphicsDevice _graphicsDevice;
        private SpriteFont _font;
        private ContentManager _content;
        private SceneManager _sceneManager;
        private Game _game;

        private Vector2 _playerSpawnPoint = new Vector2(100, 100);
        private Vector2 _bossSpawnPoint = new Vector2(-150, 100);

        private CollisionManager _collisionManager;
        private Background _background; // ✅ NIEUW

        private const int MapWidth = 5000;
        private const int MapHeight = 480;

        private KeyboardState _previousKeyState;

        public Level3Scene(ContentManager content, GraphicsDevice graphicsDevice, SceneManager sceneManager, Game game)
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
                _playerSpawnPoint
            );

            _boss = new FlyingBoss(
                content.Load<Texture2D>("FLYING"),
                content.Load<Texture2D>("BOSSHURT"),
                content.Load<Texture2D>("BOSSDEATH"),
                _bossSpawnPoint,
                maxHP: 300
            );

            _platforms = new List<Platform>();
            Texture2D platformTex = content.Load<Texture2D>("platform");

            _platforms.Add(new Platform(platformTex, new Vector2(0, 400), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(300, 350), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(600, 300), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(900, 250), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(1200, 300), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(1500, 350), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(1800, 300), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(2100, 250), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(2400, 320), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(2700, 380), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(3000, 300), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(3300, 250), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(3600, 320), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(3900, 360), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(4200, 300), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(4500, 250), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(4800, 400), platformTex.Width, 20));

            _platforms.Add(new Platform(platformTex, new Vector2(450, 200), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(1050, 150), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(1650, 180), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(2250, 140), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(2850, 160), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(3450, 130), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(4050, 170), platformTex.Width, 20));
            _platforms.Add(new Platform(platformTex, new Vector2(4650, 150), platformTex.Width, 20));

            Texture2D cannonTexture = content.Load<Texture2D>("Cannon_main");
            Texture2D ballTexture = content.Load<Texture2D>("Bomb");

            _cannons = new List<Cannon>
            {
                new Cannon(cannonTexture, ballTexture, new Vector2(500, 330), new Vector2(-1, 0), ballDamage: 30),
                new Cannon(cannonTexture, ballTexture, new Vector2(850, 230), new Vector2(-1, -0.2f), ballDamage: 30),
                new Cannon(cannonTexture, ballTexture, new Vector2(1300, 280), new Vector2(-1, 0.1f), ballDamage: 30),
                new Cannon(cannonTexture, ballTexture, new Vector2(1700, 330), new Vector2(-1, 0), ballDamage: 30),
                new Cannon(cannonTexture, ballTexture, new Vector2(2200, 230), new Vector2(-1, -0.15f), ballDamage: 30),
                new Cannon(cannonTexture, ballTexture, new Vector2(2600, 360), new Vector2(-1, 0.1f), ballDamage: 30),
                new Cannon(cannonTexture, ballTexture, new Vector2(2950, 280), new Vector2(-1, 0), ballDamage: 30),
                new Cannon(cannonTexture, ballTexture, new Vector2(3400, 230), new Vector2(-1, -0.1f), ballDamage: 30),
                new Cannon(cannonTexture, ballTexture, new Vector2(3750, 300), new Vector2(-1, 0), ballDamage: 30),
                new Cannon(cannonTexture, ballTexture, new Vector2(4100, 340), new Vector2(-1, 0.15f), ballDamage: 30),
                new Cannon(cannonTexture, ballTexture, new Vector2(4400, 280), new Vector2(-1, 0), ballDamage: 30),
                new Cannon(cannonTexture, ballTexture, new Vector2(4700, 230), new Vector2(-1, -0.2f), ballDamage: 30),
            };

            _camera = new Camera(_graphicsDevice.Viewport);
            _camera.SetBounds(0, MapWidth, 0, MapHeight);

            _collisionManager = new CollisionManager();

            // ✅ NIEUW: Creëer Level 3 background
            _background = BackgroundFactory.CreateLevel3Background(content, MapWidth, MapHeight);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currentKeyState = Keyboard.GetState();

            if (_player.HP <= 0)
            {
                if (_player.Lives > 0)
                {
                    if (currentKeyState.IsKeyDown(Keys.R) && _previousKeyState.IsKeyUp(Keys.R))
                    {
                        _player.Respawn(_playerSpawnPoint);
                        _boss.Reset(_bossSpawnPoint);

                        foreach (var cannon in _cannons)
                        {
                            cannon.CannonBalls.Clear();
                        }
                    }
                }
                else
                {
                    _sceneManager.ChangeScene(new GameOverScene(_content, _sceneManager, _game, false));
                }

                _previousKeyState = currentKeyState;
                return;
            }

            _player.Update(gameTime);

            _collisionManager.CheckPlayerPlatformCollisions(_player, _platforms);
            _collisionManager.CheckPlayerCannonCollisions(_player, _cannons);

            if (_player.Position.X < 0)
                _player.Position = new Vector2(0, _player.Position.Y);
            if (_player.Position.X > MapWidth - _player.Bounds.Width)
                _player.Position = new Vector2(MapWidth - _player.Bounds.Width, _player.Position.Y);

            if (_player.Position.Y > MapHeight)
                _player.TakeDamage(999);

            // ✅✅✅ CRITICAL FIX: Boss update ALTIJD (ook tijdens death voor animatie!)
            _boss.Update(gameTime);

            // Alleen volgen en collision check als boss nog leeft
            if (_boss.IsAlive)
            {
                _boss.FollowPlayerVertically(_player.Position, gameTime);
                _collisionManager.CheckBossPlayerCollision(_player, _boss);
            }
            else
            {
                // ✅ Als boss dood is, verwijder alle cannonballs
                foreach (var cannon in _cannons)
                {
                    cannon.CannonBalls.Clear();
                }
            }

            foreach (var cannon in _cannons)
            {
                cannon.Update(gameTime);
            }

            _collisionManager.CheckPlayerCannonBallCollisions(_player, _cannons);
            _collisionManager.CheckBossCannonBallCollisions(_boss, _cannons);

            _camera.Follow(_player.Position);

            if (_boss.IsDeathAnimationComplete)
            {
                _sceneManager.ChangeScene(new GameOverScene(_content, _sceneManager, _game, true));
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

            foreach (var cannon in _cannons)
                cannon.Draw(sb);

            _boss.Draw(sb);
            _player.Draw(sb);

            sb.End();
            sb.Begin();

            sb.DrawString(_font, $"Lives: {_player.Lives}", new Vector2(10, 10), Color.White);
            sb.DrawString(_font, "LEVEL 3 - BOSS FIGHT", new Vector2(10, 30), Color.Yellow);

            if (_boss.IsAlive)
            {
                int barWidth = 400;
                int barHeight = 20;
                int barX = (800 - barWidth) / 2;
                int barY = 10;

                sb.Draw(TextureFactory.Pixel,
                    new Rectangle(barX, barY, barWidth, barHeight),
                    Color.DarkRed);

                float hpPercent = _boss.HP / (float)_boss.MaxHP;
                sb.Draw(TextureFactory.Pixel,
                    new Rectangle(barX, barY, (int)(barWidth * hpPercent), barHeight),
                    Color.Gold);

                sb.Draw(TextureFactory.Pixel, new Rectangle(barX - 2, barY - 2, barWidth + 4, 2), Color.White);
                sb.Draw(TextureFactory.Pixel, new Rectangle(barX - 2, barY + barHeight, barWidth + 4, 2), Color.White);
                sb.Draw(TextureFactory.Pixel, new Rectangle(barX - 2, barY - 2, 2, barHeight + 4), Color.White);
                sb.Draw(TextureFactory.Pixel, new Rectangle(barX + barWidth, barY - 2, 2, barHeight + 4), Color.White);

                string bossHPText = $"BOSS: {_boss.HP}/{_boss.MaxHP}";
                Vector2 textSize = _font.MeasureString(bossHPText);
                sb.DrawString(_font, bossHPText,
                    new Vector2(barX + barWidth / 2 - textSize.X / 2, barY + 25),
                    Color.White);
            }

            if (_player.HP <= 0 && _player.Lives > 0)
            {
                string msg = "Press R to Respawn";
                Vector2 size = _font.MeasureString(msg);
                sb.DrawString(_font, msg, new Vector2(400 - size.X / 2, 240), Color.Red);
            }

            sb.End();
            sb.Begin();
        }
    }
}