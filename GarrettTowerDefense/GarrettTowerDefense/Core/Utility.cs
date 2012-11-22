using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    class Vector2Helper
    {
        public static float FindAngle(Vector2 to, Vector2 from)
        {
            float y = to.Y - from.Y;
            float x = to.X - from.X;

            return (float)Math.Atan2(y, x);
        }
    }
}
