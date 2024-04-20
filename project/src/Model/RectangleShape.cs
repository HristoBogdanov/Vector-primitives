using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace Draw
{
	/// <summary>
	/// Класът правоъгълник е основен примитив, който е наследник на базовия Shape.
	/// </summary>
	[Serializable]
    public class RectangleShape : Shape
	{
		#region Constructor
		
		public RectangleShape(RectangleF rect) : base(rect)
		{
		}
		
		public RectangleShape(RectangleShape rectangle) : base(rectangle)
		{
		}

        protected RectangleShape(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion

        /// <summary>
        /// Проверка за принадлежност на точка point към правоъгълника.
        /// В случая на правоъгълник този метод може да не бъде пренаписван, защото
        /// Реализацията съвпада с тази на абстрактния клас Shape, който проверява
        /// дали точката е в обхващащия правоъгълник на елемента (а той съвпада с
        /// елемента в този случай).
        /// </summary>
        public override bool Contains(PointF point)
		{
            //Matrix inverseMatrix = TransformationMatrix.Clone();
            //inverseMatrix.Invert();
            //PointF[] points = { point };
            //inverseMatrix.TransformPoints(points);
            //PointF transformedPoint = points[0];

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
			grfx.FillRectangle(new SolidBrush(c), Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
			grfx.DrawRectangle(new Pen(StrokeColor, StrokeWidth),Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
			grfx.Restore(state);
		}
	}
}
