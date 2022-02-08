using CinemaSeaterLogic;
using CinemaSeaterLogic.MIS;
using CinemaSeaterLogic.Models;
using CinemaSeaterLogic.SeatingStrategies;
using CinemaSeaterRunner.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Serilog.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CinemaSeaterRunner
{
    public class ExperimentRunner
    {
        private ProgramOptions _options { get; set; }

        private string _instancesFolder = null;
        private string _graphFolder = null;
        private const string _resultsFolder = @"./Results/Experiment_results/";
        private const string _solvingFolder = @"./Results/Solving_results/";

        private readonly CsvConfiguration _csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        };

        private readonly Logger _logger;

        private readonly TimeSpan _timeout;
        private static ConcurrentQueue<SolverResult> _solverResults = new ConcurrentQueue<SolverResult>();

        private System.Random _rnd;
        private object locking_object = new object();

        public ExperimentRunner(ProgramOptions options, Logger logger, System.Random rnd)
        {
            _options = options;
            _instancesFolder = options.ExperimentsConfig.InstancesFolder;
            _graphFolder = options.ExperimentsConfig.GraphFolder;
            _timeout = TimeSpan.FromSeconds(options.TimeOut);

            if (!Directory.Exists(_instancesFolder))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Directory.Exists(_resultsFolder))
            {
                Directory.CreateDirectory(_resultsFolder);
            }

            if (!Directory.Exists(_graphFolder))
            {
                Directory.CreateDirectory(_graphFolder);
            }

            if (!Directory.Exists(_solvingFolder))
            {
                Directory.CreateDirectory(_solvingFolder);
            }

            _logger = logger;
            _rnd = rnd;
        }


        public void Run()
        {
            _logger.Information($"Running experiment with type: {_options.ExperimentsConfig.Type}");

            if (_options.ExperimentsConfig.Type == ExperimentType.Construct || _options.ExperimentsConfig.Type == ExperimentType.All)
            {
                RunConstructor();
            }

            if (_options.ExperimentsConfig.Type == ExperimentType.Solve || _options.ExperimentsConfig.Type == ExperimentType.All)
            {
                var tasks = new List<Task>();

                var source = new CancellationTokenSource();
                var token = source.Token;

                var writeTask = Task.Run(() =>
                {
                    var resultsFile = $"{_resultsFolder}/solver_{_options.SolverType}_{_options.MISType}_{_options.ExperimentsConfig.Name}.csv";

                    using var stream = File.Open(resultsFile, FileMode.Create);
                    using var streamWriter = new StreamWriter(stream);
                    using var writer = new CsvWriter(streamWriter, _csvConfiguration);

                    writer.WriteHeader<SolverResult>();
                    writer.NextRecord();

                    var numberOfFiles = GetFiles(new DirectoryInfo(_instancesFolder)).Count();
                    var totalNumberOfRecords = numberOfFiles;
                    var numberOfRecordsWritten = 0;

                    while (numberOfRecordsWritten != totalNumberOfRecords)
                    {
                        if (token.IsCancellationRequested)
                        {
                            streamWriter.Flush();
                            return;
                        }

                        SolverResult result = null;

                        while (_solverResults.TryDequeue(out result))
                        {
                            writer.WriteRecord(result);
                            writer.NextRecord();

                            lock (locking_object)
                            {
                                numberOfRecordsWritten++;
                            }

                        }
                    }
                }, token);

                var solveTask = Task.Run(() => RunSolver(_options.SolverType, _options.MISType));

                Task.WaitAll(writeTask, solveTask);
            }
        }
        private void RunSolver(SolverType solverType, MISType misType)
        {
            var instanceDirectory = new DirectoryInfo(_instancesFolder);

            foreach (var file in GetFiles(instanceDirectory))
            {
                _logger.Information($"Solving instance: {file.FullName}");

                var instance = Reader.Read(file.FullName);
                var madsInstance = Offline.CinemaReader.Read(file.FullName);

                var result = new SolverResult
                {
                    InstanceFile = file.Name,
                    SolverType = solverType.ToString(),
                    Size = instance.GetSize(),
                    Capacity = instance.GetCapacity(),
                    Height = instance.Height,
                    Width = instance.Width,

                    NumberOfGroups = instance.GetNumberOfGroups(),
                    NumberOfPeople = instance.GetNumberOfPeople(),
                    MISType = misType.ToString()
                };

                Graph graph = null;

                try
                {
                    graph = ReadGraph(Path.GetFileNameWithoutExtension(file.Name), misType);
                }
                catch (FileNotFoundException)
                {
                    _logger.Error($"Graph file not found: {file.FullName}");
                    InvalidateResult(result, false, false);
                    _solverResults.Enqueue(result);
                    continue;
                }

                var solver = new CinemaSolver(graph, _logger, _options.Debug);

                var madsILPSolver = new Offline.ILPSolver(madsInstance);
                var madsGreedySolver = new Offline.GreedySolver(madsInstance);

                Task<(string, int)> task = null;

                CancellationTokenSource tokenSource = new CancellationTokenSource();

                task = Task.Run(() => HandleSolverType(file.Name, solverType, instance, graph, solver, madsILPSolver, madsGreedySolver), tokenSource.Token);

                if (task.Wait(_timeout))
                {
                    (var solvingTime, int variableCount) = task.Result;

                    if (solvingTime == null)
                    {
                        _logger.Error($"Out of memory exception solving for instance: {file.FullName}");
                        tokenSource.Cancel();
                        InvalidateResult(result, false, true);
                        _solverResults.Enqueue(result);
                        continue;
                    }

                    File.WriteAllText($"{_solvingFolder}/{Path.GetFileNameWithoutExtension(file.Name)}_{solverType}_{misType}_{_options.ExperimentsConfig.Name}_cinema.txt", solverType.ToString().Contains("MADS") ? madsInstance.ToString() : instance.ToString());
                    File.WriteAllText($"{_solvingFolder}/{Path.GetFileNameWithoutExtension(file.Name)}_{solverType}_{misType}_{_options.ExperimentsConfig.Name}_graph.xml", graph.Serialize());

                    result.SolvingTime = solvingTime;
                    result.NumberOfGroupsSeated = solverType.ToString().Contains("MADS") ? 0 : instance.GetNumberOfGroupsSeated();
                    result.NumberOfPeopleSeated = solverType.ToString().Contains("MADS") ? madsInstance.CountSeated() : instance.GetNumberOfPeopleSeated();
                    result.Valid = solverType.ToString().Contains("MADS") ? madsInstance.Verify() : instance.Verify();
                    result.AllSeated = solverType.ToString().Contains("MADS") ? madsInstance.CountSeated() == madsInstance.TotalNumberOfPeople : instance.AllGroupsSeated();
                    result.Timeout = false;
                    result.OutOfMemory = false;
                    result.VariableCount = variableCount;
                }
                else
                {
                    _logger.Error($"Timeout solving for instance: {file.FullName}");
                    tokenSource.Cancel();
                    InvalidateResult(result, true, false);
                }

                _solverResults.Enqueue(result);
            }
        }

        private void InvalidateResult(SolverResult result, bool timeout, bool outOfMemory)
        {
            var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
               _timeout.Hours, _timeout.Minutes, _timeout.Seconds,
               _timeout.Milliseconds / 10);

            result.SolvingTime = timeout ? elapsedTime : "00:00:00.00";
            result.NumberOfGroupsSeated = 0;
            result.NumberOfPeopleSeated = 0;
            result.Valid = true;
            result.AllSeated = false;
            result.Timeout = timeout;
            result.OutOfMemory = outOfMemory;
        }

        private (string, int) HandleSolverType(string fileName, SolverType solverType, Cinema instance, Graph graph, CinemaSolver solver,
            Offline.ILPSolver madsILPSolver, Offline.GreedySolver madsGreedySolver)
        {
            string solvingTime = null;
            int variableCount = -1;

            try
            {
                switch (solverType)
                {
                    case SolverType.Greedy_LF:
                        solvingTime = solver.RunGreedy(instance, new LargestFirst(instance.ToGroupList()));
                        break;
                    case SolverType.Greedy_SF:
                        solvingTime = solver.RunGreedy(instance, new SmallestFirst(instance.ToGroupList()));
                        break;
                    case SolverType.Greedy_Random:
                        solvingTime = solver.RunGreedy(instance, new CinemaSeaterLogic.SeatingStrategies.Random(instance.ToGroupList(), _rnd));
                        break;
                    case SolverType.Greedy_MIS_LF:
                        solvingTime = solver.RunGreedyMIS(instance, new LargestFirst(graph.GetMIS()));
                        break;
                    case SolverType.Greedy_MIS_SF:
                        solvingTime = solver.RunGreedyMIS(instance, new SmallestFirst(graph.GetMIS()));
                        break;
                    case SolverType.Greedy_MIS_Random:
                        solvingTime = solver.RunGreedyMIS(instance, new CinemaSeaterLogic.SeatingStrategies.Random(graph.GetMIS(), _rnd));
                        break;
                    case SolverType.ILP:
                        (solvingTime, variableCount) = solver.RunOptimal(instance, false);
                        break;
                    case SolverType.ILP_MIS:
                        (solvingTime, variableCount) = solver.RunOptimal(instance, true);
                        break;
                    case SolverType.MADS_ILP:
                        (var ilpTimes, var count) = madsILPSolver.Solve(false, false);
                        variableCount = count;
                        solvingTime = ilpTimes["Optimizing"];
                        break;
                    case SolverType.MADS_Greedy:
                        var greedyTimes = madsGreedySolver.Solve();
                        solvingTime = greedyTimes["Total"];
                        break;
                }

                return (solvingTime, variableCount);
            }
            catch (OutOfMemoryException)
            {
                return (null, variableCount);
            }
        }

        private void RunConstructor()
        {
            var resultsFile = $"{_resultsFolder}/construction_{_options.MISType}_{_options.ExperimentsConfig.Name}.csv";
            var cinemaConstructor = new CinemaConstructor(_logger);
            var instanceDirectory = new DirectoryInfo(_instancesFolder);

            using (var writer = new StreamWriter(resultsFile))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader(typeof(ConstructionResult));
                csv.NextRecord();
            }

            foreach (var file in GetFiles(instanceDirectory))
            {
                try
                {
                    using var stream = File.Open(resultsFile, FileMode.Append);
                    using var writer = new StreamWriter(stream);
                    using var csv = new CsvWriter(writer, _csvConfiguration);

                    _logger.Debug($"Constructing instance: {file.FullName}");

                    var instance = Reader.Read(file.FullName);

                    var result = new ConstructionResult
                    {
                        InstanceFile = file.Name,
                        Size = instance.GetSize(),
                        Capacity = instance.GetCapacity(),
                        Height = instance.Height,
                        Width = instance.Width,
                        MISType = _options.MISType.ToString()
                    };

                    var task = Task.Run(() => HandleConstructionType(cinemaConstructor, instance));

                    if (task.Wait(_timeout))
                    {
                        ConstructorResult constructionResult = task.Result;

                        var graph = constructionResult.Graph;
                        var constructorTimes = constructionResult.Times;

                        File.WriteAllText($"{_graphFolder}/{Path.GetFileNameWithoutExtension(file.Name)}_{_options.MISType}_{_options.ExperimentsConfig.Name}.xml", graph.Serialize());

                        result.ConstructionTime = constructorTimes["Construct"];
                        result.MISFindingTime = constructorTimes.ContainsKey("FindMIS") ? constructorTimes["FindMIS"] : null;
                        result.MISLength = graph.GetMIS().Count();
                        result.Timeout = false;
                    }
                    else
                    {
                        _logger.Error($"Timeout construction for instance: {file.FullName}");
                        result.ConstructionTime = String.Format("{0:00}:{1:00}:{2:00}", _timeout.Hours, _timeout.Minutes, _timeout.Seconds);
                        result.MISFindingTime = String.Format("{0:00}:{1:00}:{2:00}", _timeout.Hours, _timeout.Minutes, _timeout.Seconds);
                        result.MISLength = 0;
                        result.Timeout = true;
                    }

                    csv.WriteRecord(result);
                    csv.NextRecord();
                }
                catch (Exception ex)
                {
                    _logger.Error($"An unexpected error has occurd: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        private ConstructorResult HandleConstructionType(CinemaConstructor cinemaConstructor, Cinema instance)
        {
            ConstructorResult constructionResult;

            bool excludeDiagnal = _options.ExcludeDiagnal;

            switch (_options.MISType)
            {
                case MISType.Greedy:
                    constructionResult = cinemaConstructor.ConstructWithGreedyMIS(instance, excludeDiagnal);
                    break;
                case MISType.ILP:
                    constructionResult = cinemaConstructor.ConstructWithOptimalMIS(instance, excludeDiagnal);
                    break;
                case MISType.None:
                    constructionResult = cinemaConstructor.RunWithoutMIS(instance, excludeDiagnal);
                    break;
                default:
                    throw new NotSupportedException();
            }

            return constructionResult;
        }

        private Graph ReadGraph(string name, MISType type)
        {
            var xml = File.ReadAllText(@$"{_graphFolder}/{name}_{type}_{_options.ExperimentsConfig.Name}.xml");
            return Graph.Parse(xml);
        }

        private IOrderedEnumerable<FileInfo> GetFiles(DirectoryInfo info)
        {
            return info.GetFiles("*.txt").OrderBy(n => Regex.Replace(n.Name, @"\d+", n => n.Value.PadLeft(9, '0')));
        }

        private class ConstructionResult
        {
            public string InstanceFile { get; set; }
            public string MISType { get; set; }


            public int Width { get; set; }
            public int Height { get; set; }
            public int Size { get; set; }
            public int Capacity { get; set; }

            public bool Timeout { get; set; }

            public string ConstructionTime { get; set; }
            public string MISFindingTime { get; set; }
            public int MISLength { get; set; }
        }

        private class SolverResult
        {
            public string InstanceFile { get; set; }

            public string SolverType { get; set; }
            public string MISType { get; set; }
            public string OrderingType { get; set; }

            public int Width { get; set; }
            public int Height { get; set; }

            public int Size { get; set; }
            public int Capacity { get; set; }

            public int NumberOfGroups { get; set; }
            public int NumberOfPeople { get; set; }

            public int NumberOfGroupsSeated { get; set; }
            public int NumberOfPeopleSeated { get; set; }
            public double PercentageSeated { get { return (double)NumberOfPeopleSeated / (double)Capacity; } }
            public double PercentageOccupied { get { return (double)NumberOfPeople / (double)Capacity; } }
            public double PercentageEmpty { get { return (double)Capacity / (double)Size; } }

            public bool Valid { get; set; }
            public bool AllSeated { get; set; }

            public bool Timeout { get; set; }

            public bool OutOfMemory { get; set; }

            public string SolvingTime { get; set; }

            public int VariableCount { get; set; }
            public int ConstraintCount { get; set; }
        }

        private enum OrderingType
        {
            LargestFirst,
            SmallestFirst,
            Random
        }
    }
}
