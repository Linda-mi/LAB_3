using System;
using System.Collections.Generic;

namespace InterpolationLab.Models
{
    
    public abstract class Interpolation
    {
        public abstract double Calculate(IReadOnlyList<PointD> points, double x);

        
        protected void ValidatePoints(IReadOnlyList<PointD> points)
        {
            if (points == null || points.Count < 2)
                throw new ArgumentException("Потрібно мінімум 2 точки!");

            for (int i = 0; i < points.Count - 1; i++)
            {
                if (Math.Abs(points[i].X - points[i + 1].X) < 1e-9)
                    throw new InvalidOperationException($"Заборонено однакові X! Точка X = {points[i].X}");

                if (points[i].X > points[i + 1].X)
                    throw new InvalidOperationException("Точки мають бути відсортовані за зростанням X.");
            }
        }
    }

    
    public sealed class LinearInterpolation : Interpolation
    {
        public override double Calculate(IReadOnlyList<PointD> points, double x)
        {
            ValidatePoints(points);

            if (x <= points[0].X) return points[0].Y;
            if (x >= points[points.Count - 1].X) return points[points.Count - 1].Y;

            for (int i = 0; i < points.Count - 1; i++)
            {
                if (x >= points[i].X && x <= points[i + 1].X)
                {
                    double t = (x - points[i].X) / (points[i + 1].X - points[i].X);
                    return points[i].Y + t * (points[i + 1].Y - points[i].Y);
                }
            }
            return 0;
        }
    }

    
    public sealed class HermiteInterpolation : Interpolation
    {
        public override double Calculate(IReadOnlyList<PointD> points, double x)
        {
            ValidatePoints(points);
            int n = points.Count;

            if (x <= points[0].X) return points[0].Y;
            if (x >= points[n - 1].X) return points[n - 1].Y;

           
            double[] m = new double[n];
            for (int i = 0; i < n; i++)
            {
                if (i == 0)
                    m[i] = (points[1].Y - points[0].Y) / (points[1].X - points[0].X);
                else if (i == n - 1)
                    m[i] = (points[n - 1].Y - points[n - 2].Y) / (points[n - 1].X - points[n - 2].X);
                else
                    m[i] = (points[i + 1].Y - points[i - 1].Y) / (points[i + 1].X - points[i - 1].X);
            }

            for (int i = 0; i < n - 1; i++)
            {
                if (x >= points[i].X && x <= points[i + 1].X)
                {
                    double dx = points[i + 1].X - points[i].X;
                    double t = (x - points[i].X) / dx;

                    double h00 = 2 * t * t * t - 3 * t * t + 1;
                    double h10 = t * t * t - 2 * t * t + t;
                    double h01 = -2 * t * t * t + 3 * t * t;
                    double h11 = t * t * t - t * t;

                    return h00 * points[i].Y + h10 * dx * m[i] + h01 * points[i + 1].Y + h11 * dx * m[i + 1];
                }
            }
            return 0;
        }
    }
}