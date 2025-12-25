using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Components;

namespace MonoGameProject.Entities
{
    public class Enemy : Entity
    {
        private Animation _idle;
        private Animation _run;
        private Animation _attack;
        private Animation _hurt;
        private Animation _dead;

        private Animation _current;

        private float _speed = 100f;
        private bool _movingRight = true;
        private bool _isDead;

        public Enemy(
            Texture2D idle,
            Texture2D run,
            Texture2D attack,
            Texture2D hurt,
            Texture2D dead,
            Vector2 startPosition)
        {
            Position = startPosition;

            _idle = new Animation(idle, 6, 0.15f);
            _run = new Animation(run, 8, 0.1f);
            _attack = new Animation(attack, 6, 0.12f);
            _hurt = new Animation(hurt, 2, 0.2f);
            _dead = new Animation(dead, 3, 0.2f);

            _current = _run;
        }

        public override void Update(GameTime gameTime)
        {
            if (_isDead)
            {
                _current = _dead;
                _current.Update(gameTime);
                return;
            }

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Velocity.X = _movingRight ? _speed : -_speed;
            Position += Velocity * dt;

            if (Position.X > 700)
                _movingRight = false;
            if (Position.X < 100)
                _movingRight = true;

            _current = _run;
            _current.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _current.Draw(spriteBatch, Position);
        }

        public void Kill()
        {
            _isDead = true;
        }
    }
}
