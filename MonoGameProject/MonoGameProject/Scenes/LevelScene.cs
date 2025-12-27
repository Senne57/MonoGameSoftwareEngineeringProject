using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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

        public LevelScene(ContentManager content)
        {
            _player = new Player(
                content.Load<Texture2D>("Idle"),
                content.Load<Texture2D>("Run"),
                content.Load<Texture2D>("Jump"),
                content.Load<Texture2D>("Attack_1"),
                new Vector2(100, 100)
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
                    new Vector2(600, 100),
                    armoredHead: false
                )
            };

            _platforms = new List<Platform>();
            Texture2D platformTex = content.Load<Texture2D>("platform");

            // GROND
            _platforms.Add(new Platform(
                platformTex,
                new Vector2(0, 450),
                platformTex.Width,
                20 // 👈 HITBOX DUNNER DAN PNG
            ));

            // PLATFORM 1
            _platforms.Add(new Platform(
                platformTex,
                new Vector2(200, 350),
                platformTex.Width,
                20
            ));

            // PLATFORM 2
            _platforms.Add(new Platform(
                platformTex,
                new Vector2(500, 280),
                platformTex.Width,
                20
            ));

        }

        public void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
            _player.HandlePlatformCollision(_platforms);

            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime);
                enemy.HandlePlatformCollision(_platforms);

                // Attack_1
                if (_player.AttackHitbox != Rectangle.Empty &&
                    enemy.IsAlive &&
                    !_player.AttackHitEnemies.Contains(enemy) &&
                    _player.AttackHitbox.Intersects(enemy.Bounds))
                {
                    enemy.TakeDamage(10);
                    _player.AttackHitEnemies.Add(enemy);
                }

                // Stomp
                if (enemy.IsAlive && enemy.CanBeStomped)
                {
                    // Gebruik strengere overlap checks
                    int xOverlap = Math.Min(_player.Bounds.Right, enemy.HeadHitbox.Right) - Math.Max(_player.Bounds.Left, enemy.HeadHitbox.Left);
                    int yOverlap = Math.Min(_player.Bounds.Bottom, enemy.HeadHitbox.Bottom) - Math.Max(_player.Bounds.Top, enemy.HeadHitbox.Top);

                    bool verticalFall = _player.PreviousBounds.Bottom <= enemy.HeadHitbox.Top &&
                                        _player.Bounds.Bottom >= enemy.HeadHitbox.Top;

                    if (xOverlap > 10 && yOverlap > 0 && verticalFall)
                    {
                        enemy.TakeDamage(20);
                        _player.Velocity.Y = -300f;
                    }
                }
            }
        }
        public void Draw(SpriteBatch sb)
        {
            foreach (var p in _platforms)
                p.Draw(sb);

            foreach (var e in _enemies)
                e.Draw(sb);

            _player.Draw(sb);
        }
    }
}
