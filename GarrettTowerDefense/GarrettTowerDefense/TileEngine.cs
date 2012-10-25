using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GarrettTowerDefense
{
    static class TileEngine
    {
        public static int TileWidth { get; private set; }
        public static int TileHeight { get; private set; }

        public static List<Tileset> Tilesets = new List<Tileset>();

        public static void InitializeEngine()
        {
            TileWidth = 32;
            TileHeight = 32;
        }

        public static Point ScreenSpaceToMapSpace(Vector2 screenPoint)
        {
            return new Point((int)screenPoint.X / TileWidth, (int)screenPoint.Y / TileHeight);
        }

        public static Vector2 ScreenSpaceToMapVector(Vector2 screenPoint)
        {
            return new Vector2(((int)(screenPoint.X / TileWidth)) * TileWidth, ((int)(screenPoint.Y / TileHeight)) * TileHeight);
        }
    }



    public class Tileset
    {
        public Texture2D Texture { get; private set; }
        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public int TilesetID { get; private set; }

        public Tileset(Texture2D texture)
        {
            //Set texture to the passed texture and set the rows and columns appropriately
            Texture = texture;
            Rows = Texture.Height / TileEngine.TileHeight;
            Columns = Texture.Width / TileEngine.TileWidth;

            //Add self to the master list of tilesets and set the tilesetID appropriately
            TileEngine.Tilesets.Add(this);
            TilesetID = TileEngine.Tilesets.Count;
        }


        //Returns the source rectangle of the given texture index.
        public Rectangle GetSourceRectangle(int index)
        {
            int startX = (index % Columns) * TileEngine.TileWidth;
            int startY = (index / Columns) * TileEngine.TileHeight;

            return new Rectangle(startX, startY, TileEngine.TileWidth, TileEngine.TileHeight);
        }
    }



    public class Tile
    {
        public int TileID { get; private set; }

        public Tile(int index)
        {
            TileID = index;
        }

    }



    //The MapCell class represents the physical cell on the map.
    public class MapCell
    {
        public Map parentMap { get; private set; }
        public List<Tile> tiles { get; private set; }
        public bool IsWalkable = true;
        public bool IsBuildable = true;
        public bool ContainsGold = false;

        //Creates a blank MapCell
        public MapCell()
        {
            tiles = new List<Tile>();
        }

        //Creates a MapCell with a default tile of tileIndex
        public MapCell(int tileIndex)
        {
            tiles = new List<Tile>();
            AddTile(tileIndex);
        }

        //Adds a tile to this MapCell which will be drawn on top of existing tiles.
        public void  AddTile(int index)
        {
            tiles.Add(new Tile(index));
            
            //Placeholder.  Find a more elegant way to do this.
            if(index == 2 || index == 3)
            {
                IsWalkable = false;
                IsBuildable = false;
            }

            if (index == 14)
            {
                IsWalkable = false;
                IsBuildable = false;
                ContainsGold = true;
            }

            if (index == 5)
            {
                IsBuildable = false;
            }
        }
    }
}
