using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace _2048
{
    public sealed partial class GameTile
    {
        private readonly Color[] _backColors =
        {
            Color.FromArgb(255, 159, 192, 255),
            Color.FromArgb(255, 168, 255, 99),
            Color.FromArgb(255, 255, 255, 104),
            Color.FromArgb(255, 255, 221, 140),
            Color.FromArgb(255, 255, 170, 63),
            Color.FromArgb(255, 255, 223, 50),
            Color.FromArgb(255, 206, 255, 86),
            Color.FromArgb(255, 118, 255, 138),
            Color.FromArgb(255, 89, 118, 255),
            Color.FromArgb(255, 205, 96, 255),
            Color.FromArgb(255, 255, 78, 59),
        };

        private readonly Color[] _foreColors =
        {
            Color.FromArgb(0xff, 0x77, 0x6e, 0x65),
            Color.FromArgb(0xff, 0x77, 0x6e, 0x65),
            Color.FromArgb(0xff, 0x77, 0x6e, 0x65),
            Color.FromArgb(0xff, 0x77, 0x6e, 0x65),
            Color.FromArgb(255, 255, 255, 255),
            Color.FromArgb(0xff, 0x77, 0x6e, 0x65),
            Color.FromArgb(0xff, 0x77, 0x6e, 0x65),
            Color.FromArgb(0xff, 0x77, 0x6e, 0x65),
            Color.FromArgb(255, 255, 255, 255),
            Color.FromArgb(255, 255, 255, 255),
            Color.FromArgb(255, 255, 255, 255),
        };

        private int _value;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value == 2 && _model != null && _model.Cells[x][y].WasDoubled)
                {
                    Debugger.Break();
                }

                _value = value;
                _textBlock.Text = _value > 0 ? _value.ToString() : "";
                TileBorder.Background = new SolidColorBrush(value > 0 ? _backColors[(int)Math.Log(_value, 2) - 1] : Color.FromArgb(0xff, 0xbb, 0xab, 0xb0));
                _textBlock.Foreground = value > 0 ? new SolidColorBrush(_foreColors[(int)Math.Log(_value, 2) - 1]) : _textBlock.Foreground;
            }
        }

        private readonly TextBlock _textBlock;

        private GameModel _model;
        private int x;
        private int y;

        public GameTile(GameModel Model, int x, int y, bool TransparentBorder = false)
        {
            this.InitializeComponent();

            _textBlock = new TextBlock();
            _textBlock.Text = "";
            _textBlock.FontSize = 32;
            _textBlock.FontWeight = FontWeights.Bold;
            _textBlock.TextAlignment = TextAlignment.Center;
            _textBlock.VerticalAlignment = VerticalAlignment.Center;
            TileBorder.Child = _textBlock;

            if (TransparentBorder)
            {
                ContentBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
            }

            _model = Model;
            this.x = x;
            this.y = y;
        }

        public void BeginNewTileAnimation()
        {
            var scaleXAnimation = new DoubleAnimation();
            scaleXAnimation.From = 0.1;
            scaleXAnimation.To = 1.0;
            scaleXAnimation.Duration = new Duration(new TimeSpan(1200000));

            var scaleYAnimation = new DoubleAnimation();
            scaleYAnimation.From = 0.1;
            scaleYAnimation.To = 1.0;
            scaleYAnimation.Duration = new Duration(new TimeSpan(1200000));

            Storyboard.SetTarget(scaleXAnimation, TileBorder);
            Storyboard.SetTargetName(scaleXAnimation, "AnimatedScaleTransform");
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));

            //((TransformGroup)RenderTransform).Children

            Storyboard.SetTarget(scaleYAnimation, TileBorder);
            Storyboard.SetTargetName(scaleYAnimation, "AnimatedScaleTransform");
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));

            var storyboard = new Storyboard();
            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);
            storyboard.Begin();
        }

        public void BeginDoubledAnimation()
        {
            var scaleXAnimation = new DoubleAnimation();
            scaleXAnimation.From = 1.0;
            scaleXAnimation.To = 1.2;
            scaleXAnimation.Duration = new Duration(new TimeSpan(1200000));
            scaleXAnimation.AutoReverse = true;

            var scaleYAnimation = new DoubleAnimation();
            scaleYAnimation.From = 1.0;
            scaleYAnimation.To = 1.2;
            scaleYAnimation.Duration = new Duration(new TimeSpan(1200000));
            scaleYAnimation.AutoReverse = true;
            
            Storyboard.SetTarget(scaleXAnimation, TileBorder);
            Storyboard.SetTargetName(scaleXAnimation, "AnimatedScaleTransform");
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));

            Storyboard.SetTarget(scaleYAnimation, TileBorder);
            Storyboard.SetTargetName(scaleYAnimation, "AnimatedScaleTransform");
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));

            var storyboard = new Storyboard();
            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);

            storyboard.Completed += (Sender, O) => SetValue(Canvas.ZIndexProperty, 0);

            storyboard.Begin();
        }
    }
}
