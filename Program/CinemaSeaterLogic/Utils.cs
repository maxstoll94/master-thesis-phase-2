using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace CinemaSeaterLogic.Models
{
    public static class Utils
    {
        public static string TimeAction(Action action)
        {
            var timer = new Stopwatch();
            timer.Start();
            action();
            timer.Stop();

            var ts = timer.Elapsed;

            var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);

            return elapsedTime;
        }

        public static async Task<string> TimeActionAsync(Func<Task> func)
        {
            var timer = new Stopwatch();
            timer.Start();
            await func();
            timer.Stop();

            var ts = timer.Elapsed;

            var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
             ts.Hours, ts.Minutes, ts.Seconds,
             ts.Milliseconds / 10);

            return elapsedTime;
        }

        public static async Task<(T, string)> TimeFunctionAsync<T>(Func<Task<T>> func)
        {
            var timer = new Stopwatch();
            timer.Start();
            var result = await func();
            timer.Stop();

            var ts = timer.Elapsed;

            var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);

            return (result, elapsedTime);
        }

        public static (T, string) TimeFunction<T>(Func<T> func)
        {
            var timer = new Stopwatch();
            timer.Start();
            var result = func();
            timer.Stop();

            var ts = timer.Elapsed;

            var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            return (result, elapsedTime);
        }
    }
}
