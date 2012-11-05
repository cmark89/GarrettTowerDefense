using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GarrettTowerDefense
{
    public static class Serializer
    {
        public static void Serialize(string filePath, object objectToSerialize)
        {
            try
            {
                using(FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    BinaryFormatter binFormatter = new BinaryFormatter();
                    binFormatter.Serialize(stream, objectToSerialize);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static object Deserialize(string filePath)
        {
            try
            {
                using(FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter binFormatter = new BinaryFormatter();
                    return binFormatter.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return null;
        }
    }
}
