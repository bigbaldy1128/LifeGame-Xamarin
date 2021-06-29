using SkiaSharp;
using System;
using System.Threading;
using TouchTracking;
using Xamarin.Forms;

namespace LifeGame_Xamarin
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private int[,] board;
        private int width;
        private int height;
        private const int interval = 10;
        private const int area = 25;
        private int oldX, oldY;
        private bool canRun = false;

        readonly SKPaint blackStrokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
        };

        readonly SKPaint whiteFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.White
        };

        readonly SKPaint blackFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Black
        };

        private void OnCanvasViewPaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SKColors.LightGray);

            width = e.Info.Width / area;
            height = e.Info.Height / area;

            if (board == null)
            {
                board = new int[width, height];
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    SKPaint paint = board[i, j] == 1 ? blackFillPaint : whiteFillPaint;
                    canvas.DrawRect((area * i) + 1, (area * j) + 1, area - 2, area - 2, paint);
                }
            }
        }

        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchActionType.Moved:
                    SKPoint point = ConvertToPixel(args.Location);
                    int x = (int)point.X / area;
                    int y = (int)point.Y / area;
                    if (x == oldX && y == oldY)
                    {
                        return;
                    }
                    if (x > width - 1 || x < 0 || y > height - 1 || y < 0)
                    {
                        return;
                    }
                    oldX = x;
                    oldY = y;
                    board[x, y] = board[x, y] == 0 ? 1 : 0;
                    canvasView.InvalidateSurface();
                    break;
            }
        }

        private SKPoint ConvertToPixel(TouchTrackingPoint point)
        {
            return new SKPoint((float)(canvasView.CanvasSize.Width * point.X / canvasView.Width),
                               (float)(canvasView.CanvasSize.Height * point.Y / canvasView.Height));
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (button_start.Text == "Start")
            {
                canRun = true;
                Start();
                button_start.Text = "Stop";
            }
            else 
            {
                canRun = false;
                button_start.Text = "Start";
            }
        }

        void GameOfLife(int[,] board)
        {
            int[] neighbors = { 0, 1, -1 };

            int rows = board.GetLength(0);
            int cols = board.GetLength(1);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int liveNeighbors = 0;

                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (!(neighbors[i] == 0 && neighbors[j] == 0))
                            {
                                int r = row + neighbors[i];
                                int c = col + neighbors[j];
                                r = r >= rows ? 0 : r;
                                r = r < 0 ? rows - 1 : r;
                                c = c >= cols ? 0 : c;
                                c = c < 0 ? cols - 1 : c;
                                if (r < rows && r >= 0 && c < cols && c >= 0 && (Math.Abs(board[r, c]) == 1))
                                {
                                    liveNeighbors += 1;
                                }
                            }
                        }
                    }

                    if ((board[row, col] == 1) && (liveNeighbors < 2 || liveNeighbors > 3))
                    {
                        board[row, col] = -1;
                    }
                    if (board[row, col] == 0 && liveNeighbors == 3)
                    {
                        board[row, col] = 2;
                    }
                }
            }

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    board[row, col] = board[row, col] > 0 ? 1 : 0;
                }
            }
        }

        public void Start()
        {
            new Thread(() =>
            {
                while (canRun)
                {
                    GameOfLife(board);
                    canvasView.InvalidateSurface();
                    Thread.Sleep(interval);
                }
            }).Start();
        }
    }
}