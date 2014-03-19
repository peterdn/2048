namespace _2048.Model
{
    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Coordinate(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    public class Cell
    {
        public int Value { get; set; }

        public bool WasMerged { get; set; }

        public bool WasCreated { get; set; }

        public Coordinate Position { get; set; }

        public int X
        {
            get
            {
                return Position.X;
            }
            set
            {
                Position.X = value;
            }
        }

        public int Y
        {
            get
            {
                return Position.Y;
            }
            set
            {
                Position.Y = value;
            }
        }

        public Coordinate PreviousPosition { get; set; }
        
        public Cell(int X, int Y)
        {
            this.Position = new Coordinate(X, Y);
        }

        public bool IsEmpty()
        {
            return Value == 0;
        }
    }
}
