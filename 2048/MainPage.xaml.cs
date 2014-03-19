using System;
using Windows.UI.Xaml;

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
