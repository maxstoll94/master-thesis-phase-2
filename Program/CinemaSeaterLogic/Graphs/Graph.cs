using CinemaSeaterLogic.Graphs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CinemaSeaterLogic.Models
{
    public class Graph
    {
        private readonly Vertex[] _adjacencyList;

        private const string graph_template = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <graphml>
                  <key id = ""d1"" for=""node"" attr.name=""label"" attr.type=""string""/>
                  <key id = ""d2"" for=""node"" attr.name=""pos"" attr.type=""string""/>
                  <key id = ""d3"" for=""edge"" attr.name=""weight"" attr.type=""int"" />
                  <key id = ""d4"" for=""node"" attr.name=""maxSeated"" attr.type=""int""/>
                  <key id = ""d5"" for=""node"" attr.name=""isInMIS"" attr.type=""boolean""/>
                  <key id = ""d6"" for=""node"" attr.name=""Degree"" attr.type=""int""/>
                  <graph id = ""G"" edgedefault=""undirected"">
                    {0}
                    {1}
                  </graph>
                </graphml>";

        public Graph(int numVertices)
        {
            _adjacencyList = new Vertex[numVertices];

            for (int i = 0; i < _adjacencyList.Length; ++i)
            {
                _adjacencyList[i] = new Vertex(i);
            }
        }

        public static Graph Parse(string content)
        {
            var graph = XElement.Parse(content);

            var nodes = from node in graph.Descendants("node")
                        select node;

            var edges = from edge in graph.Descendants("edge")
                        select edge;

            var adjacencyList = new Graph(nodes.Count());

            foreach (var node in nodes)
            {
                var id = int.Parse(node.Attribute("id").Value);
                var data = node.Descendants("data");
                var label = data.FirstOrDefault(n => n.Attribute("key").Value == "d1").Value;
                var point = Point.Parse(data.FirstOrDefault(n => n.Attribute("key").Value == "d2").Value);
                var maxSeatSize = int.Parse(data.FirstOrDefault(n => n.Attribute("key").Value == "d4").Value);
                var isInMis = bool.Parse(data.FirstOrDefault(n => n.Attribute("key").Value == "d5").Value);

                adjacencyList.SetLabel(id, label);
                adjacencyList.SetPoint(id, point);
                adjacencyList.SetMaxSeated(id, maxSeatSize);

                if (isInMis)
                {
                    adjacencyList.SetMIS(id);
                }
            }

            foreach (var edge in edges)
            {
                var source = int.Parse(edge.Attribute("source").Value);
                var target = int.Parse(edge.Attribute("target").Value);
                var data = edge.Descendants("data");
                var weight = int.Parse(data.FirstOrDefault(e => e.Attribute("key").Value == "d3").Value);

                adjacencyList.AddEdge(source, target, weight);
            }

            return adjacencyList;
        }

        public void AddEdge(int v1, int v2, int weight)
        {
            _adjacencyList[v1].AddEdge(new Edge(v1, v2, weight));
        }

        public IEnumerable<int> GetAdjacentVertices(int v)
        {
            return _adjacencyList[v].GetEdges().Select(i => i.DestinationId);
        }

        public IEnumerable<int> GetAdjacentVertices(IEnumerable<int> seatingPath)
        {
            var result = new List<int>();

            foreach (var vertex in seatingPath)
            {
                result.AddRange(GetAdjacentVertices(vertex));
            }

            return result.Distinct();
        }

        public IEnumerable<int> GetAdjacentVertices(int v, int weight)
        {
            return _adjacencyList[v].GetEdges()
                .Where(i => i.Weight == weight)
                .Select(i => i.DestinationId);
        }

        public int GetEdgeWeight(int v1, int v2)
        {
            return _adjacencyList[v1].GetEdges()
                .Where(i => i.DestinationId == v2)
                .Select(i => i.Weight)
                .SingleOrDefault();
        }

        public string Serialize()
        {
            List<string> xmlNodes = new List<string>();
            List<string> xmlEdges = new List<string>();

            for (int v = 0; v < GetNumberOfVertices(); v++)
            {
                var vertex = _adjacencyList[v];
                var edges = vertex.GetEdges();

                foreach (var edge in edges)
                {
                    xmlEdges.Add(edge.ToXml());
                }

                xmlNodes.Add(vertex.ToXml());
            }

            return string.Format(graph_template, string.Join("", xmlNodes), string.Join("", xmlEdges));
        }

        public string GetLabel(int v)
{
            return _adjacencyList[v].Label;
        }

        public void SetLabel(int v, string label)
        {
            _adjacencyList[v].Label = label;
        }

        public void SetLabel(IEnumerable<int> seatingPath, string label)
        {
            foreach (var vertex in seatingPath)
            {
                _adjacencyList[vertex].Label = label;
            }
        }

        public void SetPoint(int v, Point point)
        {
            _adjacencyList[v].Pos = point;
        }

        public IEnumerable<string> GetLabels()
        {
            return _adjacencyList.Select(v => v.Label);
        }

        public IEnumerable<int> GetVerticesWithLabel(string label)
        {
            return _adjacencyList.Where(v => v.Label == label)
                .Select(v => v.GetId());
        }

        public IEnumerable<int> GetVertices()
        {
            return _adjacencyList.Select(v => v.GetId());
        }

        public void SetMIS(int v)
        {
            _adjacencyList[v].IsInMIS = true;
        }

        public void SetDegree(int v, int degree)
        {
            _adjacencyList[v].Degree = degree;
        }

        public void SetMaxSeated(int v, int maxSeated)
        {
            _adjacencyList[v].MaxSeated = maxSeated;
        }

        public int GetNumberOfVertices()
        {
            return _adjacencyList.Count();
        }

        public bool HasEdge(int v1, int v2)
        {
            return _adjacencyList[v1].GetEdges()
                .Select(i => i.DestinationId)
                .Contains(v2);
        }

        public bool HasEdgeWithWeight(int v1, int v2, int weight)
        {
            return _adjacencyList[v1].GetEdges()
                .Where(i => i.Weight == weight)
                .Select(i => i.DestinationId)
                .Contains(v2);
        }

        public IEnumerable<int> GetMIS()
        {
            return _adjacencyList
                .Where(v => v.IsInMIS)
                .Select(v => v.GetId());
        }

        public IDictionary<int, int> GetMaxSeats()
        {
            return _adjacencyList
                .Where(v => v.Label != "s")
                .ToDictionary(v => v.GetId(), v => v.MaxSeated);
        }

        public IEnumerable<int> GetMaxSeat(int maxSeated)
        {
            return _adjacencyList
                .Where(v => v.Label == "e" && v.MaxSeated == maxSeated)
                .Select(v => v.GetId());
        }

        public int GetDegree(int v)
        {
            return _adjacencyList[v].Degree;
        }

        public IEnumerable<int> GetDegree(IEnumerable<int> vertices)
        {
            return vertices.Select(sp => GetDegree(sp));
        }
    }
}
