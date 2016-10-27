using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTwisting
{
    class Tools
    {
        internal static double U(double r)
        {
            return Math.Pow(r, 2) * Math.Log10(Math.Pow(r, 2));
        }
        internal static double r(System.Windows.Point p1, System.Windows.Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
        internal static double[,] times(double[,] a, int r1, int c1, double[,] b, int r2, int c2)
        {
            if(c1!=r2)
                return null;
            double[,] ans = new double[r1, c2];
            
            for (int i = 0; i < r1; ++i)
            {
                for (int j = 0; j < c2; ++j)
                {
                    ans[i, j] = 0;
                    for (int k = 0; k < c1; ++k)
                    {
                        ans[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return ans;
        }
        internal static double determinant(double[,] a, int r, int c)
        {
            if (r != c)
                return 0;
            if (r == 1)
            {
                return a[0, 0];
            }
            double lambda;
            bool signal = true;
            for (int i = 0; i < r; ++i)
            {
                if (a[i, i] == 0)
                {
                    for (int j = i + 1; j < r; ++j)
                    {
                        if (a[j, i] != 0)
                        {
                            for (int k = i; k < c; ++k)
                            {
                                double temp = a[i, k];
                                a[i, k] = a[j, k];
                                a[j, k] = temp;
                            }
                            signal = !signal;
                            break;
                        }
                    }
                }
                for (int j = i + 1; j < r; ++j)
                {
                    if (a[j, i] == 0)
                        continue;
                    lambda = a[j, i] / a[i, i];
                    for (int k = i + 1; k < c; ++k)
                    {
                        a[j, k] -= a[i, k] * lambda;
                    }
                }
            }
            double ans = signal ? 1 : -1;
            for (int i = 0; i < r; ++i)
                ans *= a[i, i];
            return ans;
        }
        internal static double[,] inverse(double[,] a, int r, int c)
        {
            double[,] ans = new double[r, c];
            double[,] up = new double[r - 1, c - 1];
            double down = determinant(a, r, c);
            for (int i = 0; i < r; ++i)
            {
                for (int j = 0; j < c; ++j)
                {
                    for (int y = 0; y < r - 1; ++y)
                    {
                        for (int x = 0; x < c - 1; ++x)
                        {
                            up[y, x] = a[y < i ? y : y + 1, x < j ? x : x + 1];
                        }
                    }
                    ans[i, j] = Math.Pow(-1, i + j) * determinant(up, r - 1, c - 1) / down;
                }
            }
            return ans;
        }
    }
}
