using System;
using System.Collections.Generic;
using System.Diagnostics;
using _2048.Model;
#if NETFX_CORE
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
#elif (WINDOWS_PHONE || NETFX_451)
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
#endif

namespace _2048
{
    public sealed partial class GameGrid
    {
        private const int _ROWS = 4;
        private const int _COLS = 4;

        private GameTile[][] _underlyingTiles;
        private GameModel _gameModel;

        private double GetTileSize()
        {
            return GameCanvas.ActualWidth / _ROWS;
        }

        public GameGrid()
        {
            this.InitializeComponent();

            this.SizeChanged += GameGrid_SizeChanged;

            _gameModel = new GameModel(_ROWS, _COLS);

            _underlyingTiles = new GameTile[_COLS][];

            for (int i = 0; i < _COLS; ++i)
            {
                _underlyingTiles[i] = new GameTile[_ROWS];
            }

            for (int y = 0; y < _ROWS; ++y)
            {
                for (int x = 0; x < _COLS; ++x)
                {
                    _underlyingTiles[x][y] = new GameTile(x, y);
                    //_underlyingTiles[x][y].Width = GetTileSize();
                    //_underlyingTiles[x][y].Height = GetTileSize();
                    //_underlyingTiles[x][y].SetValue(Canvas.LeftProperty, x * GetTileSize());
                    //_underlyingTiles[x][y].SetValue(Canvas.TopProperty, y * GetTileSize());
                    _underlyingTiles[x][y].SetValue(Canvas.ZIndexProperty, 0);
                    GameCanvas.Children.Add(_underlyingTiles[x][y]);
                }
            }

            StartGame();
        }

        private void GameGrid_SizeChanged(object Sender, SizeChangedEventArgs Args)
        {
            for (var y = 0; y < _ROWS; ++y)
            {
                for (var x = 0; x < _COLS; ++x)
                {
                    _underlyingTiles[x][y].Width = GetTileSize();
                    _underlyingTiles[x][y].Height = GetTileSize();
                    _underlyingTiles[x][y].SetValue(Canvas.LeftProperty, x * GetTileSize());
                    _underlyingTiles[x][y].SetValue(Canvas.TopProperty, y * GetTileSize());
                }
            }
        }
        
        private void LoadMap()
        {
            _gameModel.Cells[2][1] = new Cell(2, 1) { Value = 8 };
            _gameModel.Cells[2][2] = new Cell(2, 2) { Value = 4 };
            _gameModel.Cells[2][3] = new Cell(2, 3) { Value = 4 };
            _gameModel.Cells[3][2] = new Cell(3, 2) { Value = 8 };
            _gameModel.Cells[3][3] = new Cell(3, 3) { Value = 2 };
        }

        private void StartGame()
        {
            LoadMap();

            /*var first = new Tuple<int, int>(0, 0);//GetRandomEmptyTile();
            _gameModel.Cells[first.Item1][first.Item2].Value = GetRandomStartingNumber();
            _gameModel.Cells[first.Item1][first.Item2].WasCreated = true;

            /*var second = GetRandomEmptyTile();
            _gameModel.Cells[second.Item1][second.Item2].Value = GetRandomStartingNumber();
            _gameModel.Cells[second.Item1][second.Item2].WasCreated = true;*/

            UpdateUI();

            //Window.Current.CoreWindow.KeyDown += OnKeyDown;
            //this.ManipulationStarted += OnManipulationStarted;
            //this.ManipulationDelta += OnManipulationDelta;
            //this.ManipulationMode = ManipulationModes.All;
        }


        private void UpdateUI()
        {
            // Set to 0 any underlying tile where MovedFrom != null && !WasDoubled OR newValue == 0
            for (var y = 0; y < _ROWS; ++y)
            {
                for (var x = 0; x < _COLS; ++x)
                {
                    if ((_gameModel.Cells[x][y].MovedFrom != null && !_gameModel.Cells[x][y].WasDoubled) || _gameModel.Cells[x][y].Value == 0 || _gameModel.Cells[x][y].WasCreated)
                    {
                        _underlyingTiles[x][y].Value = 0;
                    }
                }
            }

            // For each tile where MovedFrom != null
            // Create a new temporary animation tile and animate to move to new location
            var storyboard = new Storyboard();
            var tempTiles = new List<GameTile>();
            for (var y = 0; y < _ROWS; ++y)
            {
                for (var x = 0; x < _COLS; ++x)
                {
                    if (_gameModel.Cells[x][y].MovedFrom != null)
                    {
                        var tempTile = new GameTile(x, y, true);
                        tempTile.Width = GetTileSize();
                        tempTile.Height = GetTileSize();
                        tempTile.SetValue(Canvas.ZIndexProperty, 1);
                        tempTiles.Add(tempTile);
                        GameCanvas.Children.Add(tempTile);

                        tempTile.Value = _gameModel.Cells[x][y].WasDoubled ? _gameModel.Cells[x][y].Value / 2 : _gameModel.Cells[x][y].Value;

                        var from = _gameModel.Cells[x][y].MovedFrom.Item1 * GetTileSize();
                        var to = x * GetTileSize();
                        var xAnimation = Animation.CreateDoubleAnimation(from, to, 1200000);

                        from = _gameModel.Cells[x][y].MovedFrom.Item2 * GetTileSize();
                        to = y * GetTileSize();
                        var yAnimation = Animation.CreateDoubleAnimation(from, to, 1200000);

                        Storyboard.SetTarget(xAnimation, tempTile);
                        Storyboard.SetTargetProperty(xAnimation, Animation.CreatePropertyPath("(Canvas.Left)"));

                        //((TransformGroup)RenderTransform).Children

                        Storyboard.SetTarget(yAnimation, tempTile);
                        Storyboard.SetTargetProperty(yAnimation, Animation.CreatePropertyPath("(Canvas.Top)"));

                        storyboard.Children.Add(xAnimation);
                        storyboard.Children.Add(yAnimation);
                    }
                }
            }

            storyboard.Completed += (Sender, O) =>
            {
                for (var y = 0; y < _ROWS; ++y)
                {
                    for (var x = 0; x < _COLS; ++x)
                    {
                        _underlyingTiles[x][y].Value = _gameModel.Cells[x][y].Value;
                    }
                }

                foreach (var tile in tempTiles)
                {
                    GameCanvas.Children.Remove(tile);
                }

                for (var y = 0; y < _ROWS; ++y)
                {
                    for (var x = 0; x < _COLS; ++x)
                    {
                        if (_gameModel.Cells[x][y].WasCreated)
                        {
                            _underlyingTiles[x][y].BeginNewTileAnimation();
                        }
                        else if (_gameModel.Cells[x][y].WasDoubled)
                        {
                            _underlyingTiles[x][y].SetValue(Canvas.ZIndexProperty, 100);
                            _underlyingTiles[x][y].BeginDoubledAnimation();
                        }

                        _gameModel.Cells[x][y].WasCreated = false;
                        _gameModel.Cells[x][y].WasDoubled = false;
                        _gameModel.Cells[x][y].MovedFrom = null;
                    }
                }

                Debug.WriteLine("ANIMATION COMPLETE");
                _moveInProgress = false;
            };

            storyboard.Begin();
        }

        private bool _moveInProgress;

        public void HandleMove(MoveDirection Direction)
        {
            if (_moveInProgress)
            {
                return;
            }

            _moveInProgress = true;

            if (_gameModel.PerformMove(Direction))
            {
                Debug.WriteLine("Starting move!");
                UpdateUI();
            }
            else
            {
                _moveInProgress = false;
            }
        }
    }
}
