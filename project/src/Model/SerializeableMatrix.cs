using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Draw.src.Model
{
    [Serializable]
    public class SerializableMatrix
    {
        public float[] Elements { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }

        public SerializableMatrix()
        {
            Elements = new float[6];
        }

        public SerializableMatrix(Matrix matrix)
        {
            Elements = matrix.Elements;
            OffsetX = matrix.OffsetX;
            OffsetY = matrix.OffsetY;
        }

        public Matrix ToMatrix()
        {
            return new Matrix(Elements[0], Elements[1], Elements[2], Elements[3], Elements[4], Elements[5]);
        }
    }
}
