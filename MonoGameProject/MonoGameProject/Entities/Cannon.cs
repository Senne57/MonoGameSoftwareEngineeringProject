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

        public List<CannonBall> CannonBalls = new List<CannonBall>();

        public Rectangle Bounds =>
            new Rectangle((int)Position.X, (int)Position.Y, 48, 48);

        public Cannon(Texture2D cannonTex, Texture2D ballTex, Vector2 position, Vector2 shootDirection)
        {
            _cannonTexture = cannonTex;
            _ballTexture = ballTex;
            Position = position;
            _shootDirection = Vector2.Normalize(shootDirection);
            _shootTimer = _shootInterval;
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
            Vector2 spawnPos = Position + new Vector2(24, 24);
            CannonBalls.Add(new CannonBall(_ballTexture, spawnPos, _shootDirection));
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(_cannonTexture, Position, Color.White);
            foreach (var ball in CannonBalls)
                ball.Draw(sb);
        }
    }
}