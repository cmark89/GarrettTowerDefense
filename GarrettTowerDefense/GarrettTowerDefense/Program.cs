using System;

namespace GarrettTowerDefense
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GarrettTowerDefense game = new GarrettTowerDefense())
            {
                game.Run();
            }
        }
    }
#endif
}

