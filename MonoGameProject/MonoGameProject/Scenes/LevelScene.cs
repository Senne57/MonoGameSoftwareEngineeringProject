using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameProject.Core;
using MonoGameProject.Entities;

namespace MonoGameProject.Scenes
{
    public class LevelScene : IScene
    {
        private Player _player;
        private Texture2D _playerTexture;

        public LevelScene(ContentManager content)
        {
            _player = new Player(
            content.Load<Texture2D>("Idle"),
            content.Load<Texture2D>("Run"),
            content.Load<Texture2D>("Jump"),
            content.Load<Texture2D>("Attack_1"),
            content.Load<Texture2D>("Hurt"),
            new Vector2(100, 400)
            );

        }

        public void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _player.Draw(spriteBatch);
        }
    }
}
