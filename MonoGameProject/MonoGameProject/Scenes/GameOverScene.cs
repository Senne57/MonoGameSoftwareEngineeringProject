using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameProject.Core;

namespace MonoGameProject.Scenes
{
    public class GameOverScene : IScene
    {
        private SpriteFont _font;
        private ContentManager _content;
        private SceneManager _sceneManager;
        private Game _game;
        private bool _isVictory;
        private KeyboardState _previousKeyState;

        public GameOverScene(ContentManager content, SceneManager sceneManager, Game game, bool victory)
        {
            _content = content;
            _sceneManager = sceneManager;
            _game = game;
            _isVictory = victory;
            _font = content.Load<SpriteFont>("DefaultFont");
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currentKeyState = Keyboard.GetState();

            // Restart met Enter
            if (currentKeyState.IsKeyDown(Keys.Enter) && _previousKeyState.IsKeyUp(Keys.Enter))
            {
                _sceneManager.ChangeScene(new StartScene(_content, _sceneManager, _game));
            }

            _previousKeyState = currentKeyState;
        }

        public void Draw(SpriteBatch sb)
        {
            if (_isVictory)
            {
                // WIN SCREEN
                string title = "VICTORY!";
                string subtitle = "You completed all levels!";
                string restart = "Press ENTER to return to menu";

                Vector2 titleSize = _font.MeasureString(title);
                Vector2 subtitleSize = _font.MeasureString(subtitle);
                Vector2 restartSize = _font.MeasureString(restart);

                sb.DrawString(_font, title, new Vector2(400 - titleSize.X / 2, 150), Color.Gold);
                sb.DrawString(_font, subtitle, new Vector2(400 - subtitleSize.X / 2, 200), Color.White);
                sb.DrawString(_font, restart, new Vector2(400 - restartSize.X / 2, 280), Color.LightGray);
            }
            else
            {
                // LOSE SCREEN
                string title = "GAME OVER";
                string subtitle = "You ran out of lives...";
                string restart = "Press ENTER to try again";

                Vector2 titleSize = _font.MeasureString(title);
                Vector2 subtitleSize = _font.MeasureString(subtitle);
                Vector2 restartSize = _font.MeasureString(restart);

                sb.DrawString(_font, title, new Vector2(400 - titleSize.X / 2, 150), Color.Red);
                sb.DrawString(_font, subtitle, new Vector2(400 - subtitleSize.X / 2, 200), Color.White);
                sb.DrawString(_font, restart, new Vector2(400 - restartSize.X / 2, 280), Color.LightGray);
            }
        }
    }
}