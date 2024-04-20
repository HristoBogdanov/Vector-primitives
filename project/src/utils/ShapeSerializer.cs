using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Draw.src.utils
{
    public static class ShapeSerializer
    {
        public static void SerializeShapes(List<Shape> shapes, string filePath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                formatter.Serialize(stream, shapes);
            }
        }

        public static List<Shape> DeserializeShapes(string filePath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                return (List<Shape>)formatter.Deserialize(stream);
            }
        }
    }
}
