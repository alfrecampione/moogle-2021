using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoogleEngine
{
    public struct Vector
    {
        public float[] elements;
        int count;
        public Vector(float[] elements)
        {
            this.elements = elements;
            this.count = elements.Length;
        }
        public float this[int i]
        {
            get
            {
                return elements[i];
            }
            set
            {
                elements[i] = value;
            }
        }
        public static float operator *(Vector a, Vector b)
        {
            if (a.count != b.count)
                throw new ArgumentException("Vectors have not the same size");
            float result = 0;
            for (int i = 0; i < a.count; i++)
            {
                result += a.elements[i] * b.elements[i];
            }
            return result;
        }
    }
}
