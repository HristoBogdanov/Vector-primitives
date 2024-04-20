using Draw.src.Model;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace Draw
{
	/// <summary>
	/// Базовия клас на примитивите, който съдържа общите характеристики на примитивите.
	/// </summary>
	[Serializable]
	public abstract class Shape : ISerializable
	{

		#region Constructors
		
		public Shape()
		{
		}
		
		public Shape(RectangleF rect)
		{
			rectangle = rect;
		}

        protected Shape(SerializationInfo info, StreamingContext context)
        {
            rectangle = (RectangleF)info.GetValue("Rectangle", typeof(RectangleF));
            fillColor = (Color)info.GetValue("FillColor", typeof(Color));
            strokeColor = (Color)info.GetValue("StrokeColor", typeof(Color));
            opacity = info.GetInt32("Opacity");
            strokeWidth = info.GetInt32("StrokeWidth");

            var matrixData = (SerializableMatrix)info.GetValue("TransformationMatrix", typeof(SerializableMatrix));
            transformationMatrix = matrixData.ToMatrix();
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize basic properties
            info.AddValue("Rectangle", rectangle);
            info.AddValue("FillColor", fillColor);
            info.AddValue("StrokeColor", strokeColor);
            info.AddValue("Opacity", opacity);
            info.AddValue("StrokeWidth", strokeWidth);

            // Serialize SerializableMatrix object manually
            var matrixData = new SerializableMatrix(transformationMatrix);
            info.AddValue("TransformationMatrix", matrixData);
        }

        public Shape(Shape shape)
		{
			this.Height = shape.Height;
			this.Width = shape.Width;
			this.Location = shape.Location;
			this.rectangle = shape.rectangle;
			this.StrokeColor = shape.StrokeColor;
			this.FillColor =  shape.FillColor;
		}
		#endregion
		
		#region Properties
		
		/// <summary>
		/// Обхващащ правоъгълник на елемента.
		/// </summary>
		private RectangleF rectangle;		
		public virtual RectangleF Rectangle {
			get { return rectangle; }
			set { rectangle = value; }
		}
		
		/// <summary>
		/// Широчина на елемента.
		/// </summary>
		public virtual float Width {
			get { return Rectangle.Width; }
			set { rectangle.Width = value; }
		}
		
		/// <summary>
		/// Височина на елемента.
		/// </summary>
		public virtual float Height {
			get { return Rectangle.Height; }
			set { rectangle.Height = value; }
		}
		
		/// <summary>
		/// Горен ляв ъгъл на елемента.
		/// </summary>
		public virtual PointF Location {
			get { return Rectangle.Location; }
			set { rectangle.Location = value; }
		}
		
		/// <summary>
		/// Цвят на елемента.
		/// </summary>
		private Color fillColor = Color.White;		
		public virtual Color FillColor {
			get { return fillColor; }
			set { fillColor = value; }
		}

		public virtual Color LastColor { get; set; }

		/// <summary>
		/// Цвят на външен контур на елемента.
		/// </summary>
        private Color strokeColor = Color.Black;
        public virtual Color StrokeColor
        {
            get { return strokeColor; }
            set { strokeColor = value; }
        }
		/// <summary>
		/// Дебелина на външен контур на елемента.
		/// </summary>
        private int strokeWidth = 1;
        public virtual int StrokeWidth
        {
            get { return strokeWidth; }
            set { strokeWidth = value; }
        }

		/// <summary>
		/// Прозрачност на елемента.
		/// </summary>
        private int opacity = 255;
        public virtual int Opacity
        {
            get { return opacity; }
            set { opacity = value; }
        }


		private Matrix transformationMatrix = new Matrix();
        public virtual Matrix TransformationMatrix
        {
            get { return transformationMatrix; }
            set { transformationMatrix = value; }
        }


        #endregion


        /// <summary>
        /// Проверка дали точка point принадлежи на елемента.
        /// </summary>
        /// <param name="point">Точка</param>
        /// <returns>Връща true, ако точката принадлежи на елемента и
        /// false, ако не пренадлежи</returns>
        public virtual bool Contains(PointF point)
		{
            Matrix inverseMatrix = TransformationMatrix.Clone();
            inverseMatrix.Invert();
            PointF[] points = { point };
            inverseMatrix.TransformPoints(points);
            PointF transformedPoint = points[0];

            return Rectangle.Contains(transformedPoint.X, transformedPoint.Y);
		}
		
		/// <summary>
		/// Визуализира елемента.
		/// </summary>
		/// <param name="grfx">Къде да бъде визуализиран елемента.</param>
		public virtual void DrawSelf(Graphics grfx)
		{
			// shape.Rectangle.Inflate(shape.BorderWidth, shape.BorderWidth);
		}
    }
}
