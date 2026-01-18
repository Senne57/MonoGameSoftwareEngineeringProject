using Microsoft.Xna.Framework.Content;

namespace MonoGameProject.Core
{
    /// <summary>
    /// Helper class om alle muziek te laden
    /// Volgt SOLID: Single Responsibility - alleen muziek laden
    /// </summary>
    public static class MusicHelper
    {
        /// <summary>
        /// Laad alle muziek bestanden voor het hele spel
        /// Roep deze aan in Game1.LoadContent()
        /// </summary>
        public static void LoadAllMusic(ContentManager content)
        {
            MusicManager music = MusicManager.Instance;

            // Menu muziek
            music.LoadMusic(content, "MenuTheme", "MenuTheme");

            // Main game muziek
            music.LoadMusic(content, "MainTheme", "MainTheme");

            // Boss muziek
            music.LoadMusic(content, "BossTheme", "BossTheme");
        }


        /// <summary>
        /// Constanten voor muziek namen (voorkomt typo's)
        /// </summary>
        public static class MusicNames
        {
            public const string Menu = "MenuTheme";      // Start, Victory, Game Over
            public const string Main = "MainTheme";      // Level 1, Level 2
            public const string Boss = "BossTheme";      // Level 3
        }
    }
}