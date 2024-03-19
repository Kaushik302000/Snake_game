
namespace Snake_game
{
    public class Directions
    {
        public readonly static Directions Left = new Directions(0,-1);
        public readonly static Directions Right = new Directions(0, 1);
        public readonly static Directions Up = new Directions(-1, 0);
        public readonly static Directions Down = new Directions(1, 0);
        public int Rowoffset { get; }
        public int Coloffset { get; }

        private Directions(int rowoffset, int coloffset)
        {
            Rowoffset = rowoffset;
            Coloffset = coloffset;
        }
        public  Directions Opposite()
        {
            return new Directions(-Rowoffset, -Coloffset);

        }

        public override bool Equals(object obj)
        {
            return obj is Directions directions &&
                   Rowoffset == directions.Rowoffset &&
                   Coloffset == directions.Coloffset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Rowoffset, Coloffset);
        }

        public static bool operator ==(Directions left, Directions right)
        {
            return EqualityComparer<Directions>.Default.Equals(left, right);
        }

        public static bool operator !=(Directions left, Directions right)
        {
            return !(left == right);
        }
    }
}
