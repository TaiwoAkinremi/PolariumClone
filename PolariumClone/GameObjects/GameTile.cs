using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;

namespace PolariumClone.GameObjects
{
    public class GameTile
    {
        //required. The Sprite that will be displayed
        private Sprite _primaryTileSprite;
        //optional. The Sprite that will be 'tweened' to after entering the 'tweened' state
        private Sprite _secondaryTileSprite;
        //required. The Sprite that is shown on top of the primary Sprite whn IsSelected = true;
        private Sprite _selectedTileOverlySprite;

        //Either Black, White or Grey
        private GameTileType _primaryTileType;
        private GameTileType? _secondaryTileType;

        private GameState _state;
        private bool _shouldFlip;

        private Vector2 _position;
        private Vector2 _scale;
        
        public int GameBoardXPosition { get; private set; }
        public int GameBoardYPosition { get; private set; }
        public GameTileType GameTileType { get; private set; }
        public RectangleF Bounds { get; private set; }
        
        public bool IsSelected { get; set; }

        public GameTile(
            Sprite primaryTileSprite,
            Sprite secondaryTileSprite,
            Sprite selectedTileOverly,
            GameTileType primaryTileType,
            GameTileType? secondaryTileType,
            Vector2 position,
            Vector2 scale,
            int gameBoardXPosition,
            int gameBoardYPositon)
        {
            _primaryTileSprite = primaryTileSprite;
            _secondaryTileSprite = secondaryTileSprite;
            _selectedTileOverlySprite = selectedTileOverly;

            _primaryTileType = primaryTileType;
            _secondaryTileType = secondaryTileType;
            
            _position = position;
            _scale = scale;

            GameBoardXPosition = gameBoardXPosition;
            GameBoardYPosition = gameBoardYPositon;

            Bounds = new RectangleF(
                position.X,
                position.Y,
                _primaryTileSprite.Size.X * _scale.X,
                _primaryTileSprite.Size.Y * _scale.Y);

            GameTileType = _primaryTileType;

            _state = GameState.Selecting;
            _shouldFlip = false;
        }

        public void SetState(GameState state)
        { 
            _state = state;
            _shouldFlip = IsSelected && _secondaryTileSprite != null;

            if (_state == GameState.SelectionComplete &&
                _shouldFlip &&
                _secondaryTileType.HasValue)
                GameTileType = _secondaryTileType.Value;
        }

        public void Update(GameTime gameTime)
        {
            
        }
        
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_state == GameState.Selecting ||
                !_shouldFlip)
            {
                spriteBatch.Draw(
                    _primaryTileSprite,
                    _position,
                    0.0f,
                    _scale);

                if (IsSelected)
                    spriteBatch.Draw(
                    _selectedTileOverlySprite,
                    _position,
                    0.0f,
                    _scale);
            }
            else if (_state == GameState.SelectionComplete &&
                _shouldFlip)
            {
                spriteBatch.Draw(
                    _secondaryTileSprite,
                    _position,
                    0.0f,
                    _scale);
            }            
        }

        public void Reset()
        {
            _state = GameState.Selecting;
            _shouldFlip = false;
            GameTileType = _primaryTileType;
        }
    }
}
