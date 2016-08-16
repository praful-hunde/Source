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
    using System;
    using System.Diagnostics;    /// <summary>
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
            double markerSize = 5;
            // Create two line series (markers are hidden by default)
            var series1 = new LineSeries { Title = "Series 1", MarkerType = MarkerType.Circle, MarkerSize = markerSize, Selectable=true, SelectionMode= SelectionMode.Single };
            series1.Points.Add(new DataPoint(0, 0));
            series1.Points.Add(new DataPoint(10, 180));
            series1.Points.Add(new DataPoint(20, 120));
            series1.Points.Add(new DataPoint(30, 80));
            series1.Points.Add(new DataPoint(40, 150));

            var series2 = new LineSeries
            {
                Title = "Series 2",
                MarkerType = MarkerType.Square,
                MarkerStroke = OxyColors.Black,
                Color = OxyColors.DarkGray,
                Smooth = false,
                StrokeThickness = 1,
                HitTestTolerance = markerSize/2d,
                MarkerSize = markerSize,
              
            };

            series2.Points.Add(new DataPoint(0, 40));
            series2.Points.Add(new DataPoint(100, 120));
            series2.Points.Add(new DataPoint(200, 160));
            series2.Points.Add(new DataPoint(300, 250));
            series2.Points.Add(new DataPoint(400, 50));
            int indexOfPointToMove = -1;
            bool canDrag;
            ScreenPoint mouseDownPoint = ScreenPoint.Undefined;

            series2.MouseDown += (s, e) =>
            {
                e.Handled = true;
                var series = s as LineSeries;


                if (e.ChangedButton == OxyMouseButton.Left)
                {
                    int indexOfNearestPoint = (int)Math.Round(e.HitTestResult.Index);
                    var nearestPoint = series.Transform(series.Points[indexOfNearestPoint]);

                    var distanceFromDataPoint = (nearestPoint - e.Position).Length;
                    // Check if we are near a point
                    if (distanceFromDataPoint <= markerSize)
                    {
                        indexOfPointToMove = indexOfNearestPoint;
                        mouseDownPoint = e.Position;
                    }

                    if (distanceFromDataPoint >= markerSize)
                    {
                        if (e.ClickCount == 2)
                        {
                            if (series.IsSelected() == false)
                            {
                                // Start editing this point
                                series.Select();
                                tmp.PlotView.SetCursorType(CursorType.SizeNS);                               
                            }
                            else
                            {
                                series.Unselect();
                                tmp.PlotView.SetCursorType(CursorType.Default);
                            }
                            series.PlotModel.PlotView.InvalidatePlot(false);
                        }

                    }
                }
            };
            int count = 0;
            series2.MouseMove += (s, e) =>
            {
                Trace.WriteLine("Series Mouse move is fired" + (count++).ToString());
                e.Handled = true;
                if (indexOfPointToMove >= 0)
                {
                    // Move the point being edited.
                    var series = s as LineSeries;
                    if (e.Position.Equals(mouseDownPoint) == false)
                    {
                        series.Points[indexOfPointToMove] = series.InverseTransform(e.Position);
                        tmp.PlotView.SetCursorType(CursorType.Pan);
                        series.PlotModel.InvalidatePlot(false);
                        
                    }

                }
              
            };

            series2.MouseUp += (s, e) => {
                e.Handled = true;

                if (indexOfPointToMove != -1)
                {
                    indexOfPointToMove = -1;
                    tmp.PlotView.SetCursorType(CursorType.Default);
                }
              
                var series = s as LineSeries;

                if (e.Position.Equals(mouseDownPoint) == false && series.IsSelected() == true)
                {
                    //series.Unselect();
                    //tmp.PlotView.SetCursorType(CursorType.Default);
                    //series.PlotModel.PlotView.InvalidatePlot(false);
                    //mouseDownPoint = ScreenPoint.Undefined;
                }
              
            };

            tmp.MouseUp += (s, e) =>
            {
                //e.Handled = true;
                var controller = s as PlotController;


                foreach (LineSeries series in tmp.Series)
                {

                    if (series.IsSelected() == true)
                    {
                        series.Unselect();
                        //series.Color = OxyColors.Black;
                        //series.StrokeThickness /= 3;
                        series.PlotModel.PlotView.InvalidatePlot(false);
                        tmp.PlotView.SetCursorType(CursorType.Default);
                    }
                }
            };


           tmp.MouseDown += (s, e) => { /*e.Handled = true;*/ };

            // Add the series to the plot model
            tmp.Series.Add(series1);
            tmp.Series.Add(series2);
            var arrowAnnotation = new LineAnnotation
            {
                X = 20, Y = 10, LineStyle = LineStyle.Solid, Selectable = true, SelectionMode = SelectionMode.All,
                Type = LineAnnotationType.Vertical, Color = OxyColors.LightGray, StrokeThickness = 5

            };

            tmp.Annotations.Add(arrowAnnotation);
            var tappet = new TappetAnnotation
            {
                MinimumX = 40,
                MinimumY = 1,
                MaximumX = 80,
                MaximumY = 41,
                Fill = OxyColors.Gray,
                Stroke = OxyColors.Black,
                StrokeThickness = 0.9
            };

            //tappet.MouseDown += Tappet_MouseDown;
            //tappet.MouseUp += Tappet_MouseUp;
            //tappet.MouseMove += Tappet_MouseMove;
            tmp.Annotations.Add(tappet);
            tmp.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                IsPanEnabled = true,
                FractionUnit = 90,
                FractionUnitSymbol = "0",
                Minimum = 0,
                Maximum = 360,
                MajorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.LightGray,
                // LabelFormatter = _ =>  string.Empty ,
                MajorStep = 20,
                Title = "Master /°",
                TitlePosition = 0.8,
            });
            tmp.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                MajorStep = 40,
                IsPanEnabled = true,
                FractionUnit = 1,
                FractionUnitSymbol = "0",
                Minimum = 0,
                Maximum = 360,
                MajorGridlineColor = OxyColors.LightGray,
                MajorGridlineStyle = LineStyle.Dot,
                Title = "Slave /°",
                TitlePosition = 0.8,
            });
            //tmp.Axes.Add(new LinearAxis { Position = AxisPosition.Left, IsPanEnabled = true, IsAxisVisible = true });
            // Axes are created automatically if they are not defined
            tmp.SelectionColor = OxyColors.Black;
            tmp.DisablePanOnPlotArea = true;
           
            // Set the Model property, the INotifyPropertyChanged event will make the WPF Plot control update its content
            this.Model = tmp;
        }

        private void Tappet_MouseUp(object sender, OxyMouseEventArgs e)
        {
            var tappet = sender as TappetAnnotation;
            canDrag = false;
            //throw new NotImplementedException();
            tappet.PlotModel.PlotView.SetCursorType(CursorType.Default);
            e.Handled = true;
        }

        private void Tappet_MouseMove(object sender, OxyMouseEventArgs e)
        {
          
            if (canDrag == false) return;
            var tappet = (sender as TappetAnnotation);
            tappet.Text = (e.Position).ToString();
            e.Handled = true;
            var currentLocation = Axis.InverseTransform(e.Position, tappet.XAxis, tappet.YAxis);
            var diff = currentLocation.X - lastMouseLocation.X;

            if (diff == 0) return;

            var direction = diff > 0 ? Direction.Right : Direction.Left;

            if (tappet.MaximumX <= tappet.MinimumX)
            {
                if (location == TappetHitTestLocation.LeftControlPoint && direction == Direction.Right)
                    location = TappetHitTestLocation.RightControlPoint;
                if (location == TappetHitTestLocation.RightControlPoint && direction == Direction.Left)
                    location = TappetHitTestLocation.LeftControlPoint;
            }

            if (location == TappetHitTestLocation.RightControlPoint)
            {
                var refrence = tappet.MaximumX;//tappet.MaximumX >= tappet.MinimumX ? tappet.MaximumX : tappet.MinimumX;
                var newMaxX = refrence + diff;
                tappet.MaximumX = newMaxX > tappet.XAxis.Maximum ? tappet.XAxis.Maximum : newMaxX;
            }
            else if (location == TappetHitTestLocation.LeftControlPoint)
            {
                var refrence = tappet.MinimumX;//tappet.MinimumX <= tappet.MaximumX ? tappet.MinimumX : tappet.MaximumX;
                var newMinX = refrence + diff;
                tappet.MinimumX = newMinX < tappet.XAxis.Minimum ? tappet.XAxis.Minimum : newMinX;
            }
            else if (location == TappetHitTestLocation.Tappet)
            {
                if (direction == Direction.Right)//Dragging right
                {
                    var refrence = tappet.MaximumX > tappet.MinimumX ? tappet.MaximumX : tappet.MinimumX;
                    var maxDiff = tappet.XAxis.Maximum - refrence;

                    if (diff > maxDiff)
                    {
                        diff = maxDiff;
                    }
                }
                else if (direction == Direction.Left)//Dragging left
                {
                    var refrence = tappet.MaximumX < tappet.MinimumX ? tappet.MaximumX : tappet.MinimumX;
                    var maxDiff = tappet.XAxis.Minimum - refrence;
                    if (diff < maxDiff)
                    {
                        diff = maxDiff;
                    }
                }
                else
                {
                    diff = 0;
                }

                tappet.MinimumX += diff;
                tappet.MaximumX += diff;
            }

            tappet.PlotModel.PlotView.InvalidatePlot(false);
            lastMouseLocation = currentLocation;
            Debug.WriteLine("Mouse_Move..............." + tappet.MaximumX);
        }

        bool canDrag;
        DataPoint lastMouseLocation;
        TappetHitTestLocation location;
        private void Tappet_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            var tappet = (sender as TappetAnnotation);
            tappet.Text = ( e.Position).ToString();
            canDrag = true;
            lastMouseLocation = Axis.InverseTransform(e.Position, tappet.XAxis, tappet.YAxis);
            var hitTestResult = e.HitTestResult as TappetHitTestResult;
            location = hitTestResult.TappetLocation;

            if (location == TappetHitTestLocation.LeftControlPoint || location == TappetHitTestLocation.RightControlPoint)
            {
                tappet.PlotModel.PlotView.SetCursorType(CursorType.Pan);
            }
            tappet.PlotModel.PlotView.InvalidatePlot(false);
            e.Handled = true;
        }

        /// <summary>
        /// Gets the plot model.
        /// </summary>
        public PlotModel Model { get; private set; }
    }
}