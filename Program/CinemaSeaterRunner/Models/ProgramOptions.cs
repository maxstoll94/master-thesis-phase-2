using CinemaSeaterLogic.Models;
using Mono.Options;
using System;
using System.Collections.Generic;

namespace CinemaSeaterRunner.Models
{
    public enum ProgramMode
    {
        Experiments,
        Instance
    }

    public enum ExperimentType
    {
        Construct,
        Solve,
        All
    }

    public enum MISType
    {
        Greedy,
        ILP,
        None
    }

    public enum SolverType
    {
        Greedy_LF,
        Greedy_SF,
        Greedy_Random,

        Greedy_MIS_LF,
        Greedy_MIS_SF,
        Greedy_MIS_Random,

        ILP,
        ILP_MIS,

        MADS_ILP,
        MADS_Greedy
    }

    public class ExperimentsConfig
    {
        public ExperimentType Type { get; set; }

        public string Name { get; set; }
        public string GraphFolder { get; set; }
        public string InstancesFolder { get; set; }
    }

    public class InstanceConfig
    {
        public string InstanceFile { get; set; }
        public string GraphFile { get; set; }
    }

    public class ProgramOptions
    {
        public bool Debug { get; set; } = false;
        public bool ExcludeDiagnal { get; set; } = false;
        public int TimeOut { get; set; } = 30;
        public ProgramMode Mode { get; set; } = ProgramMode.Instance;
        public ExperimentsConfig ExperimentsConfig { get; set; }
        public InstanceConfig InstanceConfig { get; set; }
        public MISType MISType { get; set; }

        public SolverType SolverType { get; set; }

        public ProgramOptions()
        {
            ExperimentsConfig = new ExperimentsConfig();
            InstanceConfig = new InstanceConfig();
        }

        public static ProgramOptions Parse(string[] args)
        {
            var programOptions = new ProgramOptions();

            var graphPath = "";
            var instancesPath = "";

            var options = new OptionSet {
                { "d|debug", "run the program in debug mode.", d => programOptions.Debug = d != null },
                { "to|timeout=", "Timeout of the program in seconds", t => programOptions.TimeOut = int.Parse(t) },
                { "m|mode=", "the mode the program should run in: experiments for experiments | instance (default) for single instance.", m => programOptions.Mode = Enum.Parse<ProgramMode>(m, true) },
                { "e|exp=", "the type of experiment", e => programOptions.ExperimentsConfig.Type = Enum.Parse<ExperimentType>(e, true) },
                { "i|instance=", "path to a instance file executed by this program | path to instances folder containing Exact{i} instances for executing experiments", i => instancesPath = i},
                { "g|graph=", "path to graph file (.xml) executed by this program | path to graph folder containing the txts for executing experiments", c => graphPath = c},
                { "mis|misType=", "type of mis to use", mis => programOptions.MISType = Enum.Parse<MISType>(mis, true)},
                { "st|solverType=", "type of mis to use", st => programOptions.SolverType = Enum.Parse<SolverType>(st, true)},
                { "n|name=", "name of the experiment", n => programOptions.ExperimentsConfig.Name = n},
                { "x|excludeDiagnal", "exclude diagnal constraint", x => programOptions.ExcludeDiagnal = x != null}
            };

            options.Parse(args);

            if (programOptions.Mode == ProgramMode.Experiments)
            {
                programOptions.ExperimentsConfig.InstancesFolder = instancesPath;
                programOptions.ExperimentsConfig.GraphFolder = graphPath;
            }
            else
            {
                programOptions.InstanceConfig.InstanceFile = instancesPath;
                programOptions.InstanceConfig.GraphFile = graphPath;
            }

            return programOptions;
        }
    }
}