using System;
using System.Collections.Generic;
using System.Windows.Media;
using InterpolationLab.Models;

namespace InterpolationLab.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private double _h1 = 1.0;
        private double _h2 = 0.1;
        private string? _errorMessage;

        private bool _showOriginal = true;
        private bool _showLinear = true;
        private bool _showHermite = true;
        private bool _showErrorPlot = true;

        private PointCollection? _originalPoints;
        private PointCollection? _linearPoints;
        private PointCollection? _hermitePoints;
        private PointCollection? _errorPoints;

        public double H1 { get => _h1; set { _h1 = value; OnPropertyChanged(); Recalculate(); } }
        public double H2 { get => _h2; set { _h2 = value; OnPropertyChanged(); Recalculate(); } }
        public string? ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        public bool ShowOriginal { get => _showOriginal; set { _showOriginal = value; OnPropertyChanged(); } }
        public bool ShowLinear { get => _showLinear; set { _showLinear = value; OnPropertyChanged(); } }
        public bool ShowHermite { get => _showHermite; set { _showHermite = value; OnPropertyChanged(); } }
        public bool ShowErrorPlot { get => _showErrorPlot; set { _showErrorPlot = value; OnPropertyChanged(); } }

        public PointCollection? OriginalPoints { get => _originalPoints; set { _originalPoints = value; OnPropertyChanged(); } }
        public PointCollection? LinearPoints { get => _linearPoints; set { _linearPoints = value; OnPropertyChanged(); } }
        public PointCollection? HermitePoints { get => _hermitePoints; set { _hermitePoints = value; OnPropertyChanged(); } }
        public PointCollection? ErrorPoints { get => _errorPoints; set { _errorPoints = value; OnPropertyChanged(); } }
        public MainViewModel()
        {
            Recalculate();
        }

        private void Recalculate()
        {
            if (H2 >= H1)
            {
                ErrorMessage = "Помилка: h2 має бути строго менше за h1!";
                return;
            }
            if (H1 <= 0 || H2 <= 0)
            {
                ErrorMessage = "Кроки повинні бути більше 0!";
                return;
            }

            ErrorMessage = string.Empty;

            double xMax = 12.0; 

            List<PointD> A1 = new List<PointD>();
            for (double x = 0; x <= xMax + H1; x += H1)
            {
                A1.Add(new PointD(x, Math.Sin(x)));
            }

            
            double actualMaxX = A1[A1.Count - 1].X;

            Interpolation[] interpolators = new Interpolation[]
            {
        new LinearInterpolation(),
        new HermiteInterpolation()
            };

            var originalColl = new PointCollection();
            var linearColl = new PointCollection();
            var hermiteColl = new PointCollection();
            var errorColl = new PointCollection();

            double scaleX = 55, scaleY = 80, offsetY = 180;

            
            for (double x = 0; x <= actualMaxX; x += 0.05)
            {
                originalColl.Add(new System.Windows.Point(x * scaleX, offsetY - Math.Sin(x) * scaleY));
            }

            
            for (double x = 0; x <= actualMaxX; x += H2)
            {
                double yLinear = interpolators[0].Calculate(A1, x);
                double yHermite = interpolators[1].Calculate(A1, x);
                double yExact = Math.Sin(x);

                linearColl.Add(new System.Windows.Point(x * scaleX, offsetY - yLinear * scaleY));
                hermiteColl.Add(new System.Windows.Point(x * scaleX, offsetY - yHermite * scaleY));

                double err = Math.Abs(yHermite - yExact);
                errorColl.Add(new System.Windows.Point(x * scaleX, offsetY - err * scaleY * 4));
            }

            OriginalPoints = originalColl;
            LinearPoints = linearColl;
            HermitePoints = hermiteColl;
            ErrorPoints = errorColl;
        }
    }
}