using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace _2048
{
    class Cell
    {
        public int Value { get; set; }
        public bool WasDoubled { get; set; }
        public bool WasCreated { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public GameTile GameTile { get; set; }
        public Tuple<int, int> MovedFrom { get; set; }

        public Cell(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            this.Value = 0;
        }

        public bool IsEmpty()
        {
            return Value == 0;
        }

        public void UpdateUI(Canvas GameGrid, GameTile UnderlyingTile)
        {
            if (GameTile != null)
            {
                GameGrid.Children.Remove(GameTile);
            }

            if (Value == 0)
            {
                return;
            }

            this.GameTile = new GameTile(true);
            this.GameTile.Width = 150;
            this.GameTile.Height = 150;
            GameGrid.Children.Add(this.GameTile);
            this.GameTile.SetValue(Canvas.ZIndexProperty, 1);
            
            if (MovedFrom != null)
            {
                //this.GameTile.SetValue(Canvas.LeftProperty, X * 150);
                //this.GameTile.SetValue(Canvas.TopProperty, Y * 150);
                BeginMovedTileAnimation(UnderlyingTile);
            }
            else
            {
                this.GameTile.SetValue(Canvas.LeftProperty, X * 150);
                this.GameTile.SetValue(Canvas.TopProperty, Y * 150);
            }

            this.GameTile.Value = Value;
            
            if (WasCreated)
            {
                GameTile.BeginNewTileAnimation();
            }

            if (WasDoubled)
            {
                GameTile.BeginDoubledAnimation();
            }
            
            WasCreated = false;
            WasDoubled = false;
            MovedFrom = null;
        }

        public void BeginMovedTileAnimation(GameTile UnderlyingTile)
        {
            var xAnimation = new DoubleAnimation();
            xAnimation.EnableDependentAnimation = true;
            xAnimation.From = MovedFrom.Item1 * 150;
            xAnimation.To = X * 150;
            xAnimation.Duration = new Duration(new TimeSpan(12000000));

            var yAnimation = new DoubleAnimation();
            yAnimation.EnableDependentAnimation = true;
            yAnimation.From = MovedFrom.Item2 * 150;
            yAnimation.To = Y * 150;
            yAnimation.Duration = new Duration(new TimeSpan(12000000));
            
            Storyboard.SetTarget(xAnimation, GameTile);
            Storyboard.SetTargetProperty(xAnimation, "(Canvas.Left)");

            //((TransformGroup)RenderTransform).Children

            Storyboard.SetTarget(yAnimation, GameTile);
            Storyboard.SetTargetProperty(yAnimation, "(Canvas.Top)");

            var storyboard = new Storyboard();
            storyboard.Children.Add(xAnimation);
            storyboard.Children.Add(yAnimation);

            storyboard.Completed += (Sender, O) => UnderlyingTile.Value = 0;

            storyboard.Begin();
        }
    }



    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private const int _ROWS = 4;
        private const int _COLS = 4;

        private GameTile[][] _underlyingTiles;
        private Cell[][] _cells;

        public MainPage()
        {
            this.InitializeComponent();
            
            _underlyingTiles = new GameTile[_COLS][];

            for (int i = 0; i < _COLS; ++i)
            {
                _underlyingTiles[i] = new GameTile[_ROWS];
            }

            for (int y = 0; y < _ROWS; ++y)
            {
                for (int x = 0; x < _COLS; ++x)
                {
                    _underlyingTiles[x][y] = new GameTile();
                    _underlyingTiles[x][y].Width = 150;
                    _underlyingTiles[x][y].Height = 150;
                    _underlyingTiles[x][y].SetValue(Canvas.LeftProperty, x * 150);
                    _underlyingTiles[x][y].SetValue(Canvas.TopProperty, y * 150);
                    _underlyingTiles[x][y].SetValue(Canvas.ZIndexProperty, 0);
                    GameGrid.Children.Add(_underlyingTiles[x][y]);
                }
            }

            _cells = new Cell[_COLS][];
            for (int i = 0; i < _COLS; ++i)
            {
                _cells[i] = new Cell[_ROWS];
            }

            for (int y = 0; y < _ROWS; ++y)
            {
                for (int x = 0; x < _COLS; ++x)
                {
                    _cells[x][y] = new Cell(x, y);
                }
            }

            StartGame();
        }

        private void LoadMap()
        {
            _cells[2][1] = new Cell(2, 1) { Value = 8 };
            _cells[2][2] = new Cell(2, 2) { Value = 4 };
            _cells[2][3] = new Cell(2, 3) { Value = 4 };
            _cells[3][2] = new Cell(3, 2) { Value = 8 };
            _cells[3][3] = new Cell(3, 3) { Value = 2 };
        }

        private void StartGame()
        {
            LoadMap();

            /*var first = new Tuple<int, int>(0, 0);//GetRandomEmptyTile();
            _cells[first.Item1][first.Item2].Value = GetRandomStartingNumber();
            _cells[first.Item1][first.Item2].WasCreated = true;

            /*var second = GetRandomEmptyTile();
            _cells[second.Item1][second.Item2].Value = GetRandomStartingNumber();
            _cells[second.Item1][second.Item2].WasCreated = true;*/

            UpdateUI(true);

            Window.Current.CoreWindow.KeyDown += OnKeyDown;
            this.ManipulationStarted += OnManipulationStarted;
            this.ManipulationDelta += OnManipulationDelta;
            this.ManipulationMode = ManipulationModes.All;
        }

        private void OnManipulationDelta(object Sender, ManipulationDeltaRoutedEventArgs DeltaRoutedEventArgs)
        {
            if (DeltaRoutedEventArgs.IsInertial)
            {
                if (startPoint.X - DeltaRoutedEventArgs.Position.X > 200)
                {
                    HandleMove(VirtualKey.Left);
                    DeltaRoutedEventArgs.Complete();
                    DeltaRoutedEventArgs.Handled = true;
                }
                else if (DeltaRoutedEventArgs.Position.X - startPoint.X > 200)
                {
                    HandleMove(VirtualKey.Right);
                    DeltaRoutedEventArgs.Complete();
                    DeltaRoutedEventArgs.Handled = true;
                }
                else if (startPoint.Y - DeltaRoutedEventArgs.Position.Y > 200)
                {
                    HandleMove(VirtualKey.Up);
                    DeltaRoutedEventArgs.Complete();
                    DeltaRoutedEventArgs.Handled = true;
                }
                else if (DeltaRoutedEventArgs.Position.Y - startPoint.Y > 200)
                {
                    HandleMove(VirtualKey.Down);
                    DeltaRoutedEventArgs.Complete();
                    DeltaRoutedEventArgs.Handled = true;
                }
            }
        }

        private Point startPoint;

        private void OnManipulationStarted(object Sender, ManipulationStartedRoutedEventArgs StartedRoutedEventArgs)
        {
            startPoint = StartedRoutedEventArgs.Position;
        }


        private void UpdateUI(bool FirstDraw = false)
        {
            // Update tile map
            //for (var y = 0; y < _ROWS; ++y)
            //{
            //    for (var x = 0; x < _COLS; ++x)
            //    {
            //        var p = _cells[x][y].MovedFrom;
            //        if (p != null)
            //        {
            //            if (_cells[x][y].GameTile != null)
            //            {
            //                //GameGrid.Children.Remove(_cells[x][y].GameTile);
            //            }
            //            _cells[x][y].GameTile = _cells[p.Item1][p.Item2].GameTile;
            //            _cells[p.Item1][p.Item2].GameTile = null;
            //        }
            //    }
            //}

            // Set to 0 any underlying tile where MovedFrom != null && !WasDoubled OR newValue == 0
            for (var y = 0; y < _ROWS; ++y)
            {
                for (var x = 0; x < _COLS; ++x)
                {
                    if ((_cells[x][y].MovedFrom != null && !_cells[x][y].WasDoubled) || _cells[x][y].Value == 0 || _cells[x][y].WasCreated)
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
                    if (_cells[x][y].MovedFrom != null)
                    {
                        var tempTile = new GameTile(true);
                        tempTile.Width = 150;
                        tempTile.Height = 150;
                        tempTile.SetValue(Canvas.ZIndexProperty, 1);
                        tempTiles.Add(tempTile);
                        GameGrid.Children.Add(tempTile);

                        tempTile.Value = _cells[x][y].WasDoubled ? _cells[x][y].Value / 2 : _cells[x][y].Value;

                        var xAnimation = new DoubleAnimation();
                        xAnimation.EnableDependentAnimation = true;
                        xAnimation.From = _cells[x][y].MovedFrom.Item1 * 150;
                        xAnimation.To = x * 150;
                        xAnimation.Duration = new Duration(new TimeSpan(1200000));

                        var yAnimation = new DoubleAnimation();
                        yAnimation.EnableDependentAnimation = true;
                        yAnimation.From = _cells[x][y].MovedFrom.Item2 * 150;
                        yAnimation.To = y * 150;
                        yAnimation.Duration = new Duration(new TimeSpan(1200000));

                        Storyboard.SetTarget(xAnimation, tempTile);
                        Storyboard.SetTargetProperty(xAnimation, "(Canvas.Left)");

                        //((TransformGroup)RenderTransform).Children

                        Storyboard.SetTarget(yAnimation, tempTile);
                        Storyboard.SetTargetProperty(yAnimation, "(Canvas.Top)");

                        storyboard.Children.Add(xAnimation);
                        storyboard.Children.Add(yAnimation);
                    }
                }
            }

            storyboard.Completed += (Sender, O) => {
                for (var y = 0; y < _ROWS; ++y)
                {
                    for (var x = 0; x < _COLS; ++x)
                    {
                        _underlyingTiles[x][y].Value = _cells[x][y].Value;
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
                        if (_cells[x][y].WasCreated)
                        {
                            _underlyingTiles[x][y].BeginNewTileAnimation();
                        }
                        else if (_cells[x][y].WasDoubled)
                        {
                            _underlyingTiles[x][y].SetValue(Canvas.ZIndexProperty, 100);
                            _underlyingTiles[x][y].BeginDoubledAnimation();
                        }

                        _cells[x][y].WasCreated = false;
                        _cells[x][y].WasDoubled = false;
                        _cells[x][y].MovedFrom = null;
                    }
                }
            };

            storyboard.Begin();

            /*for (var y = 0; y < _ROWS; ++y)
            {
                for (var x = 0; x < _COLS; ++x)
                {
                    _cells[x][y].UpdateUI(GameGrid, _underlyingTiles[x][y]);
                }
            }*/
        }
        
        private void OnKeyDown(CoreWindow Sender, KeyEventArgs Args)
        {
            if (Args.VirtualKey != VirtualKey.Up && Args.VirtualKey != VirtualKey.Down && Args.VirtualKey != VirtualKey.Left && Args.VirtualKey != VirtualKey.Right)
            {
                return;
            }

            HandleMove(Args.VirtualKey);
        }

        private void HandleMove(VirtualKey Key)
        {
            if (PackTiles(Key) | MergeTiles(Key))
            {
                var newTile = GetRandomEmptyTile();

                if (newTile != null)
                {
                    _cells[newTile.Item1][newTile.Item2].Value = GetRandomStartingNumber();
                    if (_cells[newTile.Item1][newTile.Item2].MovedFrom != null)
                    {
                        Debugger.Break();
                    }
                    _cells[newTile.Item1][newTile.Item2].WasCreated = true;
                }
                else
                {
                    // Game over?
                }

                UpdateUI();
            }
        }

        private bool PackTiles(VirtualKey MoveDirection)
        {
            var changed = false;
            if (MoveDirection == VirtualKey.Up)
            {
                for (var x = 0; x < _COLS; ++x)
                {
                    var lastEmptyY = 0;

                    while (lastEmptyY < _ROWS && !_cells[x][lastEmptyY].IsEmpty())
                    {
                        ++lastEmptyY;
                    }

                    var currentY = lastEmptyY + 1;
                    while (currentY < _ROWS)
                    {
                        if (!_cells[x][currentY].IsEmpty())
                        {
                            changed = true;
                            _cells[x][lastEmptyY].Value = _cells[x][currentY].Value;
                            _cells[x][lastEmptyY].MovedFrom = _cells[x][currentY].MovedFrom ?? new Tuple<int, int>(x, currentY);
                            _cells[x][currentY].Value = 0;
                            _cells[x][currentY].MovedFrom = null;
                            ++lastEmptyY;
                        }
                        ++currentY;
                    }
                }
            }
            else if (MoveDirection == VirtualKey.Down)
            {
                for (var x = 0; x < _COLS; ++x)
                {
                    var lastEmptyY = _ROWS - 1;

                    while (lastEmptyY >= 0 && !_cells[x][lastEmptyY].IsEmpty())
                    {
                        --lastEmptyY;
                    }

                    var currentY = lastEmptyY - 1;
                    while (currentY >= 0)
                    {
                        if (!_cells[x][currentY].IsEmpty())
                        {
                            changed = true;
                            _cells[x][lastEmptyY].Value = _cells[x][currentY].Value;
                            _cells[x][lastEmptyY].MovedFrom = _cells[x][currentY].MovedFrom ?? new Tuple<int, int>(x, currentY);
                            _cells[x][currentY].Value = 0;
                            _cells[x][currentY].MovedFrom = null;
                            --lastEmptyY;
                        }
                        --currentY;
                    }
                }
            }
            else if (MoveDirection == VirtualKey.Left)
            {
                for (var y = 0; y < _ROWS; ++y)
                {
                    var lastEmptyX = 0;

                    while (lastEmptyX < _COLS && !_cells[lastEmptyX][y].IsEmpty())
                    {
                        ++lastEmptyX;
                    }

                    var currentX = lastEmptyX + 1;
                    while (currentX < _COLS)
                    {
                        if (_cells[currentX][y].Value > 0)
                        {
                            changed = true;
                            _cells[lastEmptyX][y].Value = _cells[currentX][y].Value;
                            _cells[lastEmptyX][y].MovedFrom = _cells[currentX][y].MovedFrom ?? new Tuple<int, int>(currentX, y);
                            _cells[currentX][y].Value = 0;
                            _cells[currentX][y].MovedFrom = null;
                            ++lastEmptyX;
                        }
                        ++currentX;
                    }
                }
            }
            else if (MoveDirection == VirtualKey.Right)
            {

                for (var y = 0; y < _ROWS; ++y)
                {
                    var lastEmptyX = _COLS - 1;

                    while (lastEmptyX >= 0 && !_cells[lastEmptyX][y].IsEmpty())
                    {
                        --lastEmptyX;
                    }

                    var currentX = lastEmptyX - 1;
                    while (currentX >= 0)
                    {
                        if (_cells[currentX][y].Value > 0)
                        {
                            changed = true;
                            _cells[lastEmptyX][y].Value = _cells[currentX][y].Value;
                            _cells[lastEmptyX][y].MovedFrom = _cells[currentX][y].MovedFrom ?? new Tuple<int, int>(currentX, y);
                            _cells[currentX][y].Value = 0;
                            _cells[currentX][y].MovedFrom = null;
                            --lastEmptyX;
                        }
                        --currentX;
                    }
                }
            }
            return changed;
        }

        private bool MergeTiles(VirtualKey MoveDirection)
        {
            var changed = false;
            if (MoveDirection == VirtualKey.Up)
            {
                for (var x = 0; x < _COLS; ++x)
                {
                    for (var y = 0; y < _ROWS - 1 && _cells[x][y].Value > 0; ++y)
                    {
                        if (_cells[x][y].Value == _cells[x][y + 1].Value)
                        {
                            changed = true;
                            _cells[x][y].Value *= 2;
                            _cells[x][y].WasDoubled = true;
                            _cells[x][y].MovedFrom = _cells[x][y + 1].MovedFrom ?? new Tuple<int, int>(x, y + 1);
                            _cells[x][y + 1].MovedFrom = null;
                            PackTiles(MoveDirection);
                        }
                    }
                }
            }
            else if (MoveDirection == VirtualKey.Down)
            {
                for (var x = 0; x < _COLS; ++x)
                {
                    for (var y = _ROWS - 1; y >= 1 && _cells[x][y].Value > 0; --y)
                    {
                        if (_cells[x][y].Value == _cells[x][y - 1].Value)
                        {
                            changed = true;
                            _cells[x][y].Value *= 2;
                            _cells[x][y].WasDoubled = true;
                            _cells[x][y].MovedFrom = _cells[x][y - 1].MovedFrom ?? new Tuple<int, int>(x, y - 1);
                            _cells[x][y - 1].Value = 0;
                            _cells[x][y - 1].MovedFrom = null;
                            PackTiles(MoveDirection);
                        }
                    }
                }
            }
            else if (MoveDirection == VirtualKey.Left)
            {
                for (var y = 0; y < _ROWS; ++y)
                {
                    for (var x = 0; x < _COLS - 1 && _cells[x][y].Value > 0; ++x)
                    {
                        if (_cells[x][y].Value == _cells[x + 1][y].Value)
                        {
                            changed = true;
                            _cells[x][y].Value *= 2;
                            _cells[x][y].WasDoubled = true;
                            _cells[x][y].MovedFrom = _cells[x + 1][y].MovedFrom ?? new Tuple<int, int>(x + 1, y);
                            _cells[x + 1][y].Value = 0;
                            _cells[x + 1][y].MovedFrom = null;
                            PackTiles(MoveDirection);
                        }
                    }
                }
            }
            else if (MoveDirection == VirtualKey.Right)
            {
                for (var y = 0; y < _ROWS; ++y)
                {
                    for (var x = _COLS - 1; x >= 1 && _cells[x][y].Value > 0; --x)
                    {
                        if (_cells[x][y].Value == _cells[x - 1][y].Value)
                        {
                            changed = true;
                            _cells[x][y].Value *= 2;
                            _cells[x][y].WasDoubled = true;
                            _cells[x][y].MovedFrom = _cells[x - 1][y].MovedFrom ?? new Tuple<int, int>(x - 1, y);
                            _cells[x - 1][y].Value = 0;
                            _cells[x - 1][y].MovedFrom = null;
                            PackTiles(MoveDirection);
                        }
                    }
                }
            }

            return changed;
        }

        private Random rnd = new Random();
        private Tuple<int, int> GetRandomEmptyTile()
        {
            var emptyIndices = new List<Tuple<int, int>>();
            for (int y = 0; y < _ROWS; ++y)
            {
                for (int x = 0; x < _COLS; ++x)
                {
                    if (_cells[x][y].IsEmpty())
                    {
                        emptyIndices.Add(new Tuple<int, int>(x,y));
                    }
                }
            }

            if (emptyIndices.Count == 0)
            {
                return null;
            }

            var next = rnd.Next(0, emptyIndices.Count - 1);
            return emptyIndices[next];
        }

        private int GetRandomStartingNumber()
        {
            return rnd.NextDouble() < 0.75 ? 2 : 4;
        }
    }
}
