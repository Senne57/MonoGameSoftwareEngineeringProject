using Microsoft.Xna.Framework;
using MonoGameProject.Entities;
using System;
using System.Collections.Generic;

namespace MonoGameProject.Core
{
    /// <summary>
    /// Centralized collision detection system
    /// Separates collision logic from entity behavior
    /// </summary>
    public class CollisionManager
    {
        // Player platform collision with previous-frame landing detection
        public void CheckPlayerPlatformCollisions(Player player, List<Platform> platforms)
        {
            player.SetGrounded(false);

            foreach (var platform in platforms)
            {
                bool horizontal = player.Bounds.Right > platform.Bounds.Left &&
                                 player.Bounds.Left < platform.Bounds.Right;

                // Check if player was above platform last frame (prevents tunneling)
                bool landing = player.PlatformPreviousBounds.Bottom <= platform.Bounds.Top &&
                              player.Bounds.Bottom >= platform.Bounds.Top &&
                              player.Velocity.Y >= 0;

                if (horizontal && landing)
                {
                    player.Position = new Vector2(player.Position.X, platform.Bounds.Top - 90);
                    player.Velocity = new Vector2(player.Velocity.X, 0);
                    player.SetGrounded(true);
                }
            }
        }

        // Cannons act as platforms - player can stand on them
        public void CheckPlayerCannonCollisions(Player player, List<Cannon> cannons)
        {
            foreach (var cannon in cannons)
            {
                bool horizontal = player.Bounds.Right > cannon.Bounds.Left &&
                                 player.Bounds.Left < cannon.Bounds.Right;

                bool landing = player.PlatformPreviousBounds.Bottom <= cannon.Bounds.Top &&
                              player.Bounds.Bottom >= cannon.Bounds.Top &&
                              player.Velocity.Y >= 0;

                if (horizontal && landing)
                {
                    player.Position = new Vector2(player.Position.X, cannon.Position.Y - 120);
                    player.Velocity = new Vector2(player.Velocity.X, 0);
                    player.SetGrounded(true);
                }
            }
        }

        // Three collision types: Stomp (head), Attack (weapon), Touch (body)
        public void CheckPlayerEnemyCollisions(Player player, List<Enemy> enemies)
        {
            foreach (var enemy in enemies)
            {
                if (!enemy.IsAlive) continue;

                bool playerStomped = false;

                // Check stomp collision (Mario-style)
                if (enemy.CanBeStomped && player.Velocity.Y > 0)
                {
                    playerStomped = CheckStompCollision(player, enemy);
                }
                else if (!enemy.CanBeStomped && player.Velocity.Y > 0)
                {
                    // Armored enemies damage player on stomp attempt
                    CheckArmoredStompAttempt(player, enemy);
                }

                // Check attack hitbox collision
                if (!playerStomped && player.AttackHitbox != Rectangle.Empty &&
                    !player.AttackHitEnemies.Contains(enemy) &&
                    player.AttackHitbox.Intersects(enemy.Bounds))
                {
                    enemy.TakeDamage(10);
                    player.AttackHitEnemies.Add(enemy);
                }

                // Check body contact collision
                if (!playerStomped && player.Bounds.Intersects(enemy.Bounds))
                {
                    player.TakeDamage(20);
                }
            }
        }

        // Stomp requires X/Y overlap and from-above check
        private bool CheckStompCollision(Player player, Enemy enemy)
        {
            int xOverlap = Math.Min(player.Bounds.Right, enemy.HeadHitbox.Right) -
                          Math.Max(player.Bounds.Left, enemy.HeadHitbox.Left);
            int yOverlap = Math.Min(player.Bounds.Bottom, enemy.HeadHitbox.Bottom) -
                          Math.Max(player.Bounds.Top, enemy.HeadHitbox.Top);
            bool fromAbove = player.PreviousBounds.Bottom <= enemy.HeadHitbox.Top + 5;

            if (xOverlap > 10 && yOverlap > 0 && fromAbove)
            {
                if (enemy is NormalEnemy normalEnemy)
                {
                    normalEnemy.HandleStompDamage(20);
                    player.Velocity = new Vector2(player.Velocity.X, -300f);
                    return true;
                }
            }
            return false;
        }

        private void CheckArmoredStompAttempt(Player player, Enemy enemy)
        {
            int xOverlap = Math.Min(player.Bounds.Right, enemy.HeadHitbox.Right) -
                          Math.Max(player.Bounds.Left, enemy.HeadHitbox.Left);
            int yOverlap = Math.Min(player.Bounds.Bottom, enemy.HeadHitbox.Bottom) -
                          Math.Max(player.Bounds.Top, enemy.HeadHitbox.Top);
            bool fromAbove = player.PreviousBounds.Bottom <= enemy.HeadHitbox.Top + 5;

            if (xOverlap > 10 && yOverlap > 0 && fromAbove)
            {
                player.TakeDamage(15);
                player.Velocity = new Vector2(player.Velocity.X, -200f);
            }
        }

        public void CheckPlayerSpikeCollisions(Player player, List<SpikeTrap> spikes)
        {
            foreach (var spike in spikes)
            {
                if (player.Bounds.Intersects(spike.Bounds))
                {
                    player.TakeDamage(spike.Damage);
                }
            }
        }

        public void CheckPlayerCannonBallCollisions(Player player, List<Cannon> cannons)
        {
            foreach (var cannon in cannons)
            {
                // Iterate backwards for safe removal during loop
                for (int i = cannon.CannonBalls.Count - 1; i >= 0; i--)
                {
                    var ball = cannon.CannonBalls[i];
                    if (!ball.IsActive) continue;

                    if (player.Bounds.Intersects(ball.Bounds))
                    {
                        player.TakeDamage(ball.Damage);
                        ball.Explode();
                    }
                }
            }
        }

        // Delegate platform collision to enemies themselves
        public void CheckEnemyPlatformCollisions(List<Enemy> enemies, List<Platform> platforms)
        {
            foreach (var enemy in enemies)
            {
                enemy.HandlePlatformCollision(platforms);
            }
        }

        // Boss contact = instant kill (game design choice)
        public void CheckBossPlayerCollision(Player player, FlyingBoss boss)
        {
            if (!boss.IsAlive) return;

            if (player.Bounds.Intersects(boss.Bounds))
            {
                player.TakeDamage(999);
            }
        }

        // Boss can only be damaged by cannonballs
        public void CheckBossCannonBallCollisions(FlyingBoss boss, List<Cannon> cannons)
        {
            if (!boss.IsAlive) return;

            foreach (var cannon in cannons)
            {
                for (int i = cannon.CannonBalls.Count - 1; i >= 0; i--)
                {
                    var ball = cannon.CannonBalls[i];
                    if (!ball.IsActive) continue;

                    if (boss.Bounds.Intersects(ball.Bounds))
                    {
                        boss.TakeDamage(ball.Damage);
                        ball.Explode();
                    }
                }
            }
        }
    }
}