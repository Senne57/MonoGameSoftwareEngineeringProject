using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGameProject.Core
{
    /// <summary>
    /// BackgroundFactory - Open/Closed Principle
    /// OPEN voor uitbreiding: Ik kan nieuwe factory methods toevoegen (CreateLevel4Background)
    /// CLOSED voor modificatie: Bestaande methods blijven ongewijzigd
    /// Voordeel: Nieuwe backgrounds toevoegen zonder bestaande scenes te wijzigen
    /// </summary>
    public static class BackgroundFactory
    {
        public static Background CreateAnimatedBackground(
            ContentManager content,
            string textureName,
            int frameCount,
            int mapWidth = 800,
            int mapHeight = 480,
            float scale = 1.0f)
        {
            Texture2D texture = content.Load<Texture2D>(textureName);
            return new AnimatedBackground(texture, frameCount, mapWidth, mapHeight, scale);
        }

        public static Background CreateLayeredBackground(
            ContentManager content,
            string[] layerNames,
            int mapWidth,
            int mapHeight)
        {
            List<Texture2D> layers = new List<Texture2D>();
            foreach (string layerName in layerNames)
            {
                layers.Add(content.Load<Texture2D>(layerName));
            }

            return new LayeredBackground(layers, mapWidth, mapHeight);
        }

        // OCP: Nieuwe background types toevoegen = nieuwe method, rest blijft ongewijzigd
        public static Background CreateLevel1Background(ContentManager content, int mapWidth, int mapHeight)
        {
            // Factory method - encapsulates creation logic
            return CreateLayeredBackground(
                content,
                new[] { "1level1", "2level1", "3level1", "4level1", "5level1" },
                mapWidth,
                mapHeight
            );
        }

        public static Background CreateLevel2Background(ContentManager content, int mapWidth, int mapHeight)
        {
            return CreateLayeredBackground(
                content,
                new[] { "1level2", "2level2", "3level2", "4level2" },
                mapWidth,
                mapHeight
            );
        }

        public static Background CreateLevel3Background(ContentManager content, int mapWidth, int mapHeight)
        {
            return CreateLayeredBackground(
                content,
                new[] { "1level3", "2level3", "3level3", "4level3" },
                mapWidth,
                mapHeight
            );
        }

        public static Background CreateStartBackground(ContentManager content)
        {
            return CreateAnimatedBackground(content, "Start", 36, scale: 0.7f);
        }

        public static Background CreateGameOverBackground(ContentManager content)
        {
            return CreateAnimatedBackground(content, "GameOver", 36, scale: 1.0f);
        }

        public static Background CreateVictoryBackground(ContentManager content)
        {
            return CreateAnimatedBackground(content, "Victory", 36, scale: 1.0f);
        }
    }
}