using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGameProject.Core
{
    /// <summary>
    /// Factory voor het creëren van backgrounds
    /// Volgt SOLID: Dependency Inversion - scenes zijn niet afhankelijk van concrete background types
    /// </summary>
    public static class BackgroundFactory
    {
        /// <summary>
        /// Creëer animated background voor menu screens
        /// </summary>
        public static Background CreateAnimatedBackground(
            ContentManager content, 
            string textureName, 
            int frameCount,
            int mapWidth = 800, 
            int mapHeight = 480)
        {
            Texture2D texture = content.Load<Texture2D>(textureName);
            return new AnimatedBackground(texture, frameCount, mapWidth, mapHeight);
        }

        /// <summary>
        /// Creëer layered background voor levels
        /// </summary>
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

        /// <summary>
        /// Helper methods voor specifieke levels
        /// </summary>
        public static Background CreateLevel1Background(ContentManager content, int mapWidth, int mapHeight)
        {
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
            return CreateAnimatedBackground(content, "Start", 36);
        }

        public static Background CreateGameOverBackground(ContentManager content)
        {
            return CreateAnimatedBackground(content, "GameOver", 36);
        }

        public static Background CreateVictoryBackground(ContentManager content)
        {
            return CreateAnimatedBackground(content, "Victory", 36);
        }
    }
}