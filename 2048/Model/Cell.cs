using System;

namespace _2048.Model
{
    public class Cell
    {
        public int Value { get; set; }

        public bool WasDoubled { get; set; }

        public bool WasCreated { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

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
    }
}
