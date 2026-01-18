using Microsoft.Xna.Framework;
using MonoGameProject.Entities;
using System;
using System.Collections.Generic;

namespace MonoGameProject.Core
{
    /// <summary>
    /// CollisionManager - Verantwoordelijk voor ALLE collision detection
    /// Volgt SOLID: Single Responsibility Principle
    /// </summary>
    public class CollisionManager
    {
        // ============================================
        // PLAYER <-> PLATFORM COLLISIONS
        // ============================================

        public void CheckPlayerPlatformCollisions(Player player, List<Platform> platforms)
        {
            player.SetGrounded(false);

            foreach (var platform in platforms)
            {
                bool horizontal = player.Bounds.Right > platform.Bounds.Left &&
                                 player.Bounds.Left < platform.Bounds.Right;
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

        // ============================================
        // PLAYER <-> CANNON COLLISIONS (kan erop staan)
        // ✅ FIXED: Gebruikt cannon Position ipv Bounds voor correcte landing
        // ============================================

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
                    // ✅ FIX: Gebruik cannon.Position.Y (visuele top) ipv Bounds.Top
                    // Cannon is 48*2 = 96 pixels hoog, player sprite offset is 90
                    player.Position = new Vector2(player.Position.X, cannon.Position.Y - 120);
                    player.Velocity = new Vector2(player.Velocity.X, 0);
                    player.SetGrounded(true);
                }
            }
        }

        // ============================================
        // PLAYER <-> ENEMY COLLISIONS
        // ============================================

        public void CheckPlayerEnemyCollisions(Player player, List<Enemy> enemies)
        {
            foreach (var enemy in enemies)
            {
                if (!enemy.IsAlive) continue;

                bool playerStomped = false;

                // Check STOMP collision (van boven op hoofd springen)
                if (enemy.CanBeStomped && player.Velocity.Y > 0)
                {
                    playerStomped = CheckStompCollision(player, enemy);
                }
                else if (!enemy.CanBeStomped && player.Velocity.Y > 0)
                {
                    // Player probeert te stompen op armored enemy = damage aan player
                    CheckArmoredStompAttempt(player, enemy);
                }

                // Check ATTACK collision
                if (!playerStomped && player.AttackHitbox != Rectangle.Empty &&
                    !player.AttackHitEnemies.Contains(enemy) &&
                    player.AttackHitbox.Intersects(enemy.Bounds))
                {
                    enemy.TakeDamage(10);
                    player.AttackHitEnemies.Add(enemy);
                }

                // Check TOUCH collision (body contact = damage aan player)
                if (!playerStomped && player.Bounds.Intersects(enemy.Bounds))
                {
                    player.TakeDamage(20);
                }
            }
        }

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
                player.TakeDamage(15); // Player krijgt damage bij stompen op armor
                player.Velocity = new Vector2(player.Velocity.X, -200f);
            }
        }

        // ============================================
        // PLAYER <-> SPIKE COLLISIONS
        // ============================================

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

        // ============================================
        // PLAYER <-> CANNONBALL COLLISIONS
        // ============================================

        public void CheckPlayerCannonBallCollisions(Player player, List<Cannon> cannons)
        {
            foreach (var cannon in cannons)
            {
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

        // ============================================
        // ENEMY <-> PLATFORM COLLISIONS
        // ============================================

        public void CheckEnemyPlatformCollisions(List<Enemy> enemies, List<Platform> platforms)
        {
            foreach (var enemy in enemies)
            {
                enemy.HandlePlatformCollision(platforms);
            }
        }

        // ============================================
        // BOSS <-> PLAYER COLLISIONS (Instant Kill)
        // ============================================

        public void CheckBossPlayerCollision(Player player, FlyingBoss boss)
        {
            if (!boss.IsAlive) return;

            if (player.Bounds.Intersects(boss.Bounds))
            {
                player.TakeDamage(999); // Instant kill
            }
        }

        // ============================================
        // BOSS <-> CANNONBALL COLLISIONS
        // ============================================

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