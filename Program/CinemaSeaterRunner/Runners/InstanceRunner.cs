using CinemaSeaterLogic;
using CinemaSeaterLogic.MIS;
using CinemaSeaterLogic.Models;
using CinemaSeaterLogic.SeatingStrategies;
using CinemaSeaterLogic.Solvers;
using CinemaSeaterRunner.Models;
using Serilog.Core;
using System;
using System.IO;
using System.Linq;

namespace CinemaSeaterRunner.Runners
{
    public class InstanceRunner
    {
        private readonly ProgramOptions _options;
        private readonly Logger _logger;
        private readonly System.Random _rnd;

        public InstanceRunner(ProgramOptions options, Logger logger, System.Random rnd)
        {
            _options = options;
            _logger = logger;
            _rnd = rnd;
        }

        public void Run()
        {
            var instance = Reader.Read(_options.InstanceConfig.InstanceFile);
            var madsInstance = Offline.CinemaReader.Read(_options.InstanceConfig.InstanceFile);
            var capacity = instance.GetCapacity();

            Console.WriteLine("Solving: " + _options.InstanceConfig.InstanceFile);

            Console.WriteLine(instance);

            var constructor = new CinemaConstructor(_logger);
            var result = constructor.ConstructWithOptimalMIS(instance, _options.ExcludeDiagnal);
            var graph = result.Graph;

            var solver = new CinemaSolver(graph, _logger, _options.Debug);

            switch (_options.SolverType)
            {
                case SolverType.Greedy_LF:
                    solver.RunGreedy(instance, new LargestFirst(instance.ToGroupList()));
                    break;
                case SolverType.Greedy_SF:
                    solver.RunGreedy(instance, new SmallestFirst(instance.ToGroupList()));
                    break;
                case SolverType.Greedy_Random:
                    solver.RunGreedy(instance, new CinemaSeaterLogic.SeatingStrategies.Random(instance.ToGroupList(), _rnd));
                    break;

                case SolverType.Greedy_MIS_LF:
                    solver.RunGreedyMIS(instance, new LargestFirst(graph.GetMIS()));
                    break;
                case SolverType.Greedy_MIS_SF:
                    Console.WriteLine(graph.GetMIS().Count());
                    solver.RunGreedyMIS(instance, new SmallestFirst(graph.GetMIS()));
                    break;
                case SolverType.Greedy_MIS_Random:
                    solver.RunGreedyMIS(instance, new CinemaSeaterLogic.SeatingStrategies.Random(graph.GetMIS(), _rnd));
                    break;
                case SolverType.ILP:
                    solver.RunOptimal(instance, false);
                    break;
                case SolverType.ILP_MIS:
                    solver.RunOptimal(instance, true);
                    break;
                case SolverType.MADS_ILP:
                    var madsILPSolver = new Offline.ILPSolver(madsInstance);
                    madsILPSolver.Solve(false, false);
                    break;
                case SolverType.MADS_Greedy:
                    var madsGreedySolver = new Offline.GreedySolver(madsInstance);
                    madsGreedySolver.Solve();
                    break;
            }

            _logger.Debug("Writing IS Set.");

            foreach (var item in graph.GetMIS())
            {
                _logger.Debug(item + ",");
     
            }

            Console.WriteLine();

            if (_options.SolverType.ToString().Contains("MADS"))
            {
                Console.WriteLine(madsInstance);
                Console.WriteLine($"Percentage seated: {(double)madsInstance.CountSeated() / (double)capacity}");
                Console.WriteLine("People seated: " + madsInstance.CountSeated() + " out of " + madsInstance.TotalNumberOfPeople);
                Console.WriteLine($"Valid cinema: {madsInstance.Verify()}");
            }
            else
            {
                Console.WriteLine(instance);
                Console.WriteLine($"Percentage seated: {(double)instance.GetNumberOfPeopleSeated() / (double)capacity}");
                Console.WriteLine("People seated: " + instance.GetNumberOfPeopleSeated() + " out of " + instance.GetNumberOfPeople());
                Console.WriteLine($"Valid cinema: {instance.Verify()}");
                Console.WriteLine($"All seated: {instance.AllGroupsSeated()}");
            }       
        }
    }
}
