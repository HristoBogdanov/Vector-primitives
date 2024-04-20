using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace Draw.src.Model
{
    [Serializable]
    public class PolygonShape : Shape
    {
        public PolygonShape(RectangleF rect) : base(rect)
        {
        }

        protected PolygonShape(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override bool Contains(PointF point)
        {
            Matrix inverseMatrix = TransformationMatrix.Clone();
            inverseMatrix.Invert();
            PointF[] points = { point };
            inverseMatrix.TransformPoints(points);
            PointF transformedPoint = points[0];

            PointF[] polygonPoints = CalculatePolygonPoints(Location, Width, 5);

            bool containsPoint = false;
            var prevPoint = polygonPoints[polygonPoints.Length - 1];

            foreach (var currPoint in polygonPoints)
            {
                bool doesRayCrossBasedOnYAxis = (currPoint.Y > transformedPoint.Y) != (prevPoint.Y > transformedPoint.Y);
                bool doesRayCrossBasedOnXAxis = transformedPoint.X < (prevPoint.X - currPoint.X) * (transformedPoint.Y - currPoint.Y) / (prevPoint.Y - currPoint.Y) + currPoint.X;

                if (doesRayCrossBasedOnYAxis && doesRayCrossBasedOnXAxis)
                {
                    containsPoint = !containsPoint;
                }

                prevPoint = currPoint;
            }

            return containsPoint;
        }

        /// <summary>
        /// Частта, визуализираща конкретния примитив.
        /// </summary>
        public override void DrawSelf(Graphics grfx)
        {
            Color c = Color.FromArgb(Opacity, FillColor);
            base.DrawSelf(grfx);
            var state = grfx.Save();
            grfx.Transform = TransformationMatrix;

            PointF[] polygonPoints = CalculatePolygonPoints(Location, Width, 5);

            grfx.FillPolygon(new SolidBrush(c), polygonPoints);
            grfx.DrawPolygon(new Pen(StrokeColor, StrokeWidth), polygonPoints);

            grfx.Restore(state);
        }

        public PointF[] CalculatePolygonPoints(PointF center, float radius, int numSides)
        {
            PointF[] points = new PointF[numSides];
            double angleBetweenPoints = 2 * Math.PI / numSides;

            for (int i = 0; i < numSides; i++)
            {
                double angle = i * angleBetweenPoints;
                points[i] = new PointF(
                    center.X + (float)(radius * Math.Cos(angle)),
                    center.Y + (float)(radius * Math.Sin(angle))
                );
            }

            return points;
        }
    }
}
