using CinemaSeaterRunner.Models;
using CinemaSeaterRunner.Runners;
using Serilog;
using Serilog.Events;
using System.Threading.Tasks;

namespace CinemaSeaterRunner
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var options = ProgramOptions.Parse(args);

            var loggerConfig = new LoggerConfiguration().WriteTo.Console();

            if (options.Debug)
            {
                loggerConfig = loggerConfig.MinimumLevel.Debug();
            }
            else
            {
                loggerConfig = loggerConfig.MinimumLevel.Information();
            }

            var rnd = new System.Random();

            using var logger = loggerConfig.CreateLogger();

            if (options.Mode == ProgramMode.Experiments)
            {
                var runner = new ExperimentRunner(options, logger, rnd);
                runner.Run();
            }
            else
            {
                var runner = new InstanceRunner(options, logger, rnd);
                runner.Run();
            }          
        }
    }

}
