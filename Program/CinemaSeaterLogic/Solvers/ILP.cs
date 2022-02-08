using CinemaSeaterLogic.ILPS;
using CinemaSeaterLogic.Models;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSeaterLogic.Solvers
{
    public class ILP
    {
        private Graph _graph;

        public ILP(Graph graph)
        {
            _graph = graph;
        }

        public (string, int) Solve(Cinema instance, bool useMIS, bool debug)
        {
            var seatWeights = _graph.GetMaxSeats();
            var startingSeats = useMIS ? _graph.GetMIS() : _graph.GetVerticesWithLabel("e");

            var result = CinemaSeaterILP.Solve(startingSeats, seatWeights, instance.ToGroupList(), _graph, new MIS.ILPSettings { Debug = debug });

            instance.SeatGroups(_graph);

            return result;
        }
    }
}
