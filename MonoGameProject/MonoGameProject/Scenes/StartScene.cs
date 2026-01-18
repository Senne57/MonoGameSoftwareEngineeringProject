using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameProject.Core;

namespace MonoGameProject.Scenes
{
    public class StartScene : IScene
    {
        private SpriteFont _font;
        private ContentManager _content;
        private SceneManager _sceneManager;
        private Game _game;
        private Background _background;

        public StartScene(ContentManager content, SceneManager sceneManager, Game game)
        {
            _content = content;
            _sceneManager = sceneManager;
            _game = game;
            _font = content.Load<SpriteFont>("DefaultFont");

            // Creëer animated background via Factory
            _background = BackgroundFactory.CreateStartBackground(content);

            // ✅ NIEUW: Start menu muziek
            MusicManager.Instance.Play(MusicHelper.MusicNames.Menu, repeat: true);
        }

        public void Update(GameTime gameTime)
        {
            // Update background animatie
            _background.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                _sceneManager.ChangeScene(new LevelScene(_content, _game.GraphicsDevice, _sceneManager, _game));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Teken background EERST (achter alle UI)
            _background.Draw(spriteBatch);


            // UI tekenen
            spriteBatch.DrawString(_font, "Controls:", new Vector2(5, 5), Color.White);
            spriteBatch.DrawString(_font, "Q/D = Move Left/Right", new Vector2(5, 25), Color.White);
            spriteBatch.DrawString(_font, "Z = Jump", new Vector2(5, 45), Color.White);
            spriteBatch.DrawString(_font, "E = Attack", new Vector2(5, 65), Color.White);
        }
    }
}