using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace Draw.src.Model
{
    [Serializable]
    public class LineShape : Shape
    {
        public LineShape(RectangleF rect) : base(rect)
        {
        }

        protected LineShape(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override bool Contains(PointF point)
        {
            if (base.Contains(point))
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

            PointF startPoint = new PointF(Location.X, Location.Y);
            PointF endPoint = new PointF(Location.X + Width, Location.Y);

            grfx.DrawLine(new Pen(StrokeColor, StrokeWidth), startPoint, endPoint);

            grfx.Restore(state);
        }
    }
}
