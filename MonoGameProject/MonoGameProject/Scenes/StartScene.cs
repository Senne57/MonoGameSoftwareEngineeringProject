using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameProject.Core;

namespace MonoGameProject.Scenes
{
    /// <summary>
    /// StartScene - Interface Segregation Principle
    /// Implementeert IScene met alleen Update en Draw
    /// Geen onnodige methods zoals SaveState of HandleInput
    /// Voordeel: Scene implementeert alleen wat het echt nodig heeft
    /// </summary>
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

            _background = BackgroundFactory.CreateStartBackground(content);
            MusicManager.Instance.Play(MusicHelper.MusicNames.Menu, repeat: true);
        }

        public void Update(GameTime gameTime)
        {
            _background.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                _sceneManager.ChangeScene(new LevelScene(_content, _game.GraphicsDevice, _sceneManager, _game));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _background.Draw(spriteBatch);

            spriteBatch.DrawString(_font, "Controls:", new Vector2(5, 5), Color.White);
            spriteBatch.DrawString(_font, "Q/Left Arrow = Move Left", new Vector2(5, 25), Color.White);
            spriteBatch.DrawString(_font, "D/Right Arrow = Move Right", new Vector2(5, 45), Color.White);
            spriteBatch.DrawString(_font, "Z/Up Arrow = Jump", new Vector2(5, 65), Color.White);
            spriteBatch.DrawString(_font, "E = Attack", new Vector2(5, 85), Color.White);
            spriteBatch.DrawString(_font, "R = Respawn (when dead)", new Vector2(5, 105), Color.White);

            string startText = "Press ENTER to Start";
            Vector2 textSize = _font.MeasureString(startText);
            spriteBatch.DrawString(_font, startText,
                new Vector2(400 - textSize.X / 2, 400),
                Color.Yellow);
        }
    }
}