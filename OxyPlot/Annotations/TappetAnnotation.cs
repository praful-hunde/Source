using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxyPlot.Annotations
{
    public class TappetAnnotation : RectangleAnnotation
    {
        ScreenPoint[] CustomOutline = new[]
                             {
                                    new ScreenPoint(-1, -1), new ScreenPoint(1, 1), new ScreenPoint(-1, 1), new ScreenPoint(1, -1)
                             };

        OxyRect leftControlPointRect;
        OxyRect rightControlPointRect;

        bool canDrag;
        DataPoint lastMouseLocation;
        TappetHitTestLocation location;

        public TappetAnnotation()
        {
            MarkerSize = 4d;

            this.MouseDown += Tappet_MouseDown;
            this.MouseUp += Tappet_MouseUp;
            this.MouseMove += Tappet_MouseMove;
        }
        public double MarkerSize { get; set; }

        public override void Render(IRenderContext rc)
        {
            base.Render(rc);


            var screenPointList = new System.Collections.Generic.List<ScreenPoint>(2);
            var leftControlPoint = new ScreenPoint(screenRectangle.Left, screenRectangle.Top + screenRectangle.Height / 2d);
            var rightControlPoint = new ScreenPoint(screenRectangle.Left + screenRectangle.Width, screenRectangle.Top + screenRectangle.Height / 2d);

            leftControlPointRect = new OxyRect(leftControlPoint.X - MarkerSize, leftControlPoint.Y - MarkerSize, MarkerSize * 2, MarkerSize * 2);
            rightControlPointRect = new OxyRect(rightControlPoint.X - MarkerSize, rightControlPoint.Y - MarkerSize, MarkerSize * 2, MarkerSize * 2);

            screenPointList.Add(leftControlPoint);
            screenPointList.Add(rightControlPoint);

            int i = 0;
            var color  = OxyColors.Gainsboro;
            foreach (var point in screenPointList)
            {
                
                if (i++ == 1)
                {
                    color = OxyColors.Red;
                }

                rc.DrawMarker(clippingRectangle, point, MarkerType.Square, CustomOutline, MarkerSize, color, OxyColors.Black, 0.6);
            }

        }

        protected override HitTestResult HitTestOverride(HitTestArguments args)
        {
            //if (this.screenRectangle.Inflate(new OxyThickness(this.MarkerSize)).Contains(args.Point))
            //{
            //    return new HitTestResult(this, args.Point);
            //}
            //if (this.screenRectangle.Contains(args.Point))
            //{
            //    return new HitTestResult(this, args.Point);
            //}

            if (leftControlPointRect.Contains(args.Point))
            {
                return new TappetHitTestResult(this, args.Point) { TappetLocation = TappetHitTestLocation.LeftControlPoint};
            }
            else if (rightControlPointRect.Contains(args.Point))
            {
                return new TappetHitTestResult(this, args.Point) { TappetLocation = TappetHitTestLocation.RightControlPoint };
            }
            else if (this.screenRectangle.Contains(args.Point))
            {
                return new TappetHitTestResult(this, args.Point) { TappetLocation = TappetHitTestLocation.Tappet };
            }

            return null;
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
            //tappet.Text = (e.Position).ToString();
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
            //Debug.WriteLine("Mouse_Move..............." + tappet.MaximumX);
        }

        private void Tappet_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            var tappet = (sender as TappetAnnotation);
           // tappet.Text = (e.Position).ToString();
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

    }

    public class TappetHitTestResult : HitTestResult
    {
        public TappetHitTestResult(UIElement element, ScreenPoint nearestHitPoint, object item = null, double index = 0) : base(element, nearestHitPoint, item, index)
        {
        }

        public TappetHitTestLocation TappetLocation { get; set; }
    }

    public enum TappetHitTestLocation
    {
        Tappet,LeftControlPoint,RightControlPoint
    }

    public enum Direction
    {
        Left,Right
    }


   
    
}
