using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Core
{
    public class Camera
    {
        private Viewport _viewport;
        public Matrix Transform { get; private set; }
        public Vector2 Position { get; private set; }

        private float _minX;
        private float _maxX;
        private float _minY;
        private float _maxY;

        public Camera(Viewport viewport)
        {
            _viewport = viewport;
            Position = Vector2.Zero;
        }

        public void SetBounds(float minX, float maxX, float minY, float maxY)
        {
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
        }

        public void Follow(Vector2 target)
        {
            Position = new Vector2(
                target.X - _viewport.Width / 2f,
                target.Y - _viewport.Height / 2f
            );

            Position = new Vector2(
                MathHelper.Clamp(Position.X, _minX, _maxX - _viewport.Width),
                MathHelper.Clamp(Position.Y, _minY, _maxY - _viewport.Height)
            );

            Transform = Matrix.CreateTranslation(-Position.X, -Position.Y, 0);
        }
    }
}