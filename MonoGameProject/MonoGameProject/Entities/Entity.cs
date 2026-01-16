using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Entities
{
    /// <summary>
    /// Base class voor alle game entities met positie en velocity
    /// Volgt SOLID: Single Responsibility - alleen basis physics en rendering
    /// </summary>
    public abstract class Entity
    {
        // Properties i.p.v. public fields - beter voor encapsulation
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        // Abstract methods die elke entity MOET implementeren
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);

        // Virtual Bounds - subclasses kunnen dit overriden voor collision detection
        // Default implementatie voor basis entities
        public virtual Rectangle Bounds
        {
            get => new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
        }

        // Helper method die alle entities kunnen gebruiken
        // Voorkomt code duplicatie in Update methods
        protected void ApplyVelocity(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * dt;
        }

        // Helper voor gravity (veel entities gebruiken dit)
        protected void ApplyGravity(float gravity, GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // ✅ FIX: Vervang hele Velocity property (Vector2 is een struct)
            Velocity = new Vector2(Velocity.X, Velocity.Y + gravity * dt);
        }
    }
}