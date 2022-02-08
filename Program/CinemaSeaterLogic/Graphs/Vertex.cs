using System.Collections.Generic;
using System.Linq;

namespace CinemaSeaterLogic.Graphs
{
    public class Vertex
    {
        private readonly LinkedList<Edge> edges;
        private readonly int id;
        public Point Pos;
        public string Label;
        public int MaxSeated;
        public bool IsInMIS;
        public int Degree;

        public Vertex(int id)
        {
            edges = new LinkedList<Edge>();
            this.id = id;
        }

        public int GetId()
        {
            return id;
        }

        public void AddEdge(Edge edge)
        {
            edges.AddLast(edge);
        }

        public IEnumerable<Edge> GetEdges()
        {
            return edges;
        }

        public string ToXml()
        {
            return 
            $@"<node id=""{id}"">
                <data key = ""d1"">{Label}</data>
                <data key = ""d2"">{Pos}</data>
                <data key = ""d4"">{MaxSeated}</data>
                <data key = ""d5"">{IsInMIS}</data>
                <data key = ""d6"">{Degree}</data>
            </node>";
        }
    }

    public class Point
    {
        public readonly double X;
        public readonly double Y;

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        internal static Point Parse(string value)
        {
            var coordinates = value.Split(',')
                .Select(c => double.Parse(c));

            return new Point(coordinates.ElementAt(0), coordinates.ElementAt(1));
        }
    }
}
