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
            fixPoint = new List<System.Windows.Point>();
            targetPoint = new List<System.Windows.Point>();
            fixPoint.Clear();
            targetPoint.Clear();
        }

        bool flag;
        List<System.Windows.Point> fixPoint, targetPoint;

        private void img_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point pointer = e.GetPosition(img);
            MessageBox.Show(pointer.X.ToString()+" "+pointer.Y.ToString());
            if (!flag)
            {
                flag = true;
                fixPoint.Add(pointer);
            }
            else
            {
                flag = false;
                targetPoint.Add(pointer);
            }
            excute.IsEnabled = !flag;
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            fixPoint.Clear();
            targetPoint.Clear();
            excute.IsEnabled = true;
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            fixPoint.Clear();
            targetPoint.Clear();
            excute.IsEnabled = true;
            clear.IsEnabled = true;
            img.Source = new BitmapImage(new Uri(imagesource, UriKind.Absolute));
        }
        int r, c;
        int[, , ,] map;
        string imagesource;
        private void choose_Click(object sender, RoutedEventArgs e)
        {
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
            map = new int[4, bmp.Width, bmp.Height, 3];
            r = bmp.Height;
            c = bmp.Width;
            img.Height = bmp.Height;
            img.Width = bmp.Width;
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

        double[,] L;
        private void excute_Click(object sender, RoutedEventArgs e)
        {
            clear.IsEnabled = false;
            int n = fixPoint.Count;
            L = new double[n + 3, n + 3];

        }

    }
}
