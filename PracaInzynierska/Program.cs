using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracaInzynierska
{
    class Program
    {
        static void Main(string[] args)
        {
            using (MainWindow window = new MainWindow(800, 600, "Generacja Terenu"))
            {
                window.Run(60.0f);
            }
        }
    }
}
