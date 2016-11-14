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
    /// Distort.xaml 的交互逻辑
    /// </summary>
    public partial class Distort : Window
    {
        public Distort()//图像畸变
        {
            InitializeComponent();
        }

        int r, c;
        int[, , ,] map;
        int maxR;
        int x0, y0;
        System.Drawing.Color colorForSet;
        Bitmap tempimg1, tempimg2, tempimg3;

        //选择图片
        private void chooseImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            op.InitialDirectory = @"c:\";
            op.RestoreDirectory = true;
            op.Filter = "图片文件|*.jpg;*.bmp;*.png";
            op.ShowDialog();
            path.Text = op.FileName;
            if (op.FileName == "")
                return;
            img0.Source = new BitmapImage(new Uri(op.FileName, UriKind.Absolute));
            Bitmap bmp = new Bitmap(op.FileName);
            System.Drawing.Color pixelColor;
            map = new int[4, bmp.Width, bmp.Height, 3];
            x0 = bmp.Width / 2;
            y0 = bmp.Height / 2;
            maxR = x0 < y0 ? x0 : y0 - 3;
            r = bmp.Height;
            c = bmp.Width;
            for (int i = 0; i < c; ++i)
            {
                for (int j = 0; j < r; ++j)
                {
                    pixelColor = bmp.GetPixel(i, j);
                    map[0, i, j, 0] = pixelColor.R;
                    map[0, i, j, 1] = pixelColor.G;
                    map[0, i, j, 2] = pixelColor.B;
                }
            }
        }

        IntPtr ip1, ip2, ip3;
        BitmapSource bitmapSource1, bitmapSource2, bitmapSource3;
        double _i, _j;
        double[] u, v, A, C;
        double[, ,] f2, B;

        private void execute_Click(object sender, RoutedEventArgs e)
        {
            tempimg1 = new Bitmap(c, r);
            tempimg2 = new Bitmap(c, r);
            tempimg3 = new Bitmap(c, r);
            u = new double[2];
            v = new double[2];
            f2 = new double[2, 2, 3];
            A = new double[4];
            C = new double[4];
            B = new double[4, 4, 3];

            double x_now, y_now;
            double lambda = intense.Value;
            for (int i = 0; i < c; ++i)
            {
                for (int j = 0; j < r; ++j)
                {
                    tempimg1.SetPixel(i, j, System.Drawing.Color.FromArgb(map[0, i, j, 0], map[0, i, j, 1], map[0, i, j, 2]));
                    tempimg2.SetPixel(i, j, System.Drawing.Color.FromArgb(map[0, i, j, 0], map[0, i, j, 1], map[0, i, j, 2]));
                    tempimg3.SetPixel(i, j, System.Drawing.Color.FromArgb(map[0, i, j, 0], map[0, i, j, 1], map[0, i, j, 2]));
                }
            }
            if (lambda == 0)
            {
            }
            else if (lambda > 0)//向外
            {
                lambda = 1 - lambda;
                for (int i = 0; i < c; ++i)
                {
                    for (int j = 0; j < r; ++j)
                    {
                        //最近邻插值
                        x_now = i - x0;
                        y_now = y0 - j;
                        _i = x0 + (i - x0) * Math.Sqrt(Math.Pow(lambda, 2) + (Math.Pow(x_now, 2) + Math.Pow(y_now, 2)) / Math.Pow(maxR, 2) * (1 - Math.Pow(lambda, 2)));
                        _j = y0 + (j - y0) * Math.Sqrt(Math.Pow(lambda, 2) + (Math.Pow(x_now, 2) + Math.Pow(y_now, 2)) / Math.Pow(maxR, 2) * (1 - Math.Pow(lambda, 2)));
                        if ((int)Math.Round(_i) < 0 || (int)Math.Round(_i) >= c || (int)Math.Round(_j) < 0 || (int)Math.Round(_j) >= r)
                        {
                            for (int k = 0; k < 3; ++k)
                            {
                                map[1, i, j, k] = 0;
                            }
                        }
                        else
                        {
                            for (int k = 0; k < 3; ++k)
                            {
                                map[1, i, j, k] = map[0, (int)Math.Round(_i), (int)Math.Round(_j), k];
                            }
                        }
                        colorForSet = System.Drawing.Color.FromArgb(map[1, i, j, 0], map[1, i, j, 1], map[1, i, j, 2]);
                        tempimg1.SetPixel(i, j, colorForSet);


                        //双线性插值
                        double temp;
                        if ((int)Math.Floor(_i) < 0 || (int)Math.Floor(_i) >= c - 1 || (int)Math.Floor(_j) < 0 || (int)Math.Floor(_j) >= r - 1)
                        {
                            for (int k = 0; k < 3; ++k)
                            {
                                map[2, i, j, k] = 0;
                            }
                        }
                        else
                        {
                            if (_i == x0 || _j == y0)
                            {
                                for (int k = 0; k < 3; ++k)
                                {
                                    map[2, i, j, k] = map[0, (int)_i, (int)_j, k];
                                }
                            }
                            else
                            {
                                u[0] = Math.Ceiling(_i) - _i;
                                u[1] = _i - Math.Floor(_i);
                                v[0] = Math.Ceiling(_j) - _j;
                                v[1] = _j - Math.Floor(_j);
                                for (int x = 0; x < 2; ++x)
                                {
                                    for (int y = 0; y < 2; ++y)
                                    {
                                        for (int z = 0; z < 3; ++z)
                                        {
                                            f2[x, y, z] = map[0, (int)Math.Floor(_i) + x, (int)Math.Floor(_j) + y, z];
                                        }
                                    }
                                }
                                for (int z = 0; z < 3; ++z)
                                {
                                    temp = 0;
                                    for (int x = 0; x < 2; ++x)
                                    {
                                        for (int y = 0; y < 2; ++y)
                                        {
                                            temp += u[x] * f2[x, y, z] * v[y];
                                        }
                                    }
                                    map[2, i, j, z] = (int)temp;
                                }
                            }
                        }
                        colorForSet = System.Drawing.Color.FromArgb(map[2, i, j, 0], map[2, i, j, 1], map[2, i, j, 2]);
                        tempimg2.SetPixel(i, j, colorForSet);


                        //双三次插值
                        if ((int)Math.Floor(_i) < 1 || (int)Math.Floor(_i) >= c - 2 || (int)Math.Floor(_j) < 1 || (int)Math.Floor(_j) >= r - 2)
                        {
                            for (int k = 0; k < 3; ++k)
                            {
                                map[3, i, j, k] = 0;
                            }
                        }
                        else
                        {
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
                                map[3, i, j, z] = (int)temp;
                                if (map[3, i, j, z] > 255)
                                    map[3, i, j, z] = 255;
                                if (map[3, i, j, z] < 0)
                                    map[3, i, j, z] = 0;
                            }
                        }
                        colorForSet = System.Drawing.Color.FromArgb(map[3, i, j, 0], map[3, i, j, 1], map[3, i, j, 2]);
                        tempimg3.SetPixel(i, j, colorForSet);
                    }
                }
            }
            else//向内
            {
                lambda = 1 + lambda;
                for (int i = 0; i < c; ++i)
                {
                    for (int j = 0; j < r; ++j)
                    {
                        //最近邻插值
                        x_now = i - x0;
                        y_now = y0 - j;
                        _i = x0 + (i - x0) / Math.Sqrt(Math.Pow(lambda, 2) + (Math.Pow(x_now, 2) + Math.Pow(y_now, 2)) / Math.Pow(maxR, 2) * (1 - Math.Pow(lambda, 2)));
                        _j = y0 + (j - y0) / Math.Sqrt(Math.Pow(lambda, 2) + (Math.Pow(x_now, 2) + Math.Pow(y_now, 2)) / Math.Pow(maxR, 2) * (1 - Math.Pow(lambda, 2)));
                        for (int k = 0; k < 3; ++k)
                        {
                            map[1, i, j, k] = map[0, (int)Math.Round(_i), (int)Math.Round(_j), k];
                        }
                        colorForSet = System.Drawing.Color.FromArgb(map[1, i, j, 0], map[1, i, j, 1], map[1, i, j, 2]);
                        tempimg1.SetPixel(i, j, colorForSet);


                        //双线性插值
                        double temp;
                        if (_i == x0 || _j == y0)
                        {
                            for (int k = 0; k < 3; ++k)
                            {
                                map[2, i, j, k] = map[0, (int)_i, (int)_j, k];
                            }
                        }
                        else
                        {
                            u[0] = Math.Ceiling(_i) - _i;
                            u[1] = _i - Math.Floor(_i);
                            v[0] = Math.Ceiling(_j) - _j;
                            v[1] = _j - Math.Floor(_j);
                            for (int x = 0; x < 2; ++x)
                            {
                                for (int y = 0; y < 2; ++y)
                                {
                                    for (int z = 0; z < 3; ++z)
                                    {
                                        f2[x, y, z] = map[0, (int)Math.Floor(_i) + x, (int)Math.Floor(_j) + y, z];
                                    }
                                }
                            }
                            for (int z = 0; z < 3; ++z)
                            {
                                temp = 0;
                                for (int x = 0; x < 2; ++x)
                                {
                                    for (int y = 0; y < 2; ++y)
                                    {
                                        temp += u[x] * f2[x, y, z] * v[y];
                                    }
                                }
                                map[2, i, j, z] = (int)temp;
                            }
                        }
                        colorForSet = System.Drawing.Color.FromArgb(map[2, i, j, 0], map[2, i, j, 1], map[2, i, j, 2]);
                        tempimg2.SetPixel(i, j, colorForSet);


                        //双三次插值
                        for (int t = 0; t < 4; ++t)
                        {
                            A[t] = S(u[1] + 1 - t);
                            C[t] = S(v[1] + 1 - t);
                        }
                        if ((int)Math.Floor(_i) < 1 || (int)Math.Floor(_j) < 1 || (int)Math.Floor(_i) > c - 3 || (int)Math.Floor(_j) > r - 3)
                            continue;
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
                            map[3, i, j, z] = (int)temp;
                            if (map[3, i, j, z] > 255)
                                map[3, i, j, z] = 255;
                            if (map[3, i, j, z] < 0)
                                map[3, i, j, z] = 0;
                        }
                        colorForSet = System.Drawing.Color.FromArgb(map[3, i, j, 0], map[3, i, j, 1], map[3, i, j, 2]);
                        tempimg3.SetPixel(i, j, colorForSet);
                    }
                }
            }
            ip1 = tempimg1.GetHbitmap();
            bitmapSource1 = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ip1, IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            img1.Source = bitmapSource1;

            ip2 = tempimg2.GetHbitmap();
            bitmapSource2 = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ip2, IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            img2.Source = bitmapSource2;

            ip3 = tempimg3.GetHbitmap();
            bitmapSource3 = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ip3, IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            img3.Source = bitmapSource3;
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
