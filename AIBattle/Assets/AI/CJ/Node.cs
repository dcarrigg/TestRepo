// save the map somehow
// we should have a collection of Nodes
// probably a Dictionary<int, Dictionary<int, Node>>
using SensorData = GameManager.SensorData;
using Direction = GameManager.Direction;

namespace CJ_Slav
{
    public class Node
    {
        // Was this node already visited?
        public bool visited;

        // The adjacent nodes to this node
        public Node up;
        public Node down;
        public Node left;
        public Node right;

        // the state of the tile of this node
        // represents what this space is occupied by
        public SensorData tile;

        // could be useful later on
        public Direction direction;

        public bool IsWallOrOffGrid()
        {
            return (tile & SensorData.Wall) != 0 || (tile & SensorData.OffGrid) != 0;
        }
    }
}
