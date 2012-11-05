using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GarrettTowerDefense;

namespace Pathfinding
{
    //Represents one node in the searchable area
    public class SearchNode
    {
        //Location on the map
        public Point Position;

        //Whether the node is walkable or not
        public bool Walkable;

        //Stores references to the neighboring tiles (only Up, Left, Down, Right).
        public SearchNode[] Neighbors;

        //The search node that added this node to the open list.  Used to trace the path back from the ending point.
        public SearchNode Parent;

        public bool InOpenList;

        public bool InClosedList;

        //Estimate of the distance to the end point if the path goes through this point
        public float DistanceToGoal;

        //Distance traveled to reach this node.
        public float DistanceTraveled;

        //This is the cost of moving into this tile for calculating shortest distance.
        public float MovementCost = 1;

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle(Position.X * TileEngine.TileHeight, Position.Y * TileEngine.TileHeight, TileEngine.TileWidth, TileEngine.TileHeight), new Color(1f,1f,0f,.5f));
        }
    }



    public class Pathfinder
    {
        //Stores the walkable search nodes that will represent the map
        private SearchNode[,] searchNodes;

        private int levelWidth;
        private int levelHeight;

        //Holds nodes available to search
        private List<SearchNode> openList = new List<SearchNode>();

        //Holds nodes that have already been searched
        private List<SearchNode> closedList = new List<SearchNode>();
        

        private float Heuristic(Point point1, Point point2)
        {
            return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
        }
         

        //Construct new pathfinding information using the given map
        public Pathfinder(Map map)
        {
            levelWidth = map.MapWidth;
            levelHeight = map.MapHeight;

            InitializeSearchNodes(map);
        }


        public void InitializeSearchNodes(Map map)
        {
            searchNodes = new SearchNode[map.MapHeight, map.MapWidth];

            //For each tile in our map, create a SearchNode for it
            for (int y = 0; y < levelHeight; y++)
            {
                for (int x = 0; x < levelWidth; x++)
                {
                    SearchNode node = new SearchNode();
                    node.Position = new Point(x, y);
                    node.Walkable = map.mapCells[y,x].IsWalkable;

                    if (node.Walkable)
                    {
                        //Create the list of neighbors
                        node.Neighbors = new SearchNode[4];
                        searchNodes[y, x] = node;
                    }
                }
            }

            //Now that we have created search nodes for the entire level, it is time to populate each node with its neighbors
            for (int y = 0; y < levelHeight; y++)
            {
                for (int x = 0; x < levelWidth; x++)
                {
                    SearchNode thisNode = searchNodes[y, x];

                    //Ignore any nodes that are unwalkable or don't exist.
                    if (thisNode == null || !thisNode.Walkable)
                        continue;

                    //A list of all possible neighbors this node can have
                    Point[] neighbors = new Point[]
                    {
                        new Point (x, y - 1),       //The node to the left
                        new Point (x, y + 1),       //The node to the right
                        new Point (x - 1, y),       //The node below
                        new Point (x + 1, y)        //The node above
                    };

                    //Now, loop through the neighbors
                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        Point position = neighbors[i];

                        //First verify that the neighbor is actually part of the level
                        if (position.X < 0 || position.X > levelWidth - 1 || position.Y < 0 || position.Y > levelHeight - 1)
                            continue;

                        //The neighbor is part of the level; grab a reference to it from the list of nodes
                        SearchNode neighbor = searchNodes[position.Y, position.X];

                        //We will only keep a reference to search nodes that can be walked on.
                        if (neighbor == null || !neighbor.Walkable)
                            continue;

                        //Store the reference to the neighbor
                        thisNode.Neighbors[i] = neighbor;
                    }
                }
            }
        }


        //Reset the pathfinder from the last usage
        private void ResetSearchNodes()
        {
            //NOTE: Replace the openList List<> structure with a binary heap to improve efficiency!
            openList.Clear();
            closedList.Clear();
            for (int y = 0; y < levelHeight; y++)
            {
                for (int x = 0; x < levelWidth; x++)
                {
                    SearchNode node = searchNodes[y, x];

                    if (node == null)
                        continue;

                    node.InOpenList = false;
                    node.InClosedList = false;

                    node.DistanceTraveled = float.MaxValue;
                    node.DistanceToGoal = float.MaxValue;

                }
            }
        }


        //Returns the node in the open list with the smallest distance to the goal
        private SearchNode FindBestNode()
        {
            SearchNode currentNode = openList[0];

            //Set the current distance to an arbitrarily massive value
            float smallestDistanceToGoal = float.MaxValue;

            //Find the closest node to the goal
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].DistanceToGoal < smallestDistanceToGoal)
                {
                    currentNode = openList[i];
                    smallestDistanceToGoal = currentNode.DistanceToGoal;
                }
            }

            return currentNode;
        }


        public List<Vector2> FindFinalPath(SearchNode startNode, SearchNode endNode)
        {
            closedList.Add(endNode);
            SearchNode parentNode = endNode.Parent;

            //Trace the path back through the parent field, getting the best path and adding each node to the closed list.
            while (parentNode != startNode)
            {
                closedList.Add(parentNode);
                parentNode = parentNode.Parent;
            }

            //Now that the path has been traced back, reverse the path and convert it to vectors in world space
            List<Vector2> finalPath = new List<Vector2>();

            for (int i = closedList.Count - 1; i >= 0; i--)
            {
                finalPath.Add(new Vector2((closedList[i].Position.X * TileEngine.TileWidth), (closedList[i].Position.Y * TileEngine.TileHeight)));
            }

            return finalPath;
        }


        //The meat of the system:
        public List<Vector2> FindPath(Point startPoint, Point endPoint)
        {
            Console.WriteLine("\nInitiate FindPath");
            Console.WriteLine("\nStart Point: " + startPoint.X + ", " + startPoint.Y);
            Console.WriteLine("\nEnd Point: " + endPoint.X + ", " + endPoint.Y);

            //Only attempt to use the pathfinding system if the start point and the end point are different
            if (startPoint == endPoint)
            {
                Console.WriteLine("\nStart and end point are identical.  Terminating.");
                return new List<Vector2>();
            }

            //Cache the start and end nodes for convenience
            Console.WriteLine("\nPF Width: " + levelWidth + "\n PF Height:" + levelHeight);
            SearchNode startNode = searchNodes[startPoint.Y, startPoint.X];
            SearchNode endNode = searchNodes[endPoint.Y, endPoint.X];


            //====================
            //STEP 1: Clear the open and closed list and reset each tile's distance to goal and distance traveled
            //====================
            ResetSearchNodes();

            //====================
            //STEP 2: Reset the start node's G to 0, and its F to the estimated distance to the end.  Then add it to the open list.
            //====================
            startNode.DistanceTraveled = 0;
            startNode.DistanceToGoal = Heuristic(startNode.Position, endNode.Position);

            openList.Add(startNode);

            //====================
            //STEP 3: Loop through the following as long as there are nodes in the open list
            //====================
            while (openList.Count > 0)
            {
                //====================
                //A) Loop through the open list and find the node with the smallest distance to the goal
                //====================
                SearchNode currentNode = FindBestNode();
                
                    //Corey note here: The reason this works is because it takes into account the G value, which is
                    //the estimate of how long the path will be if the final path moves through this node.
                    //Every time the end node is reached, it is not immediately returned.  Instead, it is added to the
                    //open list and its parent is updated.  The end node, if added like this, will /NEVER/ be the
                    //current node until its DistanceToGoal field is the lowest in the open list.  Because of the way it
                    //is updated (and cheaper routes are sure to be explored before the end node becomes active)
                    //you can be sure that the path returned is indeed the "cheapest" and not just the closest spatially.

                    //Corey note 2 here: It doesn't matter, because as soon as the lowest cost node finds the end, there is virtually no circumstance
                    //that would prevent that path from being the fastest.  Save on calculations by returning the path immediately.

                //====================
                //B) If the open list is somehow empty, or no node can be found, the algorithm will terminate
                //====================
                if (currentNode == null)
                {
                    Console.WriteLine("\nNull node found.");
                    break;
                }

                //====================
                //C) If the active node is the goal node, we will terminate and return the path
                //====================
                if (currentNode == endNode)
                {
                    Console.WriteLine("\nGoal found.  Returning final path.");
                    //Trace our final path back to the start
                    return FindFinalPath(startNode, endNode);
                }

                //====================
                //D) Otherwise, for each of the selected node's neighbors:
                //====================
                for (int i = 0; i < currentNode.Neighbors.Length; i++)
                {
                    SearchNode thisNeighbor = currentNode.Neighbors[i];

                    //====================
                    //i) Verify that the tile exists and is indeed able to be walked on
                    //====================
                    if (thisNeighbor == null || !thisNeighbor.Walkable)
                        continue;

                    //====================
                    //ii) Calculate the new G value for this node
                    //====================
                    //Calculate the distance traveled if moving to this node
                    float distanceTraveled = currentNode.DistanceTraveled + thisNeighbor.MovementCost;
                    //Estimate the distance from this node to the end point
                    float heuristic = Heuristic(thisNeighbor.Position, endNode.Position);

                    //====================
                    //iii) If the neighboring node is not in either the open or the closed lists:
                    //====================
                    if (!thisNeighbor.InClosedList && !thisNeighbor.InOpenList)
                    {
                        //1) Set the neighboring node's G value to the number we just calculated
                        thisNeighbor.DistanceTraveled = distanceTraveled;
                        //2) Set the F value to the distanceTraveled + the heuristic
                        thisNeighbor.DistanceToGoal = distanceTraveled + heuristic;
                        //3) Set the neighboring node's parent to the currently considered node
                        thisNeighbor.Parent = currentNode;
                        //4) Add the neighbor to the open list
                        thisNeighbor.InOpenList = true;
                        openList.Add(thisNeighbor);
                    }
                    //====================
                    //iv) Otherwise, if the node is in either the open or the closed list
                    //====================
                    else if (thisNeighbor.InClosedList || thisNeighbor.InOpenList)
                    {
                        //1) If the newly calculated G value is less than the neighbor node's current G value,
                        //set the G value to the newly calculated G and reset the parent to this node (because the path 
                        //is shorter, but do not add it to the open list.
                        if (thisNeighbor.DistanceTraveled > distanceTraveled)
                        {
                            thisNeighbor.DistanceTraveled = distanceTraveled;
                            thisNeighbor.DistanceToGoal = distanceTraveled + heuristic;
                            thisNeighbor.Parent = currentNode;
                        }
                    }
                }

                //====================
                //e) Now that we've checked all of this node's neighbors, remove it from the open list and add it to the closed list.
                //====================
                openList.Remove(currentNode);
                currentNode.InClosedList = true;
            }

            Console.WriteLine("\nNo path found.");
            //No path could be found.
            return new List<Vector2>();
        }


        //For testing nodes
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < levelHeight; y++)
            {
                for (int x = 0; x < levelWidth; x++)
                {
                    if(searchNodes[y,x] != null)
                        searchNodes[y, x].Draw(spriteBatch);
                }
            }
        }

        //Changes the movement cost of a given tile
        public void SetMovementCost(Point point, float value)
        {
            searchNodes[point.Y, point.X].MovementCost = value;
        }
    }
}
