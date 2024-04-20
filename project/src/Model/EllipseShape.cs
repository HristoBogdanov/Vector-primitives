using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace Draw.src.Model
{
    [Serializable]
    public class EllipseShape : Shape
    {
        public EllipseShape(RectangleF rect) : base(rect)
        {
        }

        protected EllipseShape(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override bool Contains(PointF point)
        {
            Matrix inverseMatrix = TransformationMatrix.Clone();
            inverseMatrix.Invert();
            PointF[] points = { point };
            inverseMatrix.TransformPoints(points);
            PointF transformedPoint = points[0];

            float a = Width / 2;
            float b = Height / 2;
            float xc = Location.X + a;
            float yc = Location.Y + b;
            return Math.Pow((transformedPoint.X - xc) / a, 2) + Math.Pow((transformedPoint.Y - yc) / b, 2) - 1 <= 0;
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
            grfx.FillEllipse(new SolidBrush(c), Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
            grfx.DrawEllipse(new Pen(StrokeColor, StrokeWidth), Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
            grfx.Restore(state);
        }
    }
}
