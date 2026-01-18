using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGameProject.Core
{
    /// <summary>
    /// Layered background voor levels met meerdere PNG layers
    /// Level 1: 5 layers (1level1, 2level1, 3level1, 4level1, 5level1)
    /// Level 2: 4 layers (1level2, 2level2, 3level2, 4level2)
    /// Level 3: 4 layers (1level3, 2level3, 3level3, 4level3)
    /// Volgt SOLID: Open/Closed - extends Background zonder base te wijzigen
    /// </summary>
    public class LayeredBackground : Background
    {
        private List<Texture2D> _layers;

        public LayeredBackground(List<Texture2D> layers, int mapWidth, int mapHeight)
            : base(mapWidth, mapHeight)
        {
            _layers = layers;
        }

        public override void Update(GameTime gameTime)
        {
            // Static background - geen update nodig
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix? cameraTransform = null)
        {
            // ✅ Teken alle layers over elkaar (geen parallax, gewoon gestretch)
            foreach (var layer in _layers)
            {
                // Stretch elke layer over de hele map breedte en hoogte
                spriteBatch.Draw(
                    layer,
                    new Rectangle(0, 0, _mapWidth, _mapHeight),
                    Color.White
                );
            }
        }
    }
}