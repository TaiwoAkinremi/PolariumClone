using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using PolariumClone.CustomData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PolariumClone.GameObjects
{
    public class GameBoard
    {
        private int _backBufferWidth;
        private int _backBufferHeight;

        private Sprite _blackGameTile;
        private Sprite _whiteGameTile;
        private Sprite _greyGameTile;
        private Sprite _selectedTileOverlay;

        private LevelData _level;
        
        private List<GameTile> _gameTiles;
        private List<GameTile> _selectedGameTiles;
        private List<GameTile> _posibleNextGameTiles;

        private GameState _state;

        private float _scale = 4.0f;
        private float _spriteSize = 16.0f;

        private SpriteFont _spriteFont;

        public EventHandler PuzzleSovled;
        public EventHandler PuzzleFailed;

        public GameBoard(
            GraphicsDevice graphicsDevice,
            Texture2DAtlas atlas,
            SpriteFont spriteFont,
            LevelData level)
        {
            _backBufferWidth = graphicsDevice.PresentationParameters.BackBufferWidth;
            _backBufferHeight = graphicsDevice.PresentationParameters.BackBufferHeight;
            
            _blackGameTile = atlas.CreateSprite("blackGameTile");
            _whiteGameTile = atlas.CreateSprite("whiteGameTile");
            _greyGameTile = atlas.CreateSprite("greyGameTile");
            _selectedTileOverlay = atlas.CreateSprite("selectedTileOverlay");

            _level = level;
            //sanitiy cheeck;
            var defindedLength = _level.Width * _level.Height;
            if (_level.Board.Length != defindedLength)
                throw new InvalidOperationException($"Expected Board length: {defindedLength}, but actual Board length: {_level.Board.Length}");

            _gameTiles = new List<GameTile>();

            SetupGameTiles();

            _selectedGameTiles = new List<GameTile>();
            _posibleNextGameTiles = new List<GameTile>();

            _state = GameState.Selecting;

            _spriteFont = spriteFont;
        }

        private void SetupGameTiles()
        {
            _gameTiles.Clear();
            
            var startDrawingXPos = (float)((_backBufferWidth * 0.5) - (_level.Width * 0.5 * _spriteSize * _scale));
            var startDrawingYPos = (float)((_backBufferHeight * 0.5) - (_level.Height * 0.5 * _spriteSize * _scale));
            var offsetXPos = 0.0f;
            var offsetYPos = 0.0f;

            for (int i = 0; i < _level.Board.Length; i++)
            {
                if (i > 0)
                {
                    if (i % _level.Width == 0)
                    {
                        offsetXPos = 0.0f;
                        offsetYPos += (float)(_spriteSize * _scale);

                    }
                    else
                        offsetXPos += (float)(_spriteSize * _scale);
                }

                var tileCoords = GetTileCoordsFromGameBoard(i);

                _gameTiles.Add(GenerateGameTile(
                    _level.Board[i],
                    new Vector2(startDrawingXPos + offsetXPos, startDrawingYPos + offsetYPos),
                    tileCoords.xInTiles,
                    tileCoords.yInTiles));
            }
        }

        private (int xInTiles, int yInTiles) GetTileCoordsFromGameBoard(int index)
        {
            var x = index % _level.Width;
            var y = index / _level.Width;

            return (x, y);
        }

        private GameTile GenerateGameTile(
            int gameTileIndex,
            Vector2 position,
            int xTileCoord,
            int yTileCoord) =>

            gameTileIndex switch
            {
                1 => new GameTile(
                    _blackGameTile,
                    _whiteGameTile,
                    _selectedTileOverlay,
                    GameTileType.Black,
                    GameTileType.White,
                    position,
                    new Vector2(_scale),
                    xTileCoord,
                    yTileCoord),
                
                2 => new GameTile(
                    _whiteGameTile,
                    _blackGameTile,
                    _selectedTileOverlay,
                    GameTileType.White,
                    GameTileType.Black,
                    position,
                    new Vector2(_scale),
                    xTileCoord,
                    yTileCoord),
                
                _ => new GameTile(
                    _greyGameTile,
                    null,
                    _selectedTileOverlay,
                    GameTileType.Grey,
                    null,
                    position,
                    new Vector2(_scale),
                    xTileCoord,
                    yTileCoord)
            };        

        public void Update(GameTime gameTime)
        {
            if (_state != GameState.Selecting)
                return;

            var mouseState = MouseExtended.GetState();

            var currentMousePosition = new Vector2(
                mouseState.Position.X,
                mouseState.Position.Y);

            if (mouseState.WasButtonPressed(MouseButton.Left))
            {
                _selectedGameTiles.Clear();
                _posibleNextGameTiles.Clear();

                CalculateFirstSelectedTile(currentMousePosition);
            }
            else if (mouseState.IsButtonDown(MouseButton.Left))
            {
                CheckIfNextPossibleTileSelected(currentMousePosition);
            }            
            else if (mouseState.WasButtonReleased(MouseButton.Left))
            {
                //Mouse button was let go so we proceed to the flipping
                if (!_selectedGameTiles.Any())
                    return;

                _state = GameState.SelectionComplete;
                foreach (var tile in _gameTiles)
                {
                    tile.SetState(GameState.SelectionComplete);
                    tile.IsSelected = false;                    
                }

                CheckIfPuzzleSolved();
            }
        }

        private void CalculateFirstSelectedTile(Vector2 currentMousePosition)
        {
            GameTile firstSelectedTile = null;
            var firstSelectedTileIndex = 0;

            for (int i = 0; i < _gameTiles.Count; i++)
            {
                var tile = _gameTiles[i];
                if (tile.Bounds.Contains(currentMousePosition))
                {
                    firstSelectedTile = tile;
                    firstSelectedTileIndex = i;
                }
            }

            //The button was pressed but not over a tile;
            if (firstSelectedTile == null)
                return;

            SelectTileAndCalculatePossibleNextTilesPosition(firstSelectedTile);
        }
        
        private void SelectTileAndCalculatePossibleNextTilesPosition(GameTile gameTile)
        {
            gameTile.IsSelected = true;
            _selectedGameTiles.Add(gameTile);

            int newXTileCoord = 0;
            int newYTileCoord = 0;

            //Up
            newXTileCoord = gameTile.GameBoardXPosition;
            newYTileCoord = gameTile.GameBoardYPosition - 1;
            FillInPossibleNextGameTiles(newXTileCoord, newYTileCoord);
            //Down
            newXTileCoord = gameTile.GameBoardXPosition;
            newYTileCoord = gameTile.GameBoardYPosition + 1;
            FillInPossibleNextGameTiles(newXTileCoord, newYTileCoord);
            //Left
            newXTileCoord = gameTile.GameBoardXPosition - 1;
            newYTileCoord = gameTile.GameBoardYPosition;
            FillInPossibleNextGameTiles(newXTileCoord, newYTileCoord);
            //Right
            newXTileCoord = gameTile.GameBoardXPosition + 1;
            newYTileCoord = gameTile.GameBoardYPosition;
            FillInPossibleNextGameTiles(newXTileCoord, newYTileCoord);
        }

        private void FillInPossibleNextGameTiles(int xTileCoord, int yTileCoord)
        {
            if (xTileCoord < 0 ||
                xTileCoord > _level.Width - 1 ||
                yTileCoord < 0 ||
                yTileCoord > _level.Height - 1)
                return;

            var index = (yTileCoord * _level.Width) + xTileCoord;
            _posibleNextGameTiles.Add(_gameTiles[index]);
        }

        private void CheckIfNextPossibleTileSelected(Vector2 currentMousePosition)
        {
            if (!_selectedGameTiles.Any())
                return;

            var lastSelectedTile = _selectedGameTiles.Last();
            if (lastSelectedTile.Bounds.Contains(currentMousePosition))
                return;

            var nextPossibleTile = _posibleNextGameTiles.SingleOrDefault(
                gm => gm.Bounds.Contains(currentMousePosition));

            //Mouse is not under a tile
            if (nextPossibleTile == null)
                return;

            _posibleNextGameTiles.Clear();
            SelectTileAndCalculatePossibleNextTilesPosition(nextPossibleTile);
        }

        private void CheckIfPuzzleSolved()
        {
            var isPuzzleSolved = true;

            for (int yInTiles = 1; yInTiles < _level.Height - 1; yInTiles++)
            {
                //Plus one because we want to ignore the first tile in a row
                //because it's alway going to be grey
                var startIndex = (yInTiles * _level.Width) + 1;
                //Minus two because we want to ingore the last tile in a row
                //plus we compare the i and i+1 tile so we don't need to go
                //all the way to the minus one as one would expect.
                var endIndex = (yInTiles * _level.Width) + (_level.Width - 2);

                if (!AreAllTilesInRowTheSame(startIndex, endIndex))
                    isPuzzleSolved = false;
            }

            if (isPuzzleSolved && PuzzleSovled != null)
            {
                PuzzleSovled(this, new EventArgs());
            }
            else if (PuzzleFailed != null)
            {
                PuzzleFailed(this, new EventArgs());
            }
        }

        private bool AreAllTilesInRowTheSame(int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                if (_gameTiles[i].GameTileType != _gameTiles[i + 1].GameTileType)
                    return false;
            }

            return true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {           
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            spriteBatch.DrawString(
                _spriteFont,
                _level.Name,
                new Vector2(20, 20),
                Color.White);

            foreach (var tile in _gameTiles)
            {
                tile.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();
        }

        public void Reset()
        {
            _state = GameState.Selecting;
            _selectedGameTiles.Clear();
            _posibleNextGameTiles.Clear();

            foreach (var tile in _gameTiles)
                tile.Reset();
        }

        public void ChangeLevel(LevelData newLevel)
        {
            _level = newLevel;
            SetupGameTiles();
            Reset();
        }
    }
}
