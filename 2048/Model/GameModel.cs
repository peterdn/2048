using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }

        public Cell[][] Cells { get; private set; }

        public GameModel(int RowCount, int ColumnCount)
        {
            this.RowCount = RowCount;
            this.ColumnCount = ColumnCount;

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

        public bool PerformMove(MoveDirection Direction)
        {
            if (PackTiles(Direction) | MergeTiles(Direction))
            {
                var newTile = GetRandomEmptyTile();

                if (newTile != null)
                {
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


        private bool PackTiles(MoveDirection Direction)
        {
            var changed = false;
            if (Direction == MoveDirection.Up)
            {
                for (var x = 0; x < ColumnCount; ++x)
                {
                    var lastEmptyY = 0;

                    while (lastEmptyY < RowCount && !Cells[x][lastEmptyY].IsEmpty())
                    {
                        ++lastEmptyY;
                    }

                    var currentY = lastEmptyY + 1;
                    while (currentY < RowCount)
                    {
                        if (!Cells[x][currentY].IsEmpty())
                        {
                            changed = true;
                            Cells[x][lastEmptyY].Value = Cells[x][currentY].Value;
                            Cells[x][lastEmptyY].MovedFrom = Cells[x][currentY].MovedFrom ?? new Tuple<int, int>(x, currentY);
                            Cells[x][currentY].Value = 0;
                            Cells[x][currentY].MovedFrom = null;
                            ++lastEmptyY;
                        }
                        ++currentY;
                    }
                }
            }
            else if (Direction == MoveDirection.Down)
            {
                for (var x = 0; x < ColumnCount; ++x)
                {
                    var lastEmptyY = RowCount - 1;

                    while (lastEmptyY >= 0 && !Cells[x][lastEmptyY].IsEmpty())
                    {
                        --lastEmptyY;
                    }

                    var currentY = lastEmptyY - 1;
                    while (currentY >= 0)
                    {
                        if (!Cells[x][currentY].IsEmpty())
                        {
                            changed = true;
                            Cells[x][lastEmptyY].Value = Cells[x][currentY].Value;
                            Cells[x][lastEmptyY].MovedFrom = Cells[x][currentY].MovedFrom ?? new Tuple<int, int>(x, currentY);
                            Cells[x][currentY].Value = 0;
                            Cells[x][currentY].MovedFrom = null;
                            --lastEmptyY;
                        }
                        --currentY;
                    }
                }
            }
            else if (Direction == MoveDirection.Left)
            {
                for (var y = 0; y < RowCount; ++y)
                {
                    var lastEmptyX = 0;

                    while (lastEmptyX < ColumnCount && !Cells[lastEmptyX][y].IsEmpty())
                    {
                        ++lastEmptyX;
                    }

                    var currentX = lastEmptyX + 1;
                    while (currentX < ColumnCount)
                    {
                        if (Cells[currentX][y].Value > 0)
                        {
                            changed = true;
                            Cells[lastEmptyX][y].Value = Cells[currentX][y].Value;
                            Cells[lastEmptyX][y].MovedFrom = Cells[currentX][y].MovedFrom ?? new Tuple<int, int>(currentX, y);
                            Cells[currentX][y].Value = 0;
                            Cells[currentX][y].MovedFrom = null;
                            ++lastEmptyX;
                        }
                        ++currentX;
                    }
                }
            }
            else if (Direction == MoveDirection.Right)
            {

                for (var y = 0; y < RowCount; ++y)
                {
                    var lastEmptyX = ColumnCount - 1;

                    while (lastEmptyX >= 0 && !Cells[lastEmptyX][y].IsEmpty())
                    {
                        --lastEmptyX;
                    }

                    var currentX = lastEmptyX - 1;
                    while (currentX >= 0)
                    {
                        if (Cells[currentX][y].Value > 0)
                        {
                            changed = true;
                            Cells[lastEmptyX][y].Value = Cells[currentX][y].Value;
                            Cells[lastEmptyX][y].MovedFrom = Cells[currentX][y].MovedFrom ?? new Tuple<int, int>(currentX, y);
                            Cells[currentX][y].Value = 0;
                            Cells[currentX][y].MovedFrom = null;
                            --lastEmptyX;
                        }
                        --currentX;
                    }
                }
            }
            return changed;
        }

        private bool MergeTiles(MoveDirection Direction)
        {
            var changed = false;
            if (Direction == MoveDirection.Up)
            {
                for (var x = 0; x < ColumnCount; ++x)
                {
                    for (var y = 0; y < RowCount - 1 && Cells[x][y].Value > 0; ++y)
                    {
                        if (Cells[x][y].Value == Cells[x][y + 1].Value)
                        {
                            changed = true;
                            Cells[x][y].Value *= 2;
                            Cells[x][y].WasDoubled = true;
                            if (Cells[x][y].Value == 1)
                                Debugger.Break();
                            Cells[x][y].MovedFrom = Cells[x][y + 1].MovedFrom ?? new Tuple<int, int>(x, y + 1);
                            Cells[x][y + 1].Value = 0;
                            Cells[x][y + 1].MovedFrom = null;
                            PackTiles(Direction);
                        }
                    }
                }
            }
            else if (Direction == MoveDirection.Down)
            {
                for (var x = 0; x < ColumnCount; ++x)
                {
                    for (var y = RowCount - 1; y >= 1 && Cells[x][y].Value > 0; --y)
                    {
                        if (Cells[x][y].Value == Cells[x][y - 1].Value)
                        {
                            changed = true;
                            Cells[x][y].Value *= 2;
                            Cells[x][y].WasDoubled = true;
                            if (Cells[x][y].Value == 1)
                                Debugger.Break();
                            Cells[x][y].MovedFrom = Cells[x][y - 1].MovedFrom ?? new Tuple<int, int>(x, y - 1);
                            Cells[x][y - 1].Value = 0;
                            Cells[x][y - 1].MovedFrom = null;
                            PackTiles(Direction);
                        }
                    }
                }
            }
            else if (Direction == MoveDirection.Left)
            {
                for (var y = 0; y < RowCount; ++y)
                {
                    for (var x = 0; x < ColumnCount - 1 && Cells[x][y].Value > 0; ++x)
                    {
                        if (Cells[x][y].Value == Cells[x + 1][y].Value)
                        {
                            changed = true;
                            Cells[x][y].Value *= 2;
                            Cells[x][y].WasDoubled = true;
                            if (Cells[x][y].Value == 1)
                                Debugger.Break();
                            Cells[x][y].MovedFrom = Cells[x + 1][y].MovedFrom ?? new Tuple<int, int>(x + 1, y);
                            Cells[x + 1][y].Value = 0;
                            Cells[x + 1][y].MovedFrom = null;
                            PackTiles(Direction);
                        }
                    }
                }
            }
            else if (Direction == MoveDirection.Right)
            {
                for (var y = 0; y < RowCount; ++y)
                {
                    for (var x = ColumnCount - 1; x >= 1 && Cells[x][y].Value > 0; --x)
                    {
                        if (Cells[x][y].Value == Cells[x - 1][y].Value)
                        {
                            changed = true;
                            Cells[x][y].Value *= 2;
                            Cells[x][y].WasDoubled = true;
                            if (Cells[x][y].Value == 1)
                                Debugger.Break();
                            Cells[x][y].MovedFrom = Cells[x - 1][y].MovedFrom ?? new Tuple<int, int>(x - 1, y);
                            Cells[x - 1][y].Value = 0;
                            Cells[x - 1][y].MovedFrom = null;
                            PackTiles(Direction);
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
            for (int y = 0; y < RowCount; ++y)
            {
                for (int x = 0; x < ColumnCount; ++x)
                {
                    if (Cells[x][y].IsEmpty())
                    {
                        emptyIndices.Add(new Tuple<int, int>(x, y));
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
            return rnd.NextDouble() < 0.9 ? 2 : 4;
        }
    }
}
