using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LibToast
{
    public class ToastWindow : Window
    {
        private Timer timer;

        private Corner corner;
        private Direction movein;
        private double pxPerTickC;
        private Position<double> origin;
        private TimeSpan fadeInTime;
        private TimeSpan fadeOutTime;
        private TimeSpan lingerTime;

        private Stopwatch stopw = new Stopwatch();
        private TimeSpan last;

        private void Timer_Tick(object sender, EventArgs e)
        {
            var elapsedTime = stopw.Elapsed;
            var tick = elapsedTime - last;

            var movement = new Position<double>(0, 0);

            if (elapsedTime <= fadeInTime)
            {
                Position<double> posm = movein.GetPositionAdder();

                posm = posm * ((pxPerTickC / fadeInTime.TotalMilliseconds) * tick.TotalMilliseconds);

                movement += posm;
            }
            if (elapsedTime > fadeInTime && elapsedTime <= fadeInTime + lingerTime)
            {
                Position<double> posm = movein.GetPositionAdder();

                posm = posm * pxPerTickC;

                Left = origin.X + posm.X;
                Top = origin.Y + posm.Y;
            }
            if (elapsedTime > fadeInTime + lingerTime)
            {
                Position<double> posm = movein.Reverse().GetPositionAdder();

                posm = posm * ((pxPerTickC / fadeOutTime.TotalMilliseconds) * tick.TotalMilliseconds);

                movement += posm;
            }
            if (elapsedTime > fadeInTime + lingerTime + fadeOutTime)
                Close();

            Left += movement.X;
            Top += movement.Y;

            last = elapsedTime;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            // Move window out of screen
            var posm = movein.GetPositionAdder();

            switch (corner)
            {
                case Corner.BotRight:
                    Left = Screen.PrimaryScreen.WorkingArea.Width + posm.Y * Width;
                    Top = Screen.PrimaryScreen.WorkingArea.Height + posm.X * Height;
                    break;
                case Corner.BotLeft:
                    int ofm = -1;
                    if (posm.Y > 0)
                        ofm = 0;
                    if (posm.Y < 0)
                        ofm = 1;
                    Left = 0 - posm.X * Width;
                    Top = Screen.PrimaryScreen.WorkingArea.Height + posm.Y * Height + (Height * ofm);
                    break;
                case Corner.TopRight:
                    Left = Screen.PrimaryScreen.WorkingArea.Width + posm.Y * Width;
                    Top = 0 + posm.X * Height;
                    break;
                case Corner.TopLeft:
                    Left = 0 - posm.Y * Width;
                    Top = 0 - posm.X * Height;
                    break;
            }

            origin = new Position<double>(Left, Top);

            if (posm.X != 0)
                pxPerTickC = Width;
            if (posm.Y != 0)
                pxPerTickC = Height;

            // Begin animation
            timer.Start();
            stopw.Start();
        }

        static ToastWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToastWindow), new FrameworkPropertyMetadata(typeof(ToastWindow)));
        }

        // Fadeout default is fadein
        protected ToastWindow(Corner position, Direction movedir, TimeSpan fadein, TimeSpan stay, int ticksPerSecond = 1000 / 50) : this(position, movedir, fadein, stay, fadein, ticksPerSecond) { }

        protected ToastWindow(Corner position, Direction movedir, TimeSpan fadein, TimeSpan stay, TimeSpan fadeout, int ticksPerSecond = 1000 / 50) : base()
        {
            // Dont want our toast being focusable, do we?
            Focusable = false;
            // We want our window to be the top most
            Topmost = true;
            // Nobody should be able to resize me
            ResizeMode = ResizeMode.NoResize;
            // Pop doesn't need to be shown in task bar
            ShowInTaskbar = false;
            // Cant have those pesky borders
            WindowStyle = WindowStyle.None;
            // Create and run timer for animation
            timer = new Timer()
            {
                Interval = 50,
                Enabled = true
            };
            timer.Tick += Timer_Tick;

            Loaded += OnLoad;

            timer.Interval = 1000 / ticksPerSecond;

            corner = position;

            Direction testDir = corner.GetDirection().Reverse();
            if ((movedir & testDir) == 0)
                throw new ArgumentException("The movement direction must be the reverse of one of the corner directions!");
            movein = testDir & movedir;

            fadeInTime = fadein;
            fadeOutTime = fadeout;

            lingerTime = stay;
        }
    }

    public enum Direction
    {
        Up = 1,
        Left = 2,
        Right = 4,
        Down = 8,
        UpLeft = Up | Left,
        UpRight = Up | Right,
        DownLeft = Down | Left,
        DownRight = Down | Right
    }

    public static class DirectionMethods
    {
        private static Dictionary<Direction, Position<double>> posMap = new Dictionary<Direction, Position<double>>()
        {
            { Direction.Up, new Position<double>(0,-1) },
            { Direction.Down, new Position<double>(0,1) },
            { Direction.Left, new Position<double>(-1,0) },
            { Direction.Right, new Position<double>(1,0) },
        };
        static DirectionMethods()
        {
            Direction.DownLeft.GetPositionAdder();
            Direction.DownRight.GetPositionAdder();
            Direction.UpLeft.GetPositionAdder();
            Direction.UpRight.GetPositionAdder();
        }
        public static Position<double> GetPositionAdder(this Direction dir)
        {
            if (!posMap.ContainsKey(dir))
            {
                var npos = new Position<double>(0, 0);

                Action<Direction> poscmp = (Direction test) =>
                {
                    if ((dir & test) > 0)
                    {
                        npos = npos + posMap[test];
                    }
                };

                poscmp(Direction.Up);
                poscmp(Direction.Down);
                poscmp(Direction.Left);
                poscmp(Direction.Right);

                posMap.Add(dir, npos);
            }

            return posMap[dir];
        }

        public static Direction Reverse(this Direction d)
        {
            var v = posMap.FirstOrDefault(x => x.Value == (d.GetPositionAdder() * -1)).Key;
            return v;
        }
    }

    public enum Corner
    {
        TopLeft = Direction.UpLeft,
        TopRight = Direction.UpRight,
        BotLeft = Direction.DownLeft,
        BotRight = Direction.DownRight
    }

    public static class CornerMethods
    {
        public static Direction GetDirection(this Corner c)
        {
            int coi = (int)c;
            Direction cod = (Direction)coi;

            return cod;
        }
    }
}
