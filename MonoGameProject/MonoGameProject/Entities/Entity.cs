using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Entities
{
    /// <summary>
    /// Base class for all game entities
    /// Provides common physics and rendering functionality
    /// </summary>
    public abstract class Entity
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);

        // Default collision box, can be overridden by subclasses
        public virtual Rectangle Bounds
        {
            get => new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
        }

        // Frame-rate independent movement
        protected void ApplyVelocity(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * dt;
        }

        // Note: Vector2 is a struct - must replace entire property
        protected void ApplyGravity(float gravity, GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Velocity = new Vector2(Velocity.X, Velocity.Y + gravity * dt);
        }
    }
}