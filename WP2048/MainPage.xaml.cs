using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Windows.System;
using Windows.UI.Core;
using Microsoft.Phone.Controls;
using _2048;
using KeyEventArgs = Windows.UI.Core.KeyEventArgs;

namespace WP2048
{
    public partial class MainPage : PhoneApplicationPage
    {
        private const int _ROWS = 4;
        private const int _COLS = 4;

        private GameTile[][] _underlyingTiles;
        private GameModel _gameModel;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _gameModel = new GameModel(_ROWS, _COLS);

            _underlyingTiles = new GameTile[_COLS][];

            for (int i = 0; i < _COLS; ++i)
            {
                _underlyingTiles[i] = new GameTile[_ROWS];
            }

            for (var y = 0; y < _ROWS; ++y)
            {
                for (var x = 0; x < _COLS; ++x)
                {
                    _underlyingTiles[x][y] = new GameTile(_gameModel, x, y);
                    _underlyingTiles[x][y].Width = 100;
                    _underlyingTiles[x][y].Height = 100;
                    _underlyingTiles[x][y].SetValue(Canvas.LeftProperty, (double)x * 100);
                    _underlyingTiles[x][y].SetValue(Canvas.TopProperty, (double)y * 100);
                    _underlyingTiles[x][y].SetValue(Canvas.ZIndexProperty, 0);
                    GameGrid.Children.Add(_underlyingTiles[x][y]);
                }
            }

            StartGame();
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

            this.ManipulationStarted += OnManipulationStarted;
            this.ManipulationDelta += OnManipulationDelta;
           
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
                        var tempTile = new GameTile(null, x, y, true);
                        tempTile.Width = 100;
                        tempTile.Height = 100;
                        tempTile.SetValue(Canvas.ZIndexProperty, 1);
                        tempTiles.Add(tempTile);
                        GameGrid.Children.Add(tempTile);

                        tempTile.Value = _gameModel.Cells[x][y].WasDoubled ? _gameModel.Cells[x][y].Value / 2 : _gameModel.Cells[x][y].Value;

                        var xAnimation = new DoubleAnimation();
                        xAnimation.From = _gameModel.Cells[x][y].MovedFrom.Item1 * 100;
                        xAnimation.To = x * 100;
                        xAnimation.Duration = new Duration(new TimeSpan(1200000));

                        var yAnimation = new DoubleAnimation();
                        yAnimation.From = _gameModel.Cells[x][y].MovedFrom.Item2 * 100;
                        yAnimation.To = y * 100;
                        yAnimation.Duration = new Duration(new TimeSpan(1200000));

                        Storyboard.SetTarget(xAnimation, tempTile);
                        Storyboard.SetTargetProperty(xAnimation, new PropertyPath("(Canvas.Left)"));

                        //((TransformGroup)RenderTransform).Children

                        Storyboard.SetTarget(yAnimation, tempTile);
                        Storyboard.SetTargetProperty(yAnimation, new PropertyPath("(Canvas.Top)"));

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
                    GameGrid.Children.Remove(tile);
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

        private void OnManipulationDelta(object Sender, ManipulationDeltaEventArgs DeltaRoutedEventArgs)
        {
            //if (DeltaRoutedEventArgs.IsInertial)
            {
                Debug.WriteLine(DeltaRoutedEventArgs.CumulativeManipulation.Translation.ToString());
                if (DeltaRoutedEventArgs.CumulativeManipulation.Translation.X < -30)
                {
                    HandleMove(MoveDirection.Left);
                    DeltaRoutedEventArgs.Complete();
                    DeltaRoutedEventArgs.Handled = true;
                }
                else if (DeltaRoutedEventArgs.CumulativeManipulation.Translation.X > 30)
                {
                    HandleMove(MoveDirection.Right);
                    DeltaRoutedEventArgs.Complete();
                    DeltaRoutedEventArgs.Handled = true;
                }
                else if (DeltaRoutedEventArgs.CumulativeManipulation.Translation.Y < -30)
                {
                    HandleMove(MoveDirection.Up);
                    DeltaRoutedEventArgs.Complete();
                    DeltaRoutedEventArgs.Handled = true;
                }
                else if (DeltaRoutedEventArgs.CumulativeManipulation.Translation.Y > 30)
                {
                    HandleMove(MoveDirection.Down);
                    DeltaRoutedEventArgs.Complete();
                    DeltaRoutedEventArgs.Handled = true;
                }
            }
        }

        private Point _manipulationStartPoint;

        private void OnManipulationStarted(object Sender, ManipulationStartedEventArgs ManipulationStartedEventArgs)
        {
            _manipulationStartPoint = ManipulationStartedEventArgs.ManipulationOrigin;
        }

        private bool _moveInProgress;

        private void HandleMove(MoveDirection Direction)
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