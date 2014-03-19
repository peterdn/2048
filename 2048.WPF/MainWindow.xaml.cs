using System;
using System.Windows;
using System.Windows.Input;
using _2048.Model;

namespace _2048.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int _MAX_GRID_SIZE = 600;

        private readonly GameGrid _gameGrid;

        public MainWindow()
        {
            InitializeComponent();

            _gameGrid = new GameGrid();

            ContentGrid.Children.Add(_gameGrid);

            this.SizeChanged += OnSizeChanged;
            this.KeyDown += MainWindow_KeyDown;
        }

        void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs Args)
        {
            MoveDirection? direction = null;
            if (Args.Key == Key.Up)
            {
                direction = MoveDirection.Up;
            }
            else if (Args.Key == Key.Down)
            {
                direction = MoveDirection.Down;
            }
            else if (Args.Key == Key.Left)
            {
                direction = MoveDirection.Left;
            }
            else if (Args.Key == Key.Right)
            {
                direction = MoveDirection.Right;
            }

            if (direction != null)
            {
                _gameGrid.HandleMove(direction.Value);
            }
        }

        private void OnSizeChanged(object Sender, SizeChangedEventArgs E)
        {
            var gridSize = Math.Min(ContentGrid.ActualHeight, ContentGrid.ActualWidth) * 0.9;
            gridSize = Math.Min(gridSize, _MAX_GRID_SIZE);

            _gameGrid.Width = gridSize;
            _gameGrid.Height = gridSize;
        }
    }
}
