using Gum.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGameGum;
using PolariumClone.Screens;
using PolariumClone.UI;

namespace PolariumClone
{
    public class PolariumGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly ScreenManager _screenManager;

        public PolariumUIManager UIManager { get; private set; }

        public PolariumGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;            
            _graphics.IsFullScreen = false;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _screenManager = new ScreenManager();
            Components.Add(_screenManager);

            UIManager = new PolariumUIManager();
        }

        protected override void Initialize()
        {
            InitializeGum();
            base.Initialize();
            _screenManager.ShowScreen(new TitleScreen(this));           
        }

        private void InitializeGum()
        {
            GumService.Default.Initialize(this, DefaultVisualsVersion.V3);
            GumService.Default.ContentLoader.XnaContentManager = Content;
            GumService.Default.CanvasWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            GumService.Default.CanvasHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            GumService.Default.Root.Children.Clear();
        }
    }
}
