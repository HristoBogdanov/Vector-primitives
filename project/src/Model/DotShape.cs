using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace Draw.src.Model
{
    [Serializable]
    public class DotShape : Shape
    {
        public DotShape(RectangleF rect) : base(rect)
        {
        }

        protected DotShape(SerializationInfo info, StreamingContext context) : base(info, context)
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

            PointF dotCenter = new PointF(Location.X + Width / 2, Location.Y + Height / 2);
            float dotRadius = 2.0f;

            grfx.FillEllipse(new SolidBrush(StrokeColor), dotCenter.X - dotRadius, dotCenter.Y - dotRadius, 2 * dotRadius, 2 * dotRadius);

            grfx.Restore(state);
        }
    }
}
