using CinemaSeaterLogic.Models;
using CinemaSeaterLogic.SeatingStrategies;
using CinemaSeaterLogic.Solvers;
using Serilog.Core;
using System.Collections.Generic;

namespace CinemaSeaterLogic
{
    public class CinemaSolver
    {
        private readonly GreedyMIS _greedyMIS;
        private readonly Greedy _greedy;
        private readonly ILP _ilp;

        private readonly Logger _logger;


        private readonly bool _debug;

        public CinemaSolver(Graph graph, Logger logger, bool debug)
        {
            _logger = logger;
            _greedy = new Greedy(graph);
            _greedyMIS = new GreedyMIS(graph);
            _ilp = new ILP(graph);
            _debug = debug;
        }

        public string RunGreedy(Cinema instance, Ordering groupOrdering)
        {
            _logger.Debug("Starting to solve using greedy synchronously.");

            var solvingTime = Utils.TimeAction(() => _greedy.Solve(groupOrdering));
            instance.SeatGroups(_greedy.GetGraph());

            return solvingTime;
        }

        public string RunGreedyMIS(Cinema instance, Ordering misOrdering)
        {
            _logger.Debug("Starting to solve using greedy synchronously.");

            var seatingStrategy = new BestFitStrategy(instance.ToGroupList());
            var solvingTime = Utils.TimeAction(() => _greedyMIS.Solve(seatingStrategy, misOrdering));
            instance.SeatGroups(_greedyMIS.GetGraph());

            return solvingTime;
        }

        public (string, int) RunOptimal(Cinema instance, bool useMIS)
        {
            _logger.Debug("Starting to solve using optimal.");
            return _ilp.Solve(instance, useMIS, _debug);
        }
    }
}
