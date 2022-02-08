using CinemaSeaterLogic.Models;
using CinemaSeaterLogic.SeatingStrategies;
using System.Collections.Generic;
using System.Linq;

namespace CinemaSeaterLogic.Solvers
{
    public class Greedy
    {
        private readonly Graph _graph;

        public Greedy(Graph graph)
        {
            _graph = graph;
        }

        public void Solve(Ordering groupOrdering)
        {
            while (_graph.GetVerticesWithLabel("e").Any() && groupOrdering.HasNext())
            {
                if (groupOrdering.GetNext(out var groupSize))
                {
                    var selectedSeatingPath = FindPath(groupSize);

                    if (selectedSeatingPath != null)
                    {
                        _graph.SetLabel(_graph.GetAdjacentVertices(selectedSeatingPath), "o");
                        _graph.SetLabel(selectedSeatingPath, groupSize.ToString());                   
                    }
                };
            }
        }

        private IOrderedEnumerable<int> FindPath(int groupSize)
        {
            var seatingPaths = new List<IOrderedEnumerable<int>>();

            while(!seatingPaths.Any() && groupSize <= 8)
            {
                foreach (var vertex in _graph.GetVerticesWithLabel("e"))
                {
                    var seatingPath = SolverHelper.GetSeatingPath(_graph, vertex);

                    if (seatingPath.Count() == groupSize)
                    {
                        seatingPaths.Add(seatingPath);
                    }
                }

                groupSize++;
            }
            
            if (seatingPaths.Any())
            {
                var selected = seatingPaths.First();
                var selectedDegree = SolverHelper.GetDegreeOfSeatingPath(_graph, selected);

                foreach (var seatingPath in seatingPaths)
                {
                    var currentDegree = SolverHelper.GetDegreeOfSeatingPath(_graph, seatingPath);

                    if (currentDegree < selectedDegree)
                    {
                        selected = seatingPath;
                    }
                }

                return selected;
            }

            return null;
        }

        public Graph GetGraph()
        {
            return _graph;
        }
    }
}
