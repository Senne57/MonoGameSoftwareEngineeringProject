using Microsoft.Xna.Framework.Graphics;

namespace MonoGameProject.Core
{
    public static class SceneManager
    {
        public static IScene CurrentScene { get; private set; }

        public static void LoadScene(IScene scene)
        {
            CurrentScene = scene;
        }
    }
}
