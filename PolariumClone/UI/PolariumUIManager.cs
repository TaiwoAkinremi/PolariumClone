using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals.V3;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using PolariumClone.CustomData;
using System;
using System.Collections.Generic;

namespace PolariumClone.UI
{
    public class PolariumUIManager
    {
        //For the title screen
        private Panel _titlePanel;
        private Panel _levelSelectPanel;
        public EventHandler OnQuitButtonClicked;
        public EventHandler OnBackToTitleButtonClicked;
        public EventHandler OnStartButtonClicked;
        public EventHandler<LevelSelectedEventArgs> OnLevelButtonClicked;

        //On the game screen
        private Panel _puzzleSolvedPanel;
        private Panel _puzzleFailedPanel;
        public EventHandler OnBackToLevelSelectButtonClicked;
        public EventHandler OnNextPuzzleButtonClicked;
        public EventHandler OnRetryPuzzleButtonClicked;        

        public void CreateUI(Dictionary<string, LevelData> levels)
        {
            CreateTitlePanel();
            CreateLevelSelectPanel(levels);
            CreatePuzzleSovledPanel();
            CreatePuzzleFailedPanel();
        }
                
        private void CreateTitlePanel()
        {
            _titlePanel = new Panel();
            _titlePanel.Dock(Dock.Fill);
            _titlePanel.AddToRoot();

            var titleText = new TextRuntime();
            titleText.Text = "Polarium";
            titleText.UseCustomFont = true;
            titleText.CustomFontFile = "fonts/megamax.fnt";
            titleText.WidthUnits = DimensionUnitType.RelativeToChildren;
            titleText.Color = Color.White;
            titleText.Anchor(Anchor.Center);
            _titlePanel.AddChild(titleText);

            var quitButton = CreateButton();
            quitButton.Text = "QUIT";
            quitButton.Anchor(Anchor.BottomLeft);
            quitButton.X = 70;
            quitButton.Y = -28;
            quitButton.Click += QuitButton_Clicked; ;
            _titlePanel.AddChild(quitButton);

            var startButton = CreateButton();
            startButton.Text = "START";
            startButton.Anchor(Anchor.BottomRight);
            startButton.X = -50;
            startButton.Y = -28;
            startButton.Click += StartButton_Clicked;
            _titlePanel.AddChild(startButton);
        }

        private void QuitButton_Clicked(object sender, EventArgs e)
        {
            if (OnQuitButtonClicked != null)
                OnQuitButtonClicked(this, EventArgs.Empty);
        }

        private void StartButton_Clicked(object sender, EventArgs e)
        {
            if (OnStartButtonClicked != null)
                OnStartButtonClicked(this, EventArgs.Empty);
        }

        private void CreateLevelSelectPanel(Dictionary<string, LevelData> levels)
        {
            _levelSelectPanel = new Panel();
            _levelSelectPanel.Dock(Dock.Fill);
            _levelSelectPanel.IsVisible = false;
            _levelSelectPanel.AddToRoot();

            var stackPanel = new StackPanel();
            stackPanel.Anchor(Anchor.Center);
            stackPanel.WidthUnits = DimensionUnitType.Absolute;
            stackPanel.Width = 1000;
            stackPanel.Visual.ChildrenLayout = ChildrenLayout.LeftToRightStack;
            stackPanel.Visual.WrapsChildren = true;
            stackPanel.Visual.StackSpacing = 10;
            _levelSelectPanel.AddChild(stackPanel);

            var backToTitleButton = CreateButton();
            backToTitleButton.Text = "BACK";
            backToTitleButton.Anchor(Anchor.BottomLeft);
            backToTitleButton.X = 70;
            backToTitleButton.Y = -28;
            backToTitleButton.Click += BackToTitleButton_Clicked; ;
            _levelSelectPanel.AddChild(backToTitleButton);

            foreach (var level in levels)
            {
                var levelButton = CreateButton();
                levelButton.Text = level.Key;
                levelButton.Click += LevelButton_Clicked;

                stackPanel.AddChild(levelButton);
            }
        }

        private void BackToTitleButton_Clicked(object sender, EventArgs e)
        {
            if (OnBackToTitleButtonClicked != null)
                OnBackToTitleButtonClicked(this, EventArgs.Empty);
        }

        private void LevelButton_Clicked(object sender, EventArgs e)
        {
            if (OnLevelButtonClicked != null)
            {
                var button = (Button)sender;
                var levelName = button.Text;
                OnLevelButtonClicked(this, new LevelSelectedEventArgs(levelName));
            }
        }

        private void CreatePuzzleSovledPanel()
        {
            _puzzleSolvedPanel = new Panel();
            _puzzleSolvedPanel.Anchor(Anchor.Center);
            _puzzleSolvedPanel.Width = 640.0f;
            _puzzleSolvedPanel.WidthUnits = DimensionUnitType.Absolute;
            _puzzleSolvedPanel.Height = 480.0f;
            _puzzleSolvedPanel.HeightUnits = DimensionUnitType.Absolute;
            _puzzleSolvedPanel.IsVisible = false;
            _puzzleSolvedPanel.AddToRoot();

            var background = new ColoredRectangleRuntime();
            background.Color = Color.Gray * 0.5f;
            background.Dock(Dock.Fill);
            _puzzleSolvedPanel.AddChild(background);

            var solvedText = new TextRuntime();
            solvedText.Anchor(Anchor.Top);
            solvedText.Y = 20;
            solvedText.Text = "SOLVED";
            solvedText.UseCustomFont = true;
            solvedText.CustomFontFile = "fonts/megamax.fnt";
            solvedText.FontScale = 2.0f;
            _puzzleSolvedPanel.AddChild(solvedText);

            var backToLevelSelectButton = CreateButton();
            backToLevelSelectButton.Text = "BACK";
            backToLevelSelectButton.Anchor(Anchor.BottomLeft);
            backToLevelSelectButton.X = 10;
            backToLevelSelectButton.Y = -10;
            backToLevelSelectButton.Click += BackToLevelSelectButton_Clicked;
            _puzzleSolvedPanel.AddChild(backToLevelSelectButton);

            var nextPuzzleButton = CreateButton();
            nextPuzzleButton.Text = "NEXT";
            nextPuzzleButton.Anchor(Anchor.BottomRight);
            nextPuzzleButton.X = -10;
            nextPuzzleButton.Y = -10;
            nextPuzzleButton.Click += NextButton_Clicked; ;
            _puzzleSolvedPanel.AddChild(nextPuzzleButton);
        }

        private void CreatePuzzleFailedPanel()
        {
            _puzzleFailedPanel = new Panel();
            _puzzleFailedPanel.Anchor(Anchor.Center);
            _puzzleFailedPanel.Width = 640.0f;
            _puzzleFailedPanel.WidthUnits = DimensionUnitType.Absolute;
            _puzzleFailedPanel.Height = 480.0f;
            _puzzleFailedPanel.HeightUnits = DimensionUnitType.Absolute;
            _puzzleFailedPanel.IsVisible = false;
            _puzzleFailedPanel.AddToRoot();

            var background = new ColoredRectangleRuntime();
            background.Color = Color.Gray * 0.5f;
            background.Dock(Dock.Fill);
            _puzzleFailedPanel.AddChild(background);

            var failedText = new TextRuntime();
            failedText.Anchor(Anchor.Top);
            failedText.Y = 20;
            failedText.Text = "FAILED";
            failedText.UseCustomFont = true;
            failedText.CustomFontFile = "fonts/megamax.fnt";
            failedText.FontScale = 2.0f;
            _puzzleFailedPanel.AddChild(failedText);

            var backToLevelSelectButton = CreateButton();
            backToLevelSelectButton.Text = "BACK";
            backToLevelSelectButton.Anchor(Anchor.BottomLeft);
            backToLevelSelectButton.X = 10;
            backToLevelSelectButton.Y = -10;
            backToLevelSelectButton.Click += BackToLevelSelectButton_Clicked;
            _puzzleFailedPanel.AddChild(backToLevelSelectButton);

            var retryPuzzleButton = CreateButton();
            retryPuzzleButton.Text = "RETRY";
            retryPuzzleButton.Anchor(Anchor.BottomRight);
            retryPuzzleButton.X = -10;
            retryPuzzleButton.Y = -10;
            retryPuzzleButton.Click += RetryButton_Clicked; ;
            _puzzleFailedPanel.AddChild(retryPuzzleButton);
        }

        private void BackToLevelSelectButton_Clicked(object sender, EventArgs e)
        {
            if (OnBackToLevelSelectButtonClicked  != null)
                OnBackToLevelSelectButtonClicked(this, EventArgs.Empty);
        }

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            if (OnNextPuzzleButtonClicked != null)
                OnNextPuzzleButtonClicked(this, EventArgs.Empty);
        }

        private void RetryButton_Clicked(object sender, EventArgs e)
        {
            if (OnRetryPuzzleButtonClicked != null)
                OnRetryPuzzleButtonClicked(this, EventArgs.Empty);
        }

        private Button CreateButton()
        {
            var button = new Button();
            button.Width = 100;
            button.Height = 70;

            var buttonVisual = (ButtonVisual)button.Visual;
            buttonVisual.Height = 56;
            buttonVisual.HeightUnits = DimensionUnitType.Absolute;
            buttonVisual.Width = 80;
            buttonVisual.WidthUnits = DimensionUnitType.RelativeToChildren;

            var background = buttonVisual.Background;
            background.Color = Color.Black;

            var textInstance = buttonVisual.TextInstance;
            textInstance.Text = "START";
            textInstance.Color = Color.White;
            textInstance.UseCustomFont = true;
            textInstance.CustomFontFile = "fonts/megamax.fnt";
            textInstance.FontScale = 1.0f;
            textInstance.Anchor(Anchor.Center);
            textInstance.Width = 0;
            textInstance.WidthUnits = DimensionUnitType.RelativeToChildren;

            buttonVisual.ButtonCategory.ResetAllStates();

            //At the time of writing, I only really know
            //what the Enabled and Highlighted states do
            var enabledState = buttonVisual.States.Enabled;
            enabledState.Apply = () =>
            {
                background.Color = Color.Black;
                textInstance.Color = Color.White;
            };

            var focusedState = buttonVisual.States.Focused;
            focusedState.Apply = () =>
            {
                background.Color = Color.White;
                textInstance.Color = Color.Black;
            };

            var highlightedFocusedState = buttonVisual.States.HighlightedFocused;
            highlightedFocusedState.Apply = focusedState.Apply;

            var highlightedState = buttonVisual.States.Highlighted;
            highlightedState.Apply = focusedState.Apply;

            return button;
        }

        public void ShowTitlePanel()
        {
            _titlePanel.IsVisible = true;
            _levelSelectPanel.IsVisible = false;
            _puzzleSolvedPanel.IsVisible = false;
            _puzzleFailedPanel.IsVisible = false;
        }

        public void ShowLevelSelectPanel()
        {
            _levelSelectPanel.IsVisible = true;
            _titlePanel.IsVisible = false;
            _puzzleSolvedPanel.IsVisible = false;
            _puzzleFailedPanel.IsVisible = false;
        }

        public void ShowPuzzleSovledPanel()
        {
            _puzzleSolvedPanel.IsVisible = true;
            _titlePanel.IsVisible = false;
            _levelSelectPanel.IsVisible = false;
            _puzzleFailedPanel.IsVisible = false;
        }

        public void ShowPuzzleFailedPanel()
        {
            _puzzleFailedPanel.IsVisible = true;
            _titlePanel.IsVisible = false;
            _levelSelectPanel.IsVisible = false;
            _puzzleSolvedPanel.IsVisible = false;
        }

        public void HidePanels()
        {
            _titlePanel.IsVisible = false;
            _levelSelectPanel.IsVisible = false;
            _puzzleSolvedPanel.IsVisible = false;
            _puzzleFailedPanel.IsVisible = false;
        }
    }
}
