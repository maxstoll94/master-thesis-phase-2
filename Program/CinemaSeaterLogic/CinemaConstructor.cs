using CinemaSeaterLogic.Constructors;
using CinemaSeaterLogic.MIS;
using CinemaSeaterLogic.Models;
using CinemaSeaterLogic.Solvers;
using Serilog.Core;
using System.Collections.Generic;
using System.Linq;

namespace CinemaSeaterLogic
{
    public class ConstructorResult
    {
        public ConstructorResult(Graph graph, Dictionary<string, string> times)
        {
            Graph = graph;
            Times = times;
        }

        public Graph Graph { get; set; }
        public Dictionary<string, string> Times { get; set; }
    }

    public class GreedyConstructorResult : ConstructorResult
    {
        public GreedyConstructorResult(Graph graph, Dictionary<string, string> times) : base(graph, times)
        {
        }
    }

    public class CinemaConstructor
    {
        private readonly Constructor _constructor;
        private readonly ILPMISFinder _ilpMisFinder;
        private readonly MIS.GreedyMIS _greedyMisFinder;
        private readonly Logger _logger;

        public CinemaConstructor(Logger logger)
        {
            _constructor = new Constructor();
            _greedyMisFinder = new MIS.GreedyMIS();
            _ilpMisFinder = new ILPMISFinder();
            _logger = logger;
        }

        public GreedyConstructorResult ConstructWithGreedyMIS(Cinema instance, bool excludeDiagnol)
        {
            (var graph, var contructingTime) = Utils.TimeFunction(() => Construct(instance, excludeDiagnol));
            var misFindingTime = Utils.TimeAction(() => SetGreedyMIS(graph));
            var maxSeatedTime = Utils.TimeAction(() => SetMaxSeated(graph));

            var times = new Dictionary<string, string>()
            {
                { "Construct", contructingTime },
                { "FindMIS", misFindingTime }
            };

            return new GreedyConstructorResult(graph, times);
        }

        public ConstructorResult ConstructWithOptimalMIS(Cinema instance, bool excludeDiagnal)
        {
            (var graph, var contructingTime) = Utils.TimeFunction(() => Construct(instance, excludeDiagnal));
            var misFindingTime = Utils.TimeAction(() => SetILPMIS(graph));
            var maxSeatedTime = Utils.TimeAction(() => SetMaxSeated(graph));

            var times = new Dictionary<string, string>()
            {
                { "Construct", contructingTime },
                { "FindMIS", misFindingTime }
            };

            return new ConstructorResult(graph, times);
        }

        public ConstructorResult RunWithoutMIS(Cinema instance, bool excludeDiagnol)
        {
            (var graph, var contructingTime) = Utils.TimeFunction(() => Construct(instance, excludeDiagnol));
            var maxSeatedTime = Utils.TimeAction(() => SetMaxSeated(graph));

            var times = new Dictionary<string, string>()
            {
                { "Construct", contructingTime }
            };

            return new ConstructorResult(graph, times);
        }

        private void SetGreedyMIS(Graph graph)
        {
            _logger.Debug("Starting to find the MIS.");
            _greedyMisFinder.Find(graph);
        }

        private void SetILPMIS(Graph graph)
        {
            _logger.Debug("Starting to find the MIS.");

            var mis = _ilpMisFinder.Find(graph);

            foreach (var m in mis)
            {
                graph.SetMIS(m);
            }
        }

        private Graph Construct(Cinema instance, bool excludeDiagnal)
        {
            _logger.Debug("Starting to construct the graph.");
            return _constructor.Construct(instance, excludeDiagnal);
        }

        private void SetMaxSeated(Graph graph)
        {
            _logger.Debug("Finding max seated.");

            for (int currentVertex = 0; currentVertex < graph.GetNumberOfVertices(); currentVertex++)
            {
                if (graph.GetLabel(currentVertex) != "s")
                {
                    var availableSeats = SolverHelper.GetSeatingPath(graph, currentVertex);
                    var maxGroupSize = availableSeats.Count();
                    graph.SetMaxSeated(currentVertex, maxGroupSize);
                }
                else
                {
                    graph.SetMaxSeated(currentVertex, 0);
                }
            }
        }
    }
}
