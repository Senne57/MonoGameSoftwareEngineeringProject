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
        private Background _background; // ✅ SOLID: Dependency op abstractie

        public GameOverScene(ContentManager content, SceneManager sceneManager, Game game, bool victory)
        {
            _content = content;
            _sceneManager = sceneManager;
            _game = game;
            _isVictory = victory;
            _font = content.Load<SpriteFont>("DefaultFont");

            // ✅ Creëer juiste background via Factory
            if (_isVictory)
                _background = BackgroundFactory.CreateVictoryBackground(content);
            else
                _background = BackgroundFactory.CreateGameOverBackground(content);
        }

        public void Update(GameTime gameTime)
        {
            // ✅ Update background animatie
            _background.Update(gameTime);

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
            // ✅ Teken background EERST
            _background.Draw(sb);

            if (_isVictory)
            {
                // WIN SCREEN
                string restart = "Press ENTER to play again";

                Vector2 restartSize = _font.MeasureString(restart);

                sb.DrawString(_font, restart, new Vector2(400 - restartSize.X / 2, 400), Color.LightGray);
            }
            else
            {
                // LOSE SCREEN
                string restart = "Press ENTER to play again";

                Vector2 restartSize = _font.MeasureString(restart);

                sb.DrawString(_font, restart, new Vector2(400 - restartSize.X / 2, 400), Color.LightGray);
            }
        }
    }
}