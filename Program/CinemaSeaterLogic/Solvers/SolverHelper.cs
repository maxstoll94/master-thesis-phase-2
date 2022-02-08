using CinemaSeaterLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CinemaSeaterLogic.Solvers
{
    public static class SolverHelper
    {
        private enum Operator
        {
            Plus,
            Minus
        }

        public static IOrderedEnumerable<int> GetSeatingPath(Graph graph, int startingVertex)
        {
            return GetSeatingPathRec(graph, startingVertex, new List<int>(), 8, Operator.Plus)
                .OrderBy(s => s);
        }

        private static IEnumerable<int> GetSeatingPathRec(Graph graph, int vertex, List<int> seatingPath, int max, Operator op)
        {
            var nextVertex = op == Operator.Plus ? vertex + 1 : vertex - 1;

            if (graph.GetLabel(vertex) != "e")
            {
                return seatingPath;
            }
            else if (seatingPath.Count() == max)
            {
                return seatingPath;
            }
            else if (!graph.HasEdgeWithWeight(vertex, nextVertex, 1))
            {
                seatingPath.Add(vertex);
                return seatingPath;
            }
            else
            {
                seatingPath.Add(vertex);
                return GetSeatingPathRec(graph, nextVertex, seatingPath, max, op);
            }
        }

        public static IOrderedEnumerable<int> GetTotalSeatingPath(Graph graph, int startingVertex)
        {
            var seatingPath = new List<int>();
            var currentVertex = startingVertex;

            // iterate to the left
            GetSeatingPathRec(graph, currentVertex, seatingPath, 8, Operator.Minus);

            // iterate to the right
            return GetSeatingPathRec(graph, currentVertex, seatingPath, 8, Operator.Plus)
                .Distinct()
                .OrderBy(s => s);
        }

        public static int GetDegreeOfSeatingPath(Graph graph, IOrderedEnumerable<int> seatingPath)
        {
            return graph.GetDegree(seatingPath).Sum();
        }
    }
}
