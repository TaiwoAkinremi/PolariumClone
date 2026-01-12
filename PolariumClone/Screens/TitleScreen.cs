using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGameGum;
using PolariumClone.CustomData;
using PolariumClone.UI;
using System;
using System.Collections.Generic;

namespace PolariumClone.Screens
{
    public class TitleScreen : GameScreen
    {        
        private Dictionary<string, LevelData> _allLevels;

        public TitleScreen(PolariumGame game) : base(game)
        {
        }

        public override void Initialize()
        {
            _allLevels = new Dictionary<string, LevelData>();
            base.Initialize();
        }

        public override void LoadContent()
        {
            _allLevels = Content.Load<Dictionary<string, LevelData>>("data/levels");

            var mainGame = (PolariumGame)Game;
            mainGame.UIManager.CreateUI(_allLevels);
            mainGame.UIManager.ShowTitlePanel();

            mainGame.UIManager.OnQuitButtonClicked += QuitButton_Clicked;
            mainGame.UIManager.OnStartButtonClicked += StartButton_Clicked;
            mainGame.UIManager.OnBackToTitleButtonClicked += BackToTitleButton_Clicked;
            mainGame.UIManager.OnLevelButtonClicked += LevelButton_Clicked;
        }
        
        private void QuitButton_Clicked(object sender, EventArgs e)
        {
            Game.Exit();
        }

        private void StartButton_Clicked(object sender, EventArgs e)
        {
            var mainGame = (PolariumGame)Game;
            mainGame.UIManager.ShowLevelSelectPanel();
        }

        private void BackToTitleButton_Clicked(object sender, EventArgs e)
        {
            var mainGame = (PolariumGame)Game;
            mainGame.UIManager.ShowTitlePanel();
        }

        private void LevelButton_Clicked(object sender, LevelSelectedEventArgs e)
        {
            ScreenManager.ShowScreen(new GameBoardScreen(Game, _allLevels, e.LevelName));
        }

        public override void Update(GameTime gameTime)
        {
            GumService.Default.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GumService.Default.Draw();
        }
    }
}
