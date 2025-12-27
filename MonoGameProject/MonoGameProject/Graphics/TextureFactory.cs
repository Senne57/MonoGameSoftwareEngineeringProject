using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Core
{
    public static class TextureFactory
    {
        public static Texture2D Pixel;

        public static void Init(GraphicsDevice graphicsDevice)
        {
            Pixel = new Texture2D(graphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });
        }
    }
}
