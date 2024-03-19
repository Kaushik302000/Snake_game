using System.Reflection.Metadata.Ecma335;

namespace Snake_game
{
    public class Position
    {
        public int Row {  get;}
        public int Col { get;}
        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
        public Position Translate(Directions dir)
        {
            return new Position(Row + dir.Rowoffset, Col + dir.Coloffset);
        }

        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   Row == position.Row &&
                   Col == position.Col;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        public static bool operator ==(Position left, Position right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }
    }
}
