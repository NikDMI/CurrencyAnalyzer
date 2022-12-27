using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client.Model;
using System.Globalization;

namespace Client.CustomControls
{
    
    public class LinearChart : FrameworkElement
    {
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            double h = (double)this.Parent.GetValue(Canvas.HeightProperty);
            double w = (double)this.Parent.GetValue(Canvas.WidthProperty);
            drawingContext.DrawRectangle(_backgroundColor, null, new Rect(0, 0, w, h));
            //
            DrawAxis(w, h, drawingContext);
        }


        //Set new rates to display
        internal void UpdateRates(List<Rate> newRates)
        {
            _currentRates.Clear();
            _currentRates.AddRange(newRates);
            //Sort rates by date
            _currentRates.Sort((rate1, rate2) =>
            {
                if (rate1.Date == rate2.Date)
                {
                    return 0;
                }
                else
                {
                    return rate1.Date < rate2.Date ? -1 : 1;
                }
            });
            //Get max/min currency values
            _upperValue = -1; _lowerValue = Double.MaxValue;
            _currentRates.ForEach(rate =>
            {
                if (rate.Value > _upperValue) _upperValue = rate.Value;
                if (rate.Value < _lowerValue) _lowerValue = rate.Value;
            });
            this.InvalidateVisual();
        }


        //Draws corresponding axis
        private void DrawAxis(double clientW, double clientH, DrawingContext drawingContext)
        {
            Rect axisRect = new Rect(TEXT_AXIS_SPACE, TEXT_AXIS_SPACE, clientW - 2 * TEXT_AXIS_SPACE, clientH - 2 * TEXT_AXIS_SPACE);
            drawingContext.DrawLine(_linePen, new Point { X = axisRect.Left, Y = axisRect.Top }, new Point { X = axisRect.Left, Y = axisRect.Bottom });
            drawingContext.DrawLine(_linePen, new Point { X = axisRect.Left, Y = axisRect.Bottom }, new Point { X = axisRect.Right, Y = axisRect.Bottom });
            if (_currentRates.Count >= 2)
            {
                //Draw Y axis strikes
                double yStep = axisRect.Height / (Y_AXIS_INTERVALS + 1);
                double valueStep = (_lowerValue - _upperValue) / (Y_AXIS_INTERVALS - 1);
                double currentY = axisRect.Top + yStep; double currentValue = _upperValue;
                for (int i = 0; i < Y_AXIS_INTERVALS; ++i)
                {
                    drawingContext.DrawLine(_linePen, new Point { X = axisRect.Left, Y = currentY },
                        new Point { X = axisRect.Left + TEXT_STRIKE_WIDTH, Y = currentY });
                    FormattedText text = new FormattedText((Math.Truncate(currentValue * 100) / 100).ToString(),
                        CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                        new Typeface("Verdana"), 10, Brushes.Black);
                    drawingContext.DrawText(text, new Point { X = axisRect.Left - TEXT_AXIS_SPACE, Y = currentY });
                    currentY += yStep;
                    currentValue += valueStep;
                }
                //Draw X axis
                int xAxisStrikesCount = _currentRates.Count;
                var timePeriod = _currentRates[xAxisStrikesCount - 1].Date - _currentRates[0].Date;
                double xStep = axisRect.Width / (xAxisStrikesCount + 1);
                if (xStep < TEXT_AXIS_SPACE)
                {
                    xStep = TEXT_AXIS_SPACE;
                    xAxisStrikesCount = (int)(axisRect.Width / xStep) - 1;
                }
                TimeSpan dateStep = timePeriod / (xAxisStrikesCount - 1);
                double currentX = axisRect.Left + xStep;
                DateTime currenTime = new DateTime(_currentRates[0].Date.ToBinary());
                for (int i = 0; i < xAxisStrikesCount; ++i)
                {
                    drawingContext.DrawLine(_linePen, new Point { X = currentX, Y = axisRect.Bottom },
                        new Point { X = currentX, Y = axisRect.Bottom - TEXT_STRIKE_WIDTH });
                    FormattedText text = new FormattedText(currenTime.ToString("dd.MM.yy"),
                        CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                        new Typeface("Verdana"), 10, Brushes.Black);
                    drawingContext.DrawText(text, new Point { X = currentX - TEXT_AXIS_SPACE / 2, Y = axisRect.Bottom });
                    currentX += xStep;
                    currenTime += dateStep;
                }
                //DRAW FUNCTION
                DrawFunction(new Rect(axisRect.Left + xStep, axisRect.Top + yStep, axisRect.Width - 2 * xStep, axisRect.Height - 2 * yStep),
                    -valueStep, dateStep, drawingContext);
            }
        }


        //Draws function in the function rect areat
        private void DrawFunction(Rect functionArea, double yValueStep, TimeSpan xDateStep, DrawingContext drawingContext)
        {
            double yPixelsPerStep = functionArea.Height / ((_upperValue - _lowerValue) / yValueStep);
            DateTime lowerTime = _currentRates[0].Date;
            DateTime upperTime = _currentRates[_currentRates.Count - 1].Date;
            double xPixelsPerStep = functionArea.Width / ((upperTime - lowerTime) / xDateStep);
            //Get start point
            double x = ((_currentRates[0].Date - lowerTime) / xDateStep) * xPixelsPerStep;
            double y = ((_currentRates[0].Value - _lowerValue) / yValueStep) * yPixelsPerStep;
            Point startPoint = new Point(functionArea.Left + x, functionArea.Bottom - y);
            for (int i = 1; i < _currentRates.Count; ++i)
            {
                Rate rate = _currentRates[i];
                x = ((rate.Date - lowerTime) / xDateStep) * xPixelsPerStep;
                y = ((rate.Value - _lowerValue) / yValueStep) * yPixelsPerStep;
                Point nextPoint = new Point(functionArea.Left + x, functionArea.Bottom - y);
                drawingContext.DrawLine(_functionPen, startPoint, nextPoint);
                startPoint = nextPoint;
            }
        }

        private List<Rate> _currentRates = new List<Rate>();

        private double _upperValue = 1;

        private double _lowerValue = 0;

        private const int Y_AXIS_INTERVALS = 10;    //how much intervals in currency value

        private const int TEXT_AXIS_SPACE = 50; //how much pixels to text

        private const int TEXT_STRIKE_WIDTH = 5; //width of the strike at axis


        private SolidColorBrush _backgroundColor = new SolidColorBrush(Color.FromRgb(203, 200, 203));

        private Pen _linePen = new Pen(new SolidColorBrush(Color.FromRgb(20, 20, 20)), 1);

        private Pen _functionPen = new Pen(new SolidColorBrush(Color.FromRgb(40, 20, 20)), 1);
    }
    
}
