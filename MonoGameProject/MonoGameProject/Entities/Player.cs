using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameProject.Components;

namespace MonoGameProject.Entities
{
    public class Player : Entity
    {
        private Animation _idle;
        private Animation _run;
        private Animation _jump;
        private Animation _attack;
        private Animation _hurt;

        private Animation _current;
        private bool _isGrounded;

        private const float Speed = 200f;
        private const float JumpForce = -350f;
        private const float Gravity = 900f;

        public Player(
            Texture2D idle,
            Texture2D run,
            Texture2D jump,
            Texture2D attack,
            Texture2D hurt,
            Vector2 startPosition)
        {
            Position = startPosition;

            _idle = new Animation(idle, 6, 0.15f);
            _run = new Animation(run, 8, 0.1f);
            _jump = new Animation(jump, 10, 0.08f);
            _attack = new Animation(attack, 4, 0.1f);
            _hurt = new Animation(hurt, 3, 0.15f);

            _current = _idle;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState ks = Keyboard.GetState();

            Velocity.X = 0;

            if (ks.IsKeyDown(Keys.Left))
                Velocity.X = -Speed;
            if (ks.IsKeyDown(Keys.Right))
                Velocity.X = Speed;

            if (ks.IsKeyDown(Keys.Up) && _isGrounded)
            {
                Velocity.Y = JumpForce;
                _isGrounded = false;
            }

            Velocity.Y += Gravity * dt;
            Position += Velocity * dt;

            if (Position.Y >= 400)
            {
                Position.Y = 400;
                Velocity.Y = 0;
                _isGrounded = true;
            }

            // Animatie-selectie
            if (!_isGrounded)
                _current = _jump;
            else if (Velocity.X != 0)
                _current = _run;
            else
                _current = _idle;

            if (ks.IsKeyDown(Keys.Space))
                _current = _attack;

            _current.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _current.Draw(spriteBatch, Position);
        }
    }
}
