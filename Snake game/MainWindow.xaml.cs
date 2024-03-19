using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Snake_game
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<Grid_view, ImageSource> gridValToImage = new()
        {
            {Grid_view.empty,Images.empty },
            
            {Grid_view.snake,Images.Body },
            
            {Grid_view.food,Images.Food }
        };

        private readonly Dictionary<Directions, int> dirToRotation = new()
        {
            {Directions.Up,0 },
            
            {Directions.Right,90},
            
            {Directions.Down,180 },
            
            {Directions.Left,270},
        };

        private readonly int rows = 15, cols = 15;
        
        private readonly Image[,] gridImages;
        
        private GameState gameState;
        
        private bool gameRunning;
        
        private int highestScore = 0;
        
        public MainWindow()
        {
            InitializeComponent();
            
            gridImages = SetupGrid();
            
            gameState = new GameState(rows, cols);
        }
        
        private async Task RunGame()
        {
            Draw();
            
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            
            await GameLoop();
            
            await ShowGameOver();
            gameState = new GameState(rows, cols);
        }
        
        public async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }
            if (!gameRunning)
            {
                gameRunning = true;
                
                await RunGame();
                
                gameRunning = false;
            }
        }
        
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.gameover)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Directions.Left);
                    break;
                
                case Key.Right:
                    gameState.ChangeDirection(Directions.Right);
                    break;
                
                case Key.Up:
                    gameState.ChangeDirection(Directions.Up);
                    break;
                
                case Key.Down:
                    gameState.ChangeDirection(Directions.Down);
                    break;
            }
        }
        
        private async Task GameLoop()
        {
            while (!gameState.gameover)
            {
                await Task.Delay(100);
                
                gameState.Move();
                
                Draw();
            }
        }
        
        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            
            GameGrid.Rows = rows;
            
            GameGrid.Columns = cols;
            
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.empty,
                        
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };
                    images[r, c] = image;
                    
                    GameGrid.Children.Add(image);
                }
            }
            return images;
        }
        
        private void Draw()
        {
            DrawGrid();
            
            DrawSnakeHead();
            
            ScoreText.Text = $"SCORE: {gameState.Score}";
            
            HighestScoreText.Text = $"Highest Score:{highestScore}";
        }
        
        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Grid_view grid_View = gameState.grid[r, c];
                    
                    gridImages[r, c].Source = gridValToImage[grid_View];
                    
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }
        
        private void DrawSnakeHead()
        {
            Position HeadPos = gameState.HeadPosition();
           
            Image image = gridImages[HeadPos.Row, HeadPos.Col];
            
            image.Source = Images.Head;
            
            int rotation = dirToRotation[gameState.Dir];
            
            image.RenderTransform = new RotateTransform(rotation);
        }
        
        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());
            
            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBoby;
                
                gridImages[pos.Row, pos.Col].Source = source;
                
                await Task.Delay(50);
            }
        }
        
        private async Task ShowCountDown()
        {

            for (int i = 5; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();

                await Task.Delay(500);
            }
        }
        
        private async Task ShowGameOver()
        {
            if (gameState.Score > highestScore)
            {
                highestScore = gameState.Score;
            }
            await DrawDeadSnake();
            
            await Task.Delay(1000);
            
            Overlay.Visibility = Visibility.Visible;
            
            OverlayText.Text = "Press Any Key To Restart";
        }
    }
}