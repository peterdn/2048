using System;
using System.Windows;
using System.Windows.Input;
using _2048;
using _2048.Model;

namespace WP2048
{
    public partial class MainPage
    {
        private const int _MAX_GRID_SIZE = 400;

        private GameGrid _gameGrid;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            _gameGrid = new GameGrid();

            ContentGrid.Children.Add(_gameGrid);

            this.SizeChanged += OnSizeChanged;

            this.ManipulationDelta += OnManipulationDelta;
        }

        private void OnSizeChanged(object Sender, SizeChangedEventArgs ChangedEventArgs)
        {
            // TODO: take into account a variable number of tiles
            // Calculate best size for the game grid.
            var minWindowSize = Math.Min(ActualHeight, ActualWidth) * 0.9;

            var gridSize = Math.Min(_MAX_GRID_SIZE, minWindowSize);

            _gameGrid.Width = gridSize;
            _gameGrid.Height = gridSize;
        }

        private void OnManipulationDelta(object Sender, ManipulationDeltaEventArgs DeltaRoutedEventArgs)
        {
            if (DeltaRoutedEventArgs.CumulativeManipulation.Translation.X < -30)
            {
                _gameGrid.HandleMove(MoveDirection.Left);
                DeltaRoutedEventArgs.Complete();
                DeltaRoutedEventArgs.Handled = true;
            }
            else if (DeltaRoutedEventArgs.CumulativeManipulation.Translation.X > 30)
            {
                _gameGrid.HandleMove(MoveDirection.Right);
                DeltaRoutedEventArgs.Complete();
                DeltaRoutedEventArgs.Handled = true;
            }
            else if (DeltaRoutedEventArgs.CumulativeManipulation.Translation.Y < -30)
            {
                _gameGrid.HandleMove(MoveDirection.Up);
                DeltaRoutedEventArgs.Complete();
                DeltaRoutedEventArgs.Handled = true;
            }
            else if (DeltaRoutedEventArgs.CumulativeManipulation.Translation.Y > 30)
            {
                _gameGrid.HandleMove(MoveDirection.Down);
                DeltaRoutedEventArgs.Complete();
                DeltaRoutedEventArgs.Handled = true;
            }
        }
    }
}