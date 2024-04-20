using Draw.src.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;

namespace Draw
{
	/// <summary>
	/// Класът, който ще бъде използван при управляване на диалога.
	/// </summary>
	public class DialogProcessor : DisplayProcessor
	{
		#region Constructor
		
		public DialogProcessor()
		{
		}
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// Избран елемент.
		/// </summary>
		private List<Shape> selection = new List<Shape>();
		public List<Shape> Selection {
			get { return selection; }
			set { selection = value; }
		}
		
		/// <summary>
		/// Дали в момента диалога е в състояние на "влачене" на избрания елемент.
		/// </summary>
		private bool isDragging;
		public bool IsDragging {
			get { return isDragging; }
			set { isDragging = value; }
		}
		
		/// <summary>
		/// Последна позиция на мишката при "влачене".
		/// Използва се за определяне на вектора на транслация.
		/// </summary>
		private PointF lastLocation;
		public PointF LastLocation {
			get { return lastLocation; }
			set { lastLocation = value; }
		}
		
		#endregion
		
		/// <summary>
		/// Добавя примитив - правоъгълник на произволно място върху клиентската област.
		/// </summary>
		public void AddRandomRectangle()
		{
			Random rnd = new Random();
			int x = rnd.Next(100,1000);
			int y = rnd.Next(100,600);
			
			RectangleShape rect = new RectangleShape(new Rectangle(x,y,100,200));
			ShapeList.Add(rect);
		}

        public void AddRandomLineRectangle()
        {
            Random rnd = new Random();
            int x = rnd.Next(100, 1000);
            int y = rnd.Next(100, 600);

            LineRectangleShape rect = new LineRectangleShape(new Rectangle(x, y, 400, 200));
            ShapeList.Add(rect);
        }

        /// <summary>
        /// Добавя примитив - правоъгълник на място, където сме кликнали върху клиентската област.
        /// </summary>
        public void AddRectangleOnClick(PointF point)
        {
			float x = point.X;
            float y = point.Y;

            RectangleShape rect = new RectangleShape(new RectangleF(x, y, 100, 200));
            ShapeList.Add(rect);
        }

        /// <summary>
        /// Добавя примитив - Елипса на произволно място върху клиентската област.
        /// </summary>
        public void AddRandomEllipse()
        {
            Random rnd = new Random();
            int x = rnd.Next(100, 1000);
            int y = rnd.Next(100, 600);

            EllipseShape ellipse = new EllipseShape(new Rectangle(x, y, 100, 200));

            ShapeList.Add(ellipse);
        }

        /// <summary>
        /// Добавя примитив - Елипса на произволно място върху клиентската област.
        /// </summary>
        public void AddRandomStar()
        {
            Random rnd = new Random();
            int x = rnd.Next(100, 1000);
            int y = rnd.Next(100, 600);

            StarShape star = new StarShape(new Rectangle(x, y, 100, 200));

            ShapeList.Add(star);
        }

        public void AddRandomPolygon()
        {
            Random rnd = new Random();
            int x = rnd.Next(100, 1000);
            int y = rnd.Next(100, 600);

            var polygon = new PolygonShape(new Rectangle(x, y, 100, 200));

            ShapeList.Add(polygon);
        }

        public void AddRandomLine()
        {
            Random rnd = new Random();
            int x = rnd.Next(100, 1000);
            int y = rnd.Next(100, 600);

            var line = new LineShape(new Rectangle(x, y, 100, 200));

            ShapeList.Add(line);
        }

        public void AddRandomDot()
        {
            Random rnd = new Random();
            int x = rnd.Next(100, 1000);
            int y = rnd.Next(100, 600);

            var dot = new DotShape(new RectangleF(x, y, 5, 5));

            ShapeList.Add(dot);
        }

        /// <summary>
        /// Завърта селектирания елемент според ъгъла, който сме задали.
        /// </summary>
        /// <param name="angle">Ъгъл</param>
        public void RotateSelected(int angle)
        {
            if (Selection.Count > 0)
            {
                foreach (var shape in Selection)
                {
                    RotateShape(shape, angle);
                }
            }
        }

        private void RotateShape(Shape shape, int angle)
        {
            if (shape is GroupShape group)
            {
                foreach (var subShape in group.SubShapes)
                {
                    RotateShape(subShape, angle);
                }
            }
            else
            {
                shape.TransformationMatrix.RotateAt(angle, new PointF(shape.Location.X + shape.Width / 2, shape.Location.Y + shape.Height / 2));
            }
        }


        /// <summary>
        /// Уголемява размера на селектираните фигури.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ScaleSelected(float x, float y)
        {
            if (Selection.Count > 0)
            {
                foreach (var shape in Selection)
                {
                    ScaleShape(shape, x, y);
                }
            }
        }

        private void ScaleShape(Shape shape, float x, float y)
        {
            if (shape is GroupShape group)
            {
                foreach (var subShape in group.SubShapes)
                {
                    ScaleShape(subShape, x, y);
                }
            }
            else
            {
                shape.TransformationMatrix.Scale(x, y, MatrixOrder.Append);
                shape.TransformationMatrix.Translate(-shape.Location.X * (x - 1), -shape.Location.Y * (y - 1), MatrixOrder.Append);
            }
        }

        /// <summary>
        /// Проверява дали дадена точка е в елемента.
        /// Обхожда в ред обратен на визуализацията с цел намиране на
        /// "най-горния" елемент т.е. този който виждаме под мишката.
        /// </summary>
        /// <param name="point">Указана точка</param>
        /// <returns>Елемента на изображението, на който принадлежи дадената точка.</returns>
        public Shape ContainsPoint(PointF point)
		{
			for(int i = ShapeList.Count - 1; i >= 0; i--){
				if (ShapeList[i].Contains(point))
				{
					return ShapeList[i];
				}
			}

			return null;
		}

        /// <summary>
        /// Транслация на избраният елемент на вектор определен от <paramref name="p>p</paramref>
        /// </summary>
        /// <param name="p">Вектор на транслация.</param>
        public void TranslateTo(PointF p)
        {
            if (Selection.Count > 0)
            {
                foreach (var shape in Selection)
                {
                    float deltaX = p.X - lastLocation.X;
                    float deltaY = p.Y - lastLocation.Y;

                    if(shape is GroupShape grShape)
                    {
                        foreach (Shape sh in grShape.SubShapes)
                        {
                            Translate(sh, deltaX, deltaY);
                        }
                    }
                    else
                    {
                        Matrix inverseMatrix = shape.TransformationMatrix.Clone();
                        inverseMatrix.Invert();
                        PointF[] translationVector = { new PointF(deltaX, deltaY) };
                        inverseMatrix.TransformVectors(translationVector);

                        shape.Location = new PointF(shape.Location.X + translationVector[0].X, shape.Location.Y + translationVector[0].Y);
                    }
                }
                lastLocation = p;
            }
        }

        private void Translate(Shape shape, float deltaX, float deltaY)
        {
            if (shape is GroupShape grShape)
            {
                foreach (Shape sh in grShape.SubShapes)
                {
                    Translate(sh, deltaX, deltaY);
                }
            }
            else
            {
                Matrix inverseMatrix = shape.TransformationMatrix.Clone();
                inverseMatrix.Invert();
                PointF[] translationVector = { new PointF(deltaX, deltaY) };
                inverseMatrix.TransformVectors(translationVector);

                shape.Location = new PointF(shape.Location.X + translationVector[0].X, shape.Location.Y + translationVector[0].Y);
            }
        }

        /// <summary>
        /// Групира селектираните елементи.
        /// </summary>
        public void GroupSelected()
        {
            if (Selection.Count < 2) return;

            float minX = float.PositiveInfinity;
            float minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;

            foreach (var shape in Selection)
            {
                if (shape.Location.X < minX)
                {
                    minX = shape.Location.X;
                }
                if (shape.Location.Y < minY)
                {
                    minY = shape.Location.Y;
                }
                if (shape.Location.X + shape.Width > maxX)
                {
                    maxX = shape.Location.X + shape.Width;
                }
                if (shape.Location.Y + shape.Height > maxY)
                {
                    maxY = shape.Location.Y + shape.Height;
                }
            }

            var group = new GroupShape(new RectangleF(minX, minY, maxX - minX, maxY - minY));

            foreach (var shape in Selection)
            {
                ColorGroup(shape);
                ShapeList.Remove(shape);
                group.SubShapes.Add(shape);
            }

            Selection.Clear();

            ShapeList.Add(group);
        }

        private void ColorGroup(Shape shape)
        {
            if (shape is GroupShape grShape)
            {
                foreach (Shape sh in grShape.SubShapes)
                {
                    ColorGroup(sh);
                }
                
            }
            else
            {
                shape.FillColor = shape.LastColor;
            }
        }

        /// <summary>
        /// Разделя групираните елементи от селекцията.
        /// </summary>
        public void UngroupSelected()
        {
            foreach (var shape in Selection)
            {
                ColorizeAndUngroup(shape);
            }
            Selection.Clear();
        }

        private void ColorizeAndUngroup(Shape shape)
        {
            if (shape is GroupShape group)
            {
                foreach (Shape sh in group.SubShapes)
                {
                    ColorizeAndUngroup(sh);
                }

                ShapeList.AddRange(group.SubShapes);
                ShapeList.Remove(group);
            }
            else
            {
                shape.FillColor = shape.LastColor;
                ShapeList.Add(shape);
            }
        }
    }
}
