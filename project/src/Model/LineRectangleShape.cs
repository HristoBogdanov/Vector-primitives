using System;
using System.Drawing;
using System.Net;
using System.Runtime.Serialization;

namespace Draw
{
	/// <summary>
	/// Класът правоъгълник е основен примитив, който е наследник на базовия Shape.
	/// </summary>
	[Serializable]
    public class LineRectangleShape : Shape
	{
		#region Constructor
		
		public LineRectangleShape(RectangleF rect) : base(rect)
		{
		}
		
		public LineRectangleShape(RectangleShape rectangle) : base(rectangle)
		{
		}

        protected LineRectangleShape(SerializationInfo info, StreamingContext context) : base(info, context)
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
            PointF pointOne = new PointF(Rectangle.X, Rectangle.Y + Height/2);
            PointF pointTwo = new PointF(Rectangle.X + Width/2, Rectangle.Y + Height/2);
            PointF pointThree = new PointF(Rectangle.X + Width/2, Rectangle.Y + Height/2);
            PointF pointFour = new PointF(Rectangle.X + Width, Rectangle.Y);
            PointF pointFive = new PointF(Rectangle.X + Width, Rectangle.Y + Height);
            grfx.FillRectangle(new SolidBrush(c), Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
			grfx.DrawRectangle(new Pen(StrokeColor, StrokeWidth),Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height);
            grfx.DrawLine(new Pen(StrokeColor, StrokeWidth), pointOne, pointTwo);
            grfx.DrawLine(new Pen(StrokeColor, StrokeWidth), pointThree, pointFour);
            grfx.DrawLine(new Pen(StrokeColor, StrokeWidth), pointThree, pointFive);
            grfx.Restore(state);
		}
	}
}
