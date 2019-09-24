using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace PracaInzynierska
{
    class Noise
    {
        private uint Resolution;

        private float Amplitude;

        private float[] points;



        Random rand = new Random();
        public Noise(uint resolution, float amplitude)
        {
            Resolution = resolution;

            Amplitude = amplitude;

            points = new float[Resolution * Resolution];

            generateNoise();
            
        }

        private void generateNoise()
        {
            for (int i = 0; i < Resolution; i++)
            {
                for (int j = 0; j < Resolution; j++)
                {
                    points[i * Resolution + j] = (float) (rand.NextDouble() * 2 - 1) * Amplitude;
                }
            }
        }

        public float[] GetNoise()
        {
            return points;
        }
    }
}
