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
    [Serializable]
    public class Map
    {
        public MapCell[,] mapCells { get; private set; }
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        [NonSerialized]
        public Tileset _tileset;

        public Tileset Tileset
        {
            get { return _tileset; }
            private set { _tileset = value; }
        }

        [NonSerialized]
        public Point _castleTile;

        public Point CastleTile
        {
            get { return _castleTile; }
            private set { _castleTile = value; }
        }

        public List<Point> SpawnPoints { get; private set; }
        public int Music { get; private set; }

        public Map()
        {
            MapWidth = 19;
            MapHeight = 15;
            Tileset = TileEngine.Tilesets[0];
            SpawnPoints = new List<Point>();

            mapCells = new MapCell[MapHeight,MapWidth];

            Music = 2;

            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    mapCells[y, x] = new MapCell(0);
                }
            }
        }

        //Indexer for ease of accessing tiles
        public MapCell this[int y, int x]
        {
            get { return mapCells[y, x]; }
            set { mapCells[y,x] = value; }
        }


        public void Initialize()
        {
            Console.WriteLine("Map initialized.");
            // Select a random song to play for the game.
            Music = new Random().Next(1, 4);
            AudioManager.PlaySong(Music);

            PopulateSpawnPoints();
        }

        public void PopulateSpawnPoints()
        {
            SpawnPoints = new List<Point>();

            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    if (mapCells[y, x].tiles.Count > 1 && mapCells[y, x].tiles[1].TileID == 5)
                    {
                        SpawnPoints.Add(new Point(x, y));
                    }
                }
            }
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

                    if (GameScene.showGrid)
                        spriteBatch.Draw(Tileset.Texture, destinationRect, Tileset.GetSourceRectangle(54), Color.White);
                }
            }
        }


        //Run after loading
        public void LoadFromSerialized()
        {
            Tileset = TileEngine.Tilesets[0];
            for (int y = 0; y < MapHeight; y++)
            {
                for(int x = 0; x < MapWidth; x++)
                {
                    if (mapCells[y, x].tiles.Count > 1 && mapCells[y, x].tiles[1].TileID == 4)
                    {
                        CastleTile = new Point(x, y);
                        mapCells[y, x].IsBuildable = false;
                    }

                    if (mapCells[y, x].tiles.Count > 1 && mapCells[y, x].tiles[1].TileID == 5)
                    {
                        SpawnPoints.Add(new Point(x, y));
                    }
                }
            }

            Console.WriteLine(String.Format("Castle is at {0}, {1}.", CastleTile.X, CastleTile.Y));
            foreach (Tile t in mapCells[CastleTile.Y, CastleTile.X].tiles)
            {
                Console.Write(t.TileID);
            }

            foreach (Point p in SpawnPoints)
            {
                Console.WriteLine(String.Format("Spawn point at: {0}, {1}", p.X, p.Y));
            }

            AudioManager.PlaySong(Music);
        }


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
        }

        public void SetMovementCost(Point point, float cost)
        {
            GameScene.pathfinder.SetMovementCost(point, cost);
        }
        

        public void SetTileGoldability(Point point, bool b)
        {
            mapCells[point.Y, point.X].ContainsGold = b;
        }

        public void SetTileHasTower(Point point, bool b)
        {
            this[point.Y, point.X].ContainsTower = b;
        }
    }
}
