using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGameProject.Entities
{
    public class Cannon : Entity
    {
        private Texture2D _cannonTexture;
        private Texture2D _ballTexture;
        private float _shootTimer;
        private float _shootInterval = 2.5f;
        private Vector2 _shootDirection;
        private bool _facingRight;
        private const float Scale = 2.0f; // ⬅ NIEUW: 2x groter

        public List<CannonBall> CannonBalls = new List<CannonBall>();

        // ⬅ NIEUW: Bounds voor collision (player kan erop staan)
        public Rectangle Bounds =>
            new Rectangle((int)Position.X, (int)Position.Y, (int)(48 * Scale), (int)(48 * Scale));

        public Cannon(Texture2D cannonTex, Texture2D ballTex, Vector2 position, Vector2 shootDirection)
        {
            _cannonTexture = cannonTex;
            _ballTexture = ballTex;
            Position = position;
            _shootDirection = Vector2.Normalize(shootDirection);
            _shootTimer = _shootInterval;

            if (_shootDirection.X > 0) // Naar rechts
                _facingRight = false;
            else if (_shootDirection.X < 0) // Naar links
                _facingRight = true;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _shootTimer -= dt;

            if (_shootTimer <= 0)
            {
                Shoot();
                _shootTimer = _shootInterval;
            }

            for (int i = CannonBalls.Count - 1; i >= 0; i--)
            {
                CannonBalls[i].Update(gameTime);
                if (!CannonBalls[i].IsActive)
                    CannonBalls.RemoveAt(i);
            }
        }

        private void Shoot()
        {
            // ⬅ GEFIXED: Spawn positie aangepast voor grotere cannon
            // Center van de cannon, maar iets naar voren (in shoot richting)
            float centerX = Position.X + (48 * Scale / 2f);
            float centerY = Position.Y + (48 * Scale / 2f);

            // Offset voor waar de bal uit komt (aan de voorkant van cannon)
            float offsetX = _shootDirection.X * (48 * Scale / 2f) - 50;
            float offsetY = _shootDirection.Y * (48 * Scale / 2f) - 50;

            Vector2 spawnPos = new Vector2(centerX + offsetX, centerY + offsetY);
            CannonBalls.Add(new CannonBall(_ballTexture, spawnPos, _shootDirection));
        }

        public override void Draw(SpriteBatch sb)
        {
            SpriteEffects flip = _facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            // ⬅ NIEUW: Gebruik Scale parameter om groter te tekenen
            sb.Draw(_cannonTexture, Position, null, Color.White, 0f, Vector2.Zero, Scale, flip, 0f);

            sb.Draw(MonoGameProject.Core.TextureFactory.Pixel, Bounds, Color.Green * 0.3f);

            foreach (var ball in CannonBalls)
                ball.Draw(sb);
        }
    }
}