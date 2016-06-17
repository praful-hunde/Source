// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Represents the view-model for the main window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimpleDemo
{
    using OxyPlot;
    using OxyPlot.Annotations;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    /// <summary>
    /// Represents the view-model for the main window.
    /// </summary>
    public class MainViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            // Create the plot model
            var tmp = new PlotModel { Title = "Simple example", Subtitle = "using OxyPlot", LegendBorderThickness = 8 , LegendMargin = 8};
          
            // Create two line series (markers are hidden by default)
            var series1 = new LineSeries { Title = "Series 1", MarkerType = MarkerType.Circle };
            series1.Points.Add(new DataPoint(0, 0));
            series1.Points.Add(new DataPoint(10, 18));
            series1.Points.Add(new DataPoint(20, 12));
            series1.Points.Add(new DataPoint(30, 8));
            series1.Points.Add(new DataPoint(40, 15));

            var series2 = new LineSeries { Title = "Series 2", MarkerType = MarkerType.Square , Selectable = true };
            series2.Points.Add(new DataPoint(0, 4));
            series2.Points.Add(new DataPoint(10, 12));
            series2.Points.Add(new DataPoint(20, 16));
            series2.Points.Add(new DataPoint(30, 25));
            series2.Points.Add(new DataPoint(40, 5));

            // Add the series to the plot model
            tmp.Series.Add(series1);
            tmp.Series.Add(series2);
            var arrowAnnotation = new LineAnnotation
            {
                X = 10, Y = 10, LineStyle = LineStyle.Solid, Selectable = true, SelectionMode = SelectionMode.All,
                Type = LineAnnotationType.Vertical

            };

            tmp.Annotations.Add(arrowAnnotation);
            tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, IsPanEnabled = true});
            tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Left, IsPanEnabled = true });
            tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Left, IsPanEnabled = true  , IsAxisVisible=false});
            // Axes are created automatically if they are not defined
            tmp.SelectionColor = OxyColors.Red;
            // Set the Model property, the INotifyPropertyChanged event will make the WPF Plot control update its content
            this.Model = tmp;
        }

        /// <summary>
        /// Gets the plot model.
        /// </summary>
        public PlotModel Model { get; private set; }


    }
}