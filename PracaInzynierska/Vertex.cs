using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracaInzynierska
{
    public struct Vertex : IEquatable<Vertex>
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vertex(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool Equals(Vertex other)
        {
            if(other.X != X || other.Y != Y || other.Z != Z)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
