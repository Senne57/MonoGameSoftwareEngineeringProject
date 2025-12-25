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
        private Enemy _enemy;

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
            _enemy = new Enemy(
            content.Load<Texture2D>("Enemy1Idle"),
            content.Load<Texture2D>("Enemy1Run"),
            content.Load<Texture2D>("Enemy1Attack1"),
            content.Load<Texture2D>("Enemy1Hurt"),
            content.Load<Texture2D>("Enemy1Dead"),
            new Vector2(500, 400)
            );

        }

        public void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
            _enemy.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _player.Draw(spriteBatch);
            _enemy.Draw(spriteBatch);
        }
    }
}
