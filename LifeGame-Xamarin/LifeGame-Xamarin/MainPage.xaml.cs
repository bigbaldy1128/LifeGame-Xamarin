using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        private bool[,] struArr;
        private bool[,] struArr2;
        private int width;
        private int height;
        private int interval = 10;
        private int area = 25;
        private int oldX, oldY;
        private bool canRun = false;


        SKPaint blackStrokePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black,
        };

        SKPaint whiteFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.White
        };

        SKPaint blackFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Black
        };

        private void OnCanvasViewPaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.LightGray);
            this.width = e.Info.Width / area;
            this.height = e.Info.Height / area;

            if (struArr2 == null)
            {
                struArr2 = new bool[width, height];
                struArr = new bool[width, height];
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    SKPaint paint = struArr2[i, j] ? blackFillPaint : whiteFillPaint;
                    canvas.DrawRect(area * i + 1, area * j + 1, area-2, area-2, paint);
                }
            }
        }

        void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchActionType.Pressed:

                    break;
                case TouchActionType.Moved:
                    SKPoint point = ConvertToPixel(args.Location);
                    int x = (int)point.X / area;
                    int y = (int)point.Y / area;
                    if (x == oldX && y == oldY)
                        return;
                    if (x > width - 1 || x < 0 || y > height - 1 || y < 0)
                        return;
                    oldX = x;
                    oldY = y;
                    struArr[x,y] = !struArr[x,y];
                    struArr2[x,y] = struArr[x,y];
                    canvasView.InvalidateSurface();
                    break;
                case TouchActionType.Released:

                    break;
            }
        }

        SKPoint ConvertToPixel(TouchTrackingPoint point)
        {
            return new SKPoint((float)(canvasView.CanvasSize.Width * point.X / canvasView.Width),
                               (float)(canvasView.CanvasSize.Height * point.Y / canvasView.Height));
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (button_start.Text == "Start")
            {
                Start();
                button_start.Text = "Stop";
            }
            else 
            {
                canRun = false;
                button_start.Text = "Start";
            }
        }

        public void Start()
        {
            canRun = true;
            new Thread(() =>
            {
                while (canRun)
                {
                    for (int i = 0; i < width; i++)
                        for (int j = 0; j < height; j++)
                            struArr2[i, j] = struArr[i, j];

                    for (int i = 0; i < width; i++)
                        for (int j = 0; j < height; j++)
                        {
                            int count = 0;
                            if (struArr[i - 1 == -1 ? width - 1 : i - 1, j - 1 == -1 ? height - 1 : j - 1])
                            {
                                count++;
                            }
                            if (struArr[i - 1 == -1 ? width - 1 : i - 1, j])
                            {
                                count++;
                            }
                            if (struArr[i - 1 == -1 ? width - 1 : i - 1, j + 1 == height ? 0 : j + 1])
                            {
                                count++;
                            }
                            if (struArr[i, j - 1 == -1 ? height - 1 : j - 1])
                            {
                                count++;
                            }
                            if (struArr[i, j + 1 == height ? 0 : j + 1])
                            {
                                count++;
                            }
                            if (struArr[i + 1 == width ? 0 : i + 1, j - 1 == -1 ? height - 1 : j - 1])
                            {
                                count++;
                            }
                            if (struArr[i + 1 == width ? 0 : i + 1, j])
                            {
                                count++;
                            }
                            if (struArr[i + 1 == width ? 0 : i + 1, j + 1 == height ? 0 : j + 1])
                            {
                                count++;
                            }
                            switch (count)
                            {
                                case 3:
                                    struArr2[i, j] = true;
                                    break;
                                case 2:
                                    if (struArr[i, j])
                                    {
                                        struArr2[i, j] = true;
                                    }
                                    break;
                                default:
                                    struArr2[i, j] = false;
                                    break;
                            }
                        }
                    canvasView.InvalidateSurface();
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            struArr[i, j] = struArr2[i, j];
                        }
                    }

                    Thread.Sleep(interval);
                }
            }).Start();
        }
    }
}