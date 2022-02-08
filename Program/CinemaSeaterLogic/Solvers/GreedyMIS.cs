using CinemaSeaterLogic.Models;
using CinemaSeaterLogic.SeatingStrategies;
using System.Collections.Generic;
using System.Linq;

namespace CinemaSeaterLogic.Solvers
{
    public class GreedyMIS
    {
        private readonly Graph _graph;

        public GreedyMIS(Graph graph)
        {
            _graph = graph;
        }

        public void Solve(SeatingStrategy seatingStrategy, Ordering misOrdering)
        {
            while (_graph.GetVerticesWithLabel("e").Any() && misOrdering.HasNext() && seatingStrategy.HasNextGroup())
            {
                if (misOrdering.GetNext(out var currentVertex))
                {
                    if (_graph.GetVerticesWithLabel("e").Contains(currentVertex))
                    {
                        IEnumerable<int> seatingPath = SolverHelper.GetTotalSeatingPath(_graph, currentVertex);
                        
                        if (seatingStrategy.GetNextGroup(out var groupSize, seatingPath.Count()))
                        {
                            seatingPath = seatingPath.Take(groupSize);
                            _graph.SetLabel(_graph.GetAdjacentVertices(seatingPath), "o");
                            _graph.SetLabel(seatingPath, groupSize.ToString());
                        }
                    }
                }
            }        
        }

        public Graph GetGraph()
        {
            return _graph;
        }
    }
}
