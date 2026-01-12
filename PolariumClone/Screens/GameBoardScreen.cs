using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGameGum;
using PolariumClone.CustomData;
using PolariumClone.GameObjects;
using System;
using System.Collections.Generic;

namespace PolariumClone.Screens
{
    public class GameBoardScreen : GameScreen
    {
        private SpriteBatch _spriteBatch;

        private Texture2DAtlas _atlas;
        private SpriteFont _spriteFont;

        private Dictionary<string, LevelData> _allLevels;
        private LevelData _currentLevel;

        private GameBoard _gameBoard;

        public GameBoardScreen(
            Game game,
            Dictionary<string, LevelData> allLevels,
            string currentLevelName) : base(game)
        {
            _allLevels = allLevels;
            _currentLevel = allLevels[currentLevelName];
        }

        public override void Initialize()
        {
            var mainGame = (PolariumGame)Game;
            mainGame.UIManager.HidePanels();

            mainGame.UIManager.OnBackToLevelSelectButtonClicked += BackToLevelSelectButton_Clicked;
            mainGame.UIManager.OnNextPuzzleButtonClicked += NextPuzzleButton_Clicked;
            mainGame.UIManager.OnRetryPuzzleButtonClicked += RetryPuzzleButton_Clicked;

            base.Initialize();
        }

        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _atlas = Content.Load<Texture2DAtlas>("data/spritesheet");
            _spriteFont = Content.Load<SpriteFont>("fonts/megamax");

            _gameBoard = new GameBoard(
                GraphicsDevice,
                _atlas,
                _spriteFont,
                _currentLevel);

            _gameBoard.PuzzleSovled += OnPuzzleSolved;
            _gameBoard.PuzzleFailed += OnPuzzleFailed;

            base.LoadContent();
        }

        private void BackToLevelSelectButton_Clicked(object sender, EventArgs e)
        {
            var mainGame = (PolariumGame)Game;           

            ScreenManager.CloseScreen();
            mainGame.UIManager.ShowLevelSelectPanel();
        }

        private void NextPuzzleButton_Clicked(object sender, EventArgs e)
        {
            _currentLevel = _allLevels[_currentLevel.NextLevelName];
            _gameBoard.ChangeLevel(_currentLevel);

            var mainGame = (PolariumGame)Game;
            mainGame.UIManager.HidePanels();
        }

        private void RetryPuzzleButton_Clicked(object sender, EventArgs e)
        {
            var mainGame = (PolariumGame)Game;
            mainGame.UIManager.HidePanels();
            
            _gameBoard.Reset();
        }        

        private void OnPuzzleSolved(object sender, EventArgs e)
        {
            var mainGame = (PolariumGame)Game;
            mainGame.UIManager.ShowPuzzleSovledPanel();
        }

        private void OnPuzzleFailed(object sender, EventArgs e)
        {
            var mainGame = (PolariumGame)Game;
            mainGame.UIManager.ShowPuzzleFailedPanel();
        }

        public override void Update(GameTime gameTime)
        {
            GumService.Default.Update(gameTime);
            MouseExtended.Update();
            _gameBoard.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);            

            _gameBoard.Draw(gameTime, _spriteBatch);

            GumService.Default.Draw();
        }
    }
}
