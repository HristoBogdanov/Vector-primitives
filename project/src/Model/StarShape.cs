using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace Draw.src.Model
{
    [Serializable]
    public class StarShape : Shape
    {
        public StarShape(RectangleF rect) : base(rect)
        {
        }

        protected StarShape(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        public override bool Contains(PointF point)
        {
            Matrix inverseTransformationMatrix = TransformationMatrix.Clone();
            inverseTransformationMatrix.Invert();
            PointF[] localPoints = { point };
            inverseTransformationMatrix.TransformPoints(localPoints);
            PointF transformedPoint = localPoints[0];

            PointF[] starPoints = CalculateStarPoints(Location, Width, Height, 5);
            int numPoints = starPoints.Length / 2;

            for (int i = 0; i < numPoints; i++)
            {
                int nextIndex = (i + 1) % numPoints;

                if (PointInTriangle(transformedPoint, starPoints[i], starPoints[nextIndex], starPoints[i + numPoints]))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool PointInTriangle(PointF P, PointF A, PointF B, PointF C)
        {
            var w1 = (A.X * (C.Y - A.Y) + (P.Y - A.Y) * (C.X - A.X) - P.X * (C.Y - A.Y)) /
                ((B.Y - A.Y)*(C.X - A.X) - (B.X - A.X)*(C.Y - A.Y));

            var w2 = (P.Y - A.Y - w1 * (B.Y - A.Y)) /
                (C.Y - A.Y);

            if(w1 >= 0 && w2 >= 0 && w1 + w2 <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
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

            PointF[] starPoints = CalculateStarPoints(Location, Width, Height, 5);

            grfx.FillPolygon(new SolidBrush(c), starPoints);
            grfx.DrawPolygon(new Pen(StrokeColor, StrokeWidth), starPoints);

            grfx.Restore(state);
        }

        public PointF[] CalculateStarPoints(PointF center, float width, float height, int numPoints)
        {
            PointF[] points = new PointF[numPoints * 2];
            double startAngle = -Math.PI / 2;

            double angleBetweenPoints = Math.PI / numPoints;

            float outerRadius = Math.Min(width, height) / 2;
            float innerRadius = outerRadius / 2;

            for (int i = 0; i < numPoints * 2; i++)
            {
                float radius = (i % 2 == 0) ? outerRadius : innerRadius;

                points[i] = new PointF(
                    center.X + (float)(radius * Math.Cos(startAngle)),
                    center.Y + (float)(radius * Math.Sin(startAngle))
                );

                startAngle += angleBetweenPoints;
            }

            return points;
        }
    }
}
