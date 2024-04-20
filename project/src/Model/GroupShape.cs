using Draw.src.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace Draw
{
	[Serializable]
	public class GroupShape : Shape
	{
		public List<Shape> SubShapes = new List<Shape>();
		#region Constructor
		
		public GroupShape(RectangleF rect) : base(rect)
		{
		}
		
		public GroupShape(RectangleShape rectangle) : base(rectangle)
		{
		}

        public GroupShape(EllipseShape ellipse) : base(ellipse)
        {
        }

        protected GroupShape(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            SubShapes = (List<Shape>)info.GetValue("SubShapes", typeof(List<Shape>));
        }

        #endregion

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("SubShapes", SubShapes);
        }

        public override bool Contains(PointF point)
        {
        foreach (Shape shape in SubShapes)
        {
            if (shape is GroupShape groupShape)
            {
                if (groupShape.Contains(point))
                {
                    return true;
                }
            }
            else if (shape.Contains(point))
            {
                return true;
            }
        }

        return false;
        }

        /// <summary>
        /// Частта, визуализираща конкретния примитив.
        /// </summary>
        public override void DrawSelf(Graphics grfx)
        {
            var state = grfx.Save();
            base.DrawSelf(grfx);

            foreach (var shape in SubShapes)
            {
                if (shape is GroupShape group)
                {
                    foreach (var subShape in group.SubShapes)
                    {
                        subShape.DrawSelf(grfx);
                    }
                }
                else
                {
                    shape.DrawSelf(grfx);
                }
            }
            grfx.Restore(state);
        }

        public override Matrix TransformationMatrix 
		{ 
			get => base.TransformationMatrix; 
			set
			{
				foreach(var shape in SubShapes)
				{
					shape.TransformationMatrix = value;
				}
                base.TransformationMatrix = value;
			}
		}

        public override PointF Location 
		{ 
			get => base.Location;
			set
			{
				foreach (var shape in SubShapes)
				{
					shape.Location = new PointF(shape.Location.X - Location.X + value.X, shape.Location.Y - Location.Y + value.Y);
				}
				base.Location = value;
			}
		}
    }
}
