using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace GarrettTowerDefense
{
    public class Map
    {
        public MapCell[,] mapCells { get; private set; }
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }
        public Tileset Tileset { get; private set;}

        public Point CastleTile { get; private set; }
        public List<Point> SpawnPoints { get; private set; }
        public int Music { get; private set; }


        public void InitializeTestMap()
        {
            MapWidth = 19;
            MapHeight = 15;
            Tileset = TileEngine.Tilesets[0];
            SpawnPoints = new List<Point>();
            Random random = new Random();
            Music = 3;

            //Create a map using the first 3 tiles randomly
            mapCells = new MapCell[MapHeight, MapWidth];
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    mapCells[y, x] = new MapCell(random.Next(0,3));
                }
            }

            //Now, loop back over the map.  If the tile is grass, there's a 1 in 3 chance a tree will be placed on top.
            //If the tile is dirt, there's a 1 in 5 chance a gold mine will be placed on top.
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    if (mapCells[y, x].tiles[0].TileID == 0 && random.NextDouble() < .33f)
                    {
                        mapCells[y, x].AddTile(3);
                    }
                }

                for (int x = 0; x < MapWidth; x++)
                {
                    if (mapCells[y, x].tiles[0].TileID == 1 && random.NextDouble() < .1f)
                    {
                        mapCells[y, x].AddTile(14);
                    }
                }

                Random rand = new Random();
                Point randomPoint = new Point(rand.Next(0, MapWidth), rand.Next(0, MapHeight));
                MapCell randCell = mapCells[randomPoint.Y, randomPoint.X] = new MapCell(0);
                randCell.AddTile(4);
                CastleTile = randomPoint;
            }

            Initialize();
        }


        public void InitializeTestMap2()
        {
            Console.WriteLine("INITIALIZE TEST MAP.");
            MapWidth = 19;
            MapHeight = 15;
            Tileset = TileEngine.Tilesets[0];
            SpawnPoints = new List<Point>();
            Random random = new Random();
            Music = 2;


            mapCells = new MapCell[MapHeight, MapWidth];
            for(int y = 0; y < MapHeight; y++)
            {
                for(int x = 0; x < MapWidth; x++)
                {
                    mapCells[y,x] = new MapCell(0);
                }
            }

            mapCells[0, 13] = new MapCell(2);
            mapCells[1, 13] = new MapCell(2);
            mapCells[2, 13] = new MapCell(2);
            mapCells[3, 13] = new MapCell(2);
            mapCells[4, 13] = new MapCell(2);

            mapCells[4, 0] = new MapCell(2);
            mapCells[4, 1] = new MapCell(2);
            mapCells[4, 2] = new MapCell(2);
            mapCells[4, 3] = new MapCell(2);
            mapCells[4, 4] = new MapCell(2);
            mapCells[4, 5] = new MapCell(2);
            mapCells[4, 6] = new MapCell(2);

            mapCells[0, 0].AddTile(5);
            mapCells[14, 2].AddTile(4);

            CastleTile = new Point(2, 14);
            SpawnPoints.Add(new Point(0, 0));

            Initialize();
        }


        public void Initialize()
        {
            AudioManager.PlaySong(Music);
            GameScene.pathfinder = new Pathfinding.Pathfinder(this);

            //TEST
            
        }


        public void Update(GameTime gameTime)
        {
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    Rectangle destinationRect = new Rectangle(x * TileEngine.TileWidth, y * TileEngine.TileHeight, TileEngine.TileWidth, TileEngine.TileHeight);
                    for(int i = 0; i < mapCells[y,x].tiles.Count; i++)
                    {
                        spriteBatch.Draw(Tileset.Texture, destinationRect, Tileset.GetSourceRectangle(mapCells[y,x].tiles[i].TileID), Color.White);
                    }
                }
            }
        }

        #region Serialization and Loading
        //This will deserialize a map from a file
        public static Map LoadMap(string mapName)
        {
            return null;
        }

        public static Map LoadMap(int mapIndex)
        {
            return null;
        }

        //This will serialize a map to a file
        public static void SaveMap()
        {
        }
        #endregion


        public bool TileIsBuildable(Point point)
        {
            return mapCells[point.Y, point.X].IsBuildable;
        }

        public bool TileContainsGold(Point point)
        {
            return mapCells[point.Y, point.X].ContainsGold;
        }

        public void SetTileBuildability(Point point, bool b)
        {
            mapCells[point.Y, point.X].IsBuildable = b;
        }

        public void SetTileWalkability(Point point, bool b)
        {
            mapCells[point.Y, point.X].IsWalkable = b;
            GameScene.pathfinder.SetMovementCost(point, 1000000);
        }

        public void SetTileGoldability(Point point, bool b)
        {
            mapCells[point.Y, point.X].ContainsGold = b;
        }
    }
}
