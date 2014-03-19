using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using _2048.Model;

namespace _2048
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private const int _MAX_GRID_SIZE = 600;

        private GameGrid _gameGrid;

        public MainPage()
        {
            this.InitializeComponent();

            _gameGrid = new GameGrid();

            ContentGrid.Children.Add(_gameGrid);

            this.SizeChanged += MainPage_SizeChanged;

            Window.Current.CoreWindow.KeyDown += CoreWindowOnKeyDown;
            this.ManipulationMode = ManipulationModes.All;
            this.ManipulationStarted += OnManipulationStarted;
            this.ManipulationDelta += OnManipulationDelta;
        }

        private void CoreWindowOnKeyDown(CoreWindow Sender, KeyEventArgs Args)
        {
            MoveDirection? direction = null;
            if (Args.VirtualKey == VirtualKey.Up)
            {
                direction = MoveDirection.Up;
            }
            else if (Args.VirtualKey == VirtualKey.Down)
            {
                direction = MoveDirection.Down;
            }
            else if (Args.VirtualKey == VirtualKey.Left)
            {
                direction = MoveDirection.Left;
            }
            else if (Args.VirtualKey == VirtualKey.Right)
            {
                direction = MoveDirection.Right;
            }

            if (direction != null)
            {
                _gameGrid.HandleMove(direction.Value);
            }
        }


        private void OnManipulationDelta(object Sender, ManipulationDeltaRoutedEventArgs DeltaRoutedEventArgs)
        {
            if (DeltaRoutedEventArgs.IsInertial)
            {
                if (_manipulationStartPoint.X - DeltaRoutedEventArgs.Position.X > 200)
                {
                    _gameGrid.HandleMove(MoveDirection.Left);
                    DeltaRoutedEventArgs.Complete();
                    DeltaRoutedEventArgs.Handled = true;
                }
                else if (DeltaRoutedEventArgs.Position.X - _manipulationStartPoint.X > 200)
                {
                    _gameGrid.HandleMove(MoveDirection.Right);
                    DeltaRoutedEventArgs.Complete();
                    DeltaRoutedEventArgs.Handled = true;
                }
                else if (_manipulationStartPoint.Y - DeltaRoutedEventArgs.Position.Y > 200)
                {
                    _gameGrid.HandleMove(MoveDirection.Up);
                    DeltaRoutedEventArgs.Complete();
                    DeltaRoutedEventArgs.Handled = true;
                }
                else if (DeltaRoutedEventArgs.Position.Y - _manipulationStartPoint.Y > 200)
                {
                    _gameGrid.HandleMove(MoveDirection.Down);
                    DeltaRoutedEventArgs.Complete();
                    DeltaRoutedEventArgs.Handled = true;
                }
            }
        }

        private Point _manipulationStartPoint;

        private void OnManipulationStarted(object Sender, ManipulationStartedRoutedEventArgs StartedRoutedEventArgs)
        {
            _manipulationStartPoint = StartedRoutedEventArgs.Position;
        }
        
        private void MainPage_SizeChanged(object Sender, SizeChangedEventArgs Args)
        {
            // TODO: take into account a variable number of tiles
            // Calculate best size for the game grid.
            var minWindowSize = Math.Min(Window.Current.Bounds.Height, Window.Current.Bounds.Width) * 0.9;

            var gridSize = Math.Min(_MAX_GRID_SIZE, minWindowSize);

            _gameGrid.Width = gridSize;
            _gameGrid.Height = gridSize;
        }
    }
}
