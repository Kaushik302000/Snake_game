using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake_game
{
    public class GameState
    {
        public int Rows { get;}
        public int Cols { get;}
        public Grid_view[,] grid {  get;}
        public Directions Dir { get; private set; }
        public int Score { get; private set; }
        public bool gameover { get; private set; }

        private readonly LinkedList<Directions> dirChanges = new LinkedList<Directions>();
        private readonly LinkedList<Position> snakepositions = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int rows,int cols)
        {
            Rows = rows;
            Cols = cols;
            grid = new Grid_view[rows, cols];
            Dir = Directions.Right;

            Addsnake();
            
            AddFood();
        }
        private void Addsnake()
        {
            int r = Rows / 2;
            for (int c = 1;c<=3;c++)
            {
                grid[r, c] = Grid_view.snake;
                snakepositions.AddFirst(new Position(r, c));
            }
        }
        private IEnumerable<Position> EmptyPosition()
        {
            for (int r=0;r< Rows;r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (grid[r, c] == Grid_view.empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPosition());

            if (empty.Count == 0)
            {
                return;

            }
            Position pos = empty[random.Next(empty.Count)];
            grid[pos.Row,pos.Col]=Grid_view.food;

        }

        public Position HeadPosition()
        {
            return snakepositions.First.Value;

        }
        public Position TailPosition()
        {
            return snakepositions.Last.Value;
        }
        public IEnumerable<Position> SnakePositions()
        {
            return snakepositions;
        }
        private void AddHead(Position pos)
        {
            snakepositions.AddFirst(pos);
            grid[pos.Row, pos.Col] = Grid_view.snake;

        }
        private void RemoveTail()
        {
            Position tail= snakepositions.Last.Value;
            grid[tail.Row, tail.Col] = Grid_view.empty;
            snakepositions.RemoveLast();
        }

        private Directions GetLastDirection()
        {
            if(dirChanges.Count == 0)
            {
                return Dir;
            }
            return dirChanges.Last.Value;
        }
        private bool CanChangeDirection(Directions newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;
            }

            Directions lastDir= GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();

        }
        public void ChangeDirection(Directions dir)
        {
            if (CanChangeDirection(dir))
            {
                dirChanges.AddLast(dir);
            }
        }
        public bool OutSideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;

        }
        private Grid_view WillHit(Position newHeadPos)
        {
            if (OutSideGrid(newHeadPos))
            {
                return Grid_view.outside;
            }
            if (newHeadPos == TailPosition())
            {
                return Grid_view.empty;
            }
            return grid[newHeadPos.Row, newHeadPos.Col];
        }
        public void Move()
        {

            if (dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst(); 
            }
            
            Position newHeadPos = HeadPosition().Translate(Dir);
            Grid_view Hit = WillHit(newHeadPos);
            if (Hit == Grid_view.outside || Hit == Grid_view.snake)
            {
                gameover = true;

            }
            else if (Hit == Grid_view.empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (Hit == Grid_view.food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();

            }
        }
    }
}
