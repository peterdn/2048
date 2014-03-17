using System;
using System.Diagnostics;

namespace _2048
{
    public class Cell
    {
        private int _value;

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value == 2 && WasDoubled)
                    Debugger.Break();
                _value = value;
            }
        }

        private bool _wasDoubled;

        public bool WasDoubled
        {
            get
            {
                return _wasDoubled;
            }
            set
            {
                if (value == true && _value == 2)
                    Debugger.Break();
                _wasDoubled = value;
            }
        }

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
