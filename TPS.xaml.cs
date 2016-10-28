using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageTwisting
{
    /// <summary>
    /// TPS.xaml 的交互逻辑
    /// </summary>
    public partial class TPS : Window
    {
        public TPS()
        {
            InitializeComponent();
            flag = false;
            reset.IsEnabled = false;
            clear.IsEnabled = false;
            excute.IsEnabled = false;
            fixPoint = new List<System.Windows.Point>();
            targetPoint = new List<System.Windows.Point>();
            fixPoint.Clear();
            targetPoint.Clear();
        }

        bool flag;
        List<System.Windows.Point> fixPoint, targetPoint;

        private void img_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            choose.IsEnabled = false;
            System.Windows.Point pointer = e.GetPosition(img);
            pointer.X += 2;
            pointer.Y += 2;
            //MessageBox.Show(pointer.X.ToString()+" "+pointer.Y.ToString());
            if (!flag)
            {
                flag = true;
                fixPoint.Add(pointer);
                Line myLine = new Line();
                myLine.Stroke = System.Windows.Media.Brushes.Red;
                myLine.X1 = fixPoint[fixPoint.Count - 1].X - 4;
                myLine.X2 = fixPoint[fixPoint.Count - 1].X + 4;
                myLine.Y1 = fixPoint[fixPoint.Count - 1].Y - 4;
                myLine.Y2 = fixPoint[fixPoint.Count - 1].Y + 4;
                myLine.StrokeThickness = 3;
                myLine.Name = "dot_" + (2 * (fixPoint.Count - 1)).ToString();
                myLine.SetValue(Grid.RowProperty, 1);
                myLine.SetValue(Grid.ColumnSpanProperty, 2);
                gd.RegisterName(myLine.Name, myLine);
                gd.Children.Add(myLine);
                
                myLine = new Line();
                myLine.Stroke = System.Windows.Media.Brushes.Red;
                myLine.X1 = fixPoint[fixPoint.Count - 1].X + 4;
                myLine.X2 = fixPoint[fixPoint.Count - 1].X - 4;
                myLine.Y1 = fixPoint[fixPoint.Count - 1].Y - 4;
                myLine.Y2 = fixPoint[fixPoint.Count - 1].Y + 4;
                myLine.StrokeThickness = 3;
                myLine.Name = "dot_" + (2 * (fixPoint.Count - 1) + 1).ToString();
                myLine.SetValue(Grid.RowProperty, 1);
                myLine.SetValue(Grid.ColumnSpanProperty, 2);
                gd.RegisterName(myLine.Name, myLine);
                gd.Children.Add(myLine);
            }
            else
            {
                flag = false;
                targetPoint.Add(pointer);
                Line myLine = new Line();
                myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                myLine.X1 = fixPoint[fixPoint.Count - 1].X;
                myLine.Y1 = fixPoint[fixPoint.Count - 1].Y;
                myLine.X2 = targetPoint[targetPoint.Count - 1].X;
                myLine.Y2 = targetPoint[targetPoint.Count - 1].Y;
                myLine.StrokeThickness = 2;
                myLine.Name = "line_" + (targetPoint.Count - 1).ToString();
                myLine.SetValue(Grid.RowProperty, 1);
                myLine.SetValue(Grid.ColumnSpanProperty, 2);
                gd.RegisterName(myLine.Name, myLine);
                gd.Children.Add(myLine);
            }
            excute.IsEnabled = !flag;
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < fixPoint.Count * 2; ++i)
            {
                Line myLine = FindName("dot_" + i.ToString()) as Line;
                gd.UnregisterName(myLine.Name);
                gd.Children.Remove(myLine);
            }
            for (int i = 0; i < targetPoint.Count; ++i)
            {
                Line myLine = FindName("line_" + i.ToString()) as Line;
                gd.UnregisterName(myLine.Name);
                gd.Children.Remove(myLine);
            }
            fixPoint.Clear();
            targetPoint.Clear();
            excute.IsEnabled = true;
            flag = false;
            choose.IsEnabled = true;
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            choose.IsEnabled = true;
            fixPoint.Clear();
            targetPoint.Clear();
            excute.IsEnabled = true;
            clear.IsEnabled = true;
            img.Source = new BitmapImage(new Uri(imagesource, UriKind.Absolute));
            reset.IsEnabled = false;
        }
        int r, c;
        int[, , ,] map;
        string imagesource;
        private void choose_Click(object sender, RoutedEventArgs e)
        {
            fixPoint.Clear();
            targetPoint.Clear();
            excute.IsEnabled = true;
            clear.IsEnabled = true;
            reset.IsEnabled = false;
            
            OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            op.InitialDirectory = @"c:\";
            op.RestoreDirectory = true;
            op.Filter = "图片文件|*.jpg;*.bmp;*.png";
            op.ShowDialog();
            dir.Text = op.FileName;
            if (op.FileName == "")
                return;
            imagesource = op.FileName;
            img.Source = new BitmapImage(new Uri(op.FileName, UriKind.Absolute));
            Bitmap bmp = new Bitmap(op.FileName);
            System.Drawing.Color pixelColor;
            r = bmp.Height + 4;
            c = bmp.Width + 4;
            map = new int[2, c, r, 3];
            img.Height = bmp.Height;
            img.Width = bmp.Height;
            for (int i = 0; i < c - 4; ++i)
            {
                for (int j = 0; j < r - 4; ++j)
                {
                    pixelColor = bmp.GetPixel(i, j);
                    map[0, i + 2, j + 2, 0] = pixelColor.R;
                    map[0, i + 2, j + 2, 1] = pixelColor.G;
                    map[0, i + 2, j + 2, 2] = pixelColor.B;
                }
            }
        }

        double[,] L;
        private void excute_Click(object sender, RoutedEventArgs e)
        {
            if (fixPoint.Count < 3)
            {
                MessageBox.Show("Need at least 3 pairs of control points. Now you have " + fixPoint.Count.ToString() + ".");
                return;
            }
            choose.IsEnabled = true;
            for (int i = 0; i < fixPoint.Count * 2; ++i)
            {
                Line myLine = FindName("dot_" + i.ToString()) as Line;
                gd.UnregisterName(myLine.Name);
                gd.Children.Remove(myLine);
            }
            for (int i = 0; i < targetPoint.Count; ++i)
            {
                Line myLine = FindName("line_" + i.ToString()) as Line;
                gd.UnregisterName(myLine.Name);
                gd.Children.Remove(myLine);
            }
            clear.IsEnabled = false;
            int n = fixPoint.Count;
            L = new double[n + 3, n + 3];
            for (int i = 0; i < n + 3; ++i)
                for (int j = 0; j < n + 3; ++j)
                    L[i, j] = 0;
            for (int i = 0; i < n; ++i)
            {
                for (int j = i + 1; j < n; ++j)
                {
                    L[i, j] = L[j, i] = Tools.U(Tools.r(targetPoint[i], targetPoint[j]));
                }
            }
            for (int i = 0; i < n; ++i)
            {
                L[n, i] = L[i, n] = 1;
                L[n + 1, i] = L[i, n + 1] = targetPoint[i].X;
                L[n + 2, i] = L[i, n + 2] = targetPoint[i].Y;
            }
            double[,] Y = new double[n + 3, 2];
            for (int i = n; i < n + 3; ++i)
                for (int j = 0; j < 2; ++j)
                    Y[i, j] = 0;
            for (int i = 0; i < n; ++i)
            {
                Y[i, 0] = fixPoint[i].X;
                Y[i, 1] = fixPoint[i].Y;
            }
            double[,] ans = Tools.times(Tools.inverse(L, n + 3, n + 3), n + 3, n + 3, Y, n + 3, 2);
            double[,] w = new double[n, 2];
            double[] a1 = new double[2], ax = new double[2], ay = new double[2];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    w[i, j] = ans[i, j];
                }
            }
            a1[0] = ans[n, 0]; a1[1] = ans[n, 1];
            ax[0] = ans[n + 1, 0]; ax[1] = ans[n + 1, 1];
            ay[0] = ans[n + 2, 0]; ay[1] = ans[n + 2, 1];
            Bitmap bmp = new Bitmap(c, r);
            double temp;
            double[] u = new double[2], v = new double[2], A, C;
            double[, ,] B;
            A = new double[4];
            C = new double[4];
            B = new double[4, 4, 3];
            System.Drawing.Color colorForSet;
            IntPtr ip;
            BitmapSource bitmapSource;
            for (int i = 0; i < c; ++i)
            {
                for (int j = 0; j < r; ++j)
                {
                    double _i = a1[0] + ax[0] * i + ay[0] * j, _j = a1[1] + ax[1] * i + ay[1] * j;
                    for (int k = 0; k < n; ++k)
                    {
                        _i += w[k, 0] * Tools.U(Tools.r(targetPoint[k], new System.Windows.Point(i, j)));
                        _j += w[k, 1] * Tools.U(Tools.r(targetPoint[k], new System.Windows.Point(i, j)));
                    }
                    if ((int)Math.Floor(_i) < 1 || (int)Math.Floor(_i) >= c - 2 || (int)Math.Floor(_j) < 1 || (int)Math.Floor(_j) >= r - 2)
                    {
                        for (int k = 0; k < 3; ++k)
                        {
                            map[1, i, j, k] = 0;
                        }
                    }
                    else
                    {
                        u[0] = Math.Ceiling(_i) - _i;
                        u[1] = _i - Math.Floor(_i);
                        v[0] = Math.Ceiling(_j) - _j;
                        v[1] = _j - Math.Floor(_j);
                        for (int t = 0; t < 4; ++t)
                        {
                            A[t] = S(u[1] + 1 - t);
                            C[t] = S(v[1] + 1 - t);
                        }
                        for (int x = 0; x < 4; ++x)
                        {
                            for (int y = 0; y < 4; ++y)
                            {
                                for (int z = 0; z < 3; ++z)
                                {
                                    B[x, y, z] = map[0, (int)Math.Floor(_i) - 1 + x, (int)Math.Floor(_j) - 1 + y, z];
                                }
                            }
                        }
                        for (int z = 0; z < 3; ++z)
                        {
                            temp = 0;
                            for (int x = 0; x < 4; ++x)
                            {
                                for (int y = 0; y < 4; ++y)
                                {
                                    temp += A[x] * B[x, y, z] * C[y];
                                }
                            }
                            map[1, i, j, z] = (int)temp;
                            if (map[1, i, j, z] > 255)
                                map[1, i, j, z] = 255;
                            if (map[1, i, j, z] < 0)
                                map[1, i, j, z] = 0;
                        }
                    }
                    colorForSet = System.Drawing.Color.FromArgb(map[1, i, j, 0], map[1, i, j, 1], map[1, i, j, 2]);
                    if (i >= 2 && i < c - 2 && j >= 2 && j < r - 2)
                        bmp.SetPixel(i - 2, j - 2, colorForSet);
                }
            }
            ip = bmp.GetHbitmap();
            bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ip, IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            img.Source = bitmapSource;
            excute.IsEnabled = false;
            clear.IsEnabled = false;
            reset.IsEnabled = true;
        }
        private double S(double x)
        {
            x = Math.Abs(x);
            if (x <= 1)
            {
                return (1 - 2 * Math.Pow(x, 2) + Math.Pow(x, 3));
            }
            else if (x < 2)
            {
                return (4 - 8 * x + 5 * Math.Pow(x, 2) - Math.Pow(x, 3));
            }
            else
                return 0;
        }
    }
}
