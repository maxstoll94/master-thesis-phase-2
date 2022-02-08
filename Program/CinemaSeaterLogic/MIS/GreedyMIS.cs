using CinemaSeaterLogic.Models;
using System.Collections.Generic;
using System.Linq;

namespace CinemaSeaterLogic.MIS
{
    public class GreedyMIS
    {
        public void Find(Graph graph)
        {
            var w = new HashSet<int>(graph.GetVerticesWithLabel("e"));

            while (w.Any())
            {
                var vertex = FindMinimumVertex(graph, w);

                foreach (var neighbour in graph.GetAdjacentVertices(vertex))
                {
                    w.Remove(neighbour);
                }

                w.Remove(vertex);
                graph.SetMIS(vertex);
            }
        }

        private int FindMinimumVertex(Graph graph, IEnumerable<int> w)
        {
            var min = w.First();

            foreach (var v in w)
            {
                if (graph.GetDegree(v) < min)
                {
                    min = v;
                }
            }

            return min;
        }

        //private IEnumerable<int> Find(Graph graph, int startIndex, int endIndex)
        //{
        //    // find MIS
        //    var mis = new List<int>();

        //    // loop through all disks
        //    for (int v1 = startIndex; v1 <= endIndex; v1++)
        //    {
        //        if (graph.GetLabel(v1) == "e")
        //        {
        //            // if the empty set is empty add the first vertex
        //            if (!mis.Any())
        //            {
        //                mis.Add(v1);
        //            }
        //            else
        //            {
        //                var isDisjoint = true;

        //                // check if the current disk intersects with any disk in the MIS
        //                for (int i = 0; i < mis.Count; i++)
        //                {
        //                    var v2 = mis[i];

        //                    if (graph.HasEdge(v1, v2))
        //                    {
        //                        isDisjoint = false;
        //                    }
        //                }

        //                // add the current disk if it does not intersect with any disk in MIS
        //                if (isDisjoint)
        //                {
        //                    mis.Add(v1);
        //                }
        //            }
        //        }
        //    }

        //    return mis;
        //}
    }
}
