using Microsoft.VisualStudio.TestTools.UnitTesting;
using _2048.Model;

namespace _2048.Test
{
    [TestClass]
    public class GameModelTest
    {
        private const int _ROWS = 4;
        private const int _COLS = 4;

        private readonly GameModel _gameModel;

        public GameModelTest()
        {
            _gameModel = new GameModel(_ROWS, _COLS);
        }

        [TestMethod]
        public void TestBasicMovement()
        {
            int[][] cells =
            {
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 2, 0},
                new[] {0, 0, 2, 0}
            };

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Up);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {2, 0, 0, 0},
                new[] {2, 0, 0, 0}
            }));
            Assert.IsTrue(_gameModel.Score == 0);

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Down);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 2},
                new[] {0, 0, 0, 2}
            }));
            Assert.IsTrue(_gameModel.Score == 0);

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Left);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {0, 0, 4, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0}
            }));
            Assert.IsTrue(_gameModel.Score == 4);

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Right);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 4, 0}
            }));
            Assert.IsTrue(_gameModel.Score == 4);
        }

        [TestMethod]
        public void TestMergeAndMove()
        {
            int[][] cells =
            {
                new[] {0, 4, 0, 0},
                new[] {2, 0, 2, 4},
                new[] {0, 0, 0, 0},
                new[] {0, 4, 2, 0}
            };

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Up);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {4, 0, 0, 0},
                new[] {4, 4, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {4, 2, 0, 0}
            }));
            Assert.IsTrue(_gameModel.Score == 4);

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Down);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {0, 0, 0, 4},
                new[] {0, 0, 4, 4},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 4, 2}
            }));
            Assert.IsTrue(_gameModel.Score == 4);

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Left);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {2, 8, 4, 4},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0}
            }));
            Assert.IsTrue(_gameModel.Score == 12);

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Right);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {2, 8, 4, 4}
            }));
            Assert.IsTrue(_gameModel.Score == 12);
        }

        [TestMethod]
        public void TestLongMergeAndMove()
        {
            int[][] cells =
            {
                new[] {16, 0, 0, 16},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {16, 0, 0, 16}
            };

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Up);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {32, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {32, 0, 0, 0}
            }));
            Assert.IsTrue(_gameModel.Score == 64);

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Down);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {0, 0, 0, 32},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 32}
            }));
            Assert.IsTrue(_gameModel.Score == 64);

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Left);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {32, 0, 0, 32},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0}
            }));
            Assert.IsTrue(_gameModel.Score == 64);

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Right);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {32, 0, 0, 32}
            }));
            Assert.IsTrue(_gameModel.Score == 64);
        }

        [TestMethod]
        public void TestBigMerge()
        {
            int[][] cells =
            {
                new[] {16, 0, 0, 16},
                new[] {2, 0, 0, 0},
                new[] {2, 32, 16, 8},
                new[] {64, 32, 16, 8}
            };

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Up);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {32, 0, 0, 0},
                new[] {2, 0, 0, 0},
                new[] {2, 32, 16, 8},
                new[] {64, 32, 16, 8}
            }));

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Down);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {0, 0, 0, 32},
                new[] {0, 0, 0, 2},
                new[] {2, 32, 16, 8},
                new[] {64, 32, 16, 8}
            }));

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Left);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {16, 64, 32, 16},
                new[] {4, 0, 0, 16},
                new[] {64, 0, 0, 0},
                new[] {0, 0, 0, 0}
            }));

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Right);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {0, 0, 0, 0},
                new[] {16, 0, 0, 0},
                new[] {4, 0, 0, 16},
                new[] {64, 64, 32, 16}
            }));
            Assert.IsTrue(_gameModel.Score == 116);
        }

        [TestMethod]
        public void TestMultiMerge()
        {
            int[][] cells =
            {
                new[] {4, 4, 4, 4},
                new[] {4, 4, 4, 4},
                new[] {4, 4, 4, 4},
                new[] {4, 4, 4, 4}
            };

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Up);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {8, 8, 0, 0},
                new[] {8, 8, 0, 0},
                new[] {8, 8, 0, 0},
                new[] {8, 8, 0, 0}
            }));

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Down);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {0, 0, 8, 8},
                new[] {0, 0, 8, 8},
                new[] {0, 0, 8, 8},
                new[] {0, 0, 8, 8}
            }));

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Left);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {8, 8, 8, 8},
                new[] {8, 8, 8, 8},
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0}
            }));

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Right);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {0, 0, 0, 0},
                new[] {0, 0, 0, 0},
                new[] {8, 8, 8, 8},
                new[] {8, 8, 8, 8}
            }));
        }

        [TestMethod]
        public void TestAlmostStuck()
        {
            int[][] cells =
            {
                new[] {16, 2, 4, 16},
                new[] {2, 4, 0, 32},
                new[] {4, 2, 32, 8},
                new[] {16, 32, 8, 16}
            };

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Up);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {16, 2, 4, 16},
                new[] {2, 4, 32, 0},
                new[] {4, 2, 32, 8},
                new[] {16, 32, 8, 16}
            }));

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Down);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {16, 2, 4, 16},
                new[] {0, 2, 4, 32},
                new[] {4, 2, 32, 8},
                new[] {16, 32, 8, 16}
            }));

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Left);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {16, 2, 4, 16},
                new[] {2, 4, 32, 32},
                new[] {4, 2, 8, 8},
                new[] {16, 32, 0, 16}
            }));

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Right);
            Assert.IsTrue(GridsEqual(new[]
            {
                new[] {16, 2, 0, 16},
                new[] {2, 4, 4, 32},
                new[] {4, 2, 32, 8},
                new[] {16, 32, 8, 16}
            }));
        }

        [TestMethod]
        public void TestStuck()
        {
            int[][] cells =
            {
                new[] {16, 2, 4, 16},
                new[] {2, 4, 16, 32},
                new[] {4, 2, 32, 8},
                new[] {16, 32, 8, 16}
            };

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Up);
            Assert.IsTrue(GridsEqual(cells));

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Down);
            Assert.IsTrue(GridsEqual(cells));

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Left);
            Assert.IsTrue(GridsEqual(cells));

            SetCells(cells);
            _gameModel.PerformMove(MoveDirection.Right);
            Assert.IsTrue(GridsEqual(cells));
        }

        private void SetCells(int[][] Cells)
        {
            _gameModel.Reset();
            for (var x = 0; x < _COLS; ++x)
            {
                for (var y = 0; y < _ROWS; ++y)
                {
                    _gameModel.Cells[x][y] = new Cell(x, y);
                    _gameModel.Cells[x][y].Value = Cells[x][y];
                }
            }
        }

        private bool GridsEqual(int[][] Cells)
        {
            for (var x = 0; x < _COLS; ++x)
            {
                for (var y = 0; y < _ROWS; ++y)
                {
                    if (_gameModel.Cells[x][y].Value != Cells[x][y] && !_gameModel.Cells[x][y].WasCreated)
                        return false;
                }
            }
            return true;
        }
    }
}
