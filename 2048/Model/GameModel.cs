using System;
using System.Collections.Generic;
using System.Linq;

namespace _2048.Model
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public class GameModel
    {
        public int Score { get; private set; }
        
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }

        public Cell[][] Cells { get; private set; }

        public IEnumerable<Cell> CellsIterator()
        {
            for (var x = 0; x < ColumnCount; ++x)
            {
                for (var y = 0; y < RowCount; ++y)
                {
                    yield return Cells[x][y];
                }
            }
        } 

        public GameModel(int RowCount, int ColumnCount)
        {
            this.RowCount = RowCount;
            this.ColumnCount = ColumnCount;
            
            this.Reset();
        }

        public bool PerformMove(MoveDirection Direction)
        {
            if (PackAndMerge(Direction))
            {
                var newTile = GetRandomEmptyTile();

                if (newTile != null)
                {
                    // TODO move this to its own testable method
                    Cells[newTile.Item1][newTile.Item2].Value = GetRandomStartingNumber();
                    Cells[newTile.Item1][newTile.Item2].WasCreated = true;
                    return true;
                }
                else
                {
                    // Game over?
                }
            }
            return false;
        }


        private bool PackAndMerge(MoveDirection Direction)
        {
            var changed = false;
            if (Direction == MoveDirection.Up)
            {
                // For each column
                for (var x = 0; x < ColumnCount; ++x)
                {
                    // Look at tiles in the column from bottom to top
                    for (var y = 1; y < RowCount; ++y)
                    {
                        if (Cells[x][y].IsEmpty())
                        {
                            continue;
                        }

                        var destinationY = y;
                        while (destinationY - 1 >= 0 && (Cells[x][destinationY - 1].IsEmpty() || (Cells[x][destinationY - 1].Value == Cells[x][y].Value && !Cells[x][destinationY - 1].WasMerged)))
                        {
                            --destinationY;
                        }

                        if (destinationY != y)
                        {
                            MergeCells(Cells[x][y], Cells[x][destinationY]);
                            changed = true;
                        }
                    }
                }
            }
            else if (Direction == MoveDirection.Down)
            {
                // For each column
                for (var x = 0; x < ColumnCount; ++x)
                {
                    // Look at tiles in the column from bottom to top
                    for (var y = RowCount - 2; y >= 0; --y)
                    {
                        if (Cells[x][y].IsEmpty())
                        {
                            continue;
                        }

                        var destinationY = y;
                        while (destinationY + 1 < RowCount && (Cells[x][destinationY + 1].IsEmpty() || (Cells[x][destinationY + 1].Value == Cells[x][y].Value && !Cells[x][destinationY + 1].WasMerged)))
                        {
                            ++destinationY;
                        }

                        if (destinationY != y)
                        {
                            MergeCells(Cells[x][y], Cells[x][destinationY]);
                            changed = true;
                        }
                    }
                }
            }
            else if (Direction == MoveDirection.Left)
            {
                for (var y = 0; y < RowCount; ++y)
                {
                    // Look at tiles in the column from bottom to top
                    for (var x = 1; x < ColumnCount; ++x)
                    {
                        if (Cells[x][y].IsEmpty())
                        {
                            continue;
                        }

                        var destinationX = x;
                        while (destinationX - 1 >= 0 && (Cells[destinationX - 1][y].IsEmpty() || (Cells[destinationX - 1][y].Value == Cells[x][y].Value && !Cells[destinationX - 1][y].WasMerged)))
                        {
                            --destinationX;
                        }

                        if (destinationX != x)
                        {
                            MergeCells(Cells[x][y], Cells[destinationX][y]);
                            changed = true;
                        }
                    }
                }
            }
            else if (Direction == MoveDirection.Right)
            {
                for (var y = 0; y < RowCount; ++y)
                {
                    // Look at tiles in the column from bottom to top
                    for (var x = ColumnCount - 2; x >= 0; --x)
                    {
                        if (Cells[x][y].IsEmpty())
                        {
                            continue;
                        }

                        var destinationX = x;
                        while (destinationX + 1 < ColumnCount && (Cells[destinationX + 1][y].IsEmpty() || (Cells[destinationX + 1][y].Value == Cells[x][y].Value && !Cells[destinationX + 1][y].WasMerged)))
                        {
                            ++destinationX;
                        }

                        if (destinationX != x)
                        {
                            MergeCells(Cells[x][y], Cells[destinationX][y]);
                            changed = true;
                        }
                    }
                }
            }
            return changed;
        }

        private void MergeCells(Cell SourceCell, Cell DestinationCell)
        {
            // Assumes that an appropriate merge CAN definitely be done.
            if (!(SourceCell.X == DestinationCell.X ^ SourceCell.Y == DestinationCell.Y))
            {
                throw new InvalidOperationException("Cells to be merged must share either a row or column but not both");
            }
            
            if (DestinationCell.IsEmpty())
            {
                // This is the last available empty cell so take it!
                DestinationCell.Value = SourceCell.Value;
                DestinationCell.PreviousPosition = SourceCell.PreviousPosition ?? new Coordinate(SourceCell.X, SourceCell.Y);
                SourceCell.Value = 0;
                SourceCell.PreviousPosition = null;
            }
            else
            {
                if (DestinationCell.WasMerged)
                {
                    throw new InvalidOperationException("Destination cell has already been merged");
                }
                else if (SourceCell.Value != DestinationCell.Value)
                {
                    throw new InvalidOperationException("Source and destination cells must have the same value");
                }

                // The next available cell has the same value and hasn't yet
                // been merged, so lets merge them!
                DestinationCell.Value *= 2;
                DestinationCell.WasMerged = true;
                DestinationCell.PreviousPosition = SourceCell.PreviousPosition ?? new Coordinate(SourceCell.X, SourceCell.Y);
                SourceCell.Value = 0;
                SourceCell.PreviousPosition = null;

                // Update the score
                Score += DestinationCell.Value;
            }
        }

        public void Reset()
        {
            this.Score = 0;

            Cells = new Cell[this.ColumnCount][];
            for (int i = 0; i < this.ColumnCount; ++i)
            {
                Cells[i] = new Cell[this.RowCount];
            }

            for (int y = 0; y < this.RowCount; ++y)
            {
                for (int x = 0; x < this.ColumnCount; ++x)
                {
                    Cells[x][y] = new Cell(x, y);
                }
            }
        }

        private Random rnd = new Random();
        private Tuple<int, int> GetRandomEmptyTile()
        {
            var emptyIndices = (from cell in CellsIterator() where cell.IsEmpty() select new Tuple<int, int>(cell.X, cell.Y)).ToList();

            if (emptyIndices.Count == 0)
            {
                return null;
            }

            var next = rnd.Next(0, emptyIndices.Count - 1);
            return emptyIndices[next];
        }

        private int GetRandomStartingNumber()
        {
            return rnd.NextDouble() < 0.9 ? 2 : 4;
        }
    }
}
