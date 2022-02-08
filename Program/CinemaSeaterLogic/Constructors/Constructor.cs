using CinemaSeaterLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaSeaterLogic.Constructors
{
    public class Constructor
    {
        public Graph Construct(Cinema instance, bool excludeDiagnol)
        {
            var numberOfVertices = instance.GetSize();
            var seatCoordinates = instance.Coordinates;
            var seatMatrix = instance.SeatMatrix;

            Graph emptyUDG = new Graph(numberOfVertices);

            Parallel.For(0, seatCoordinates.Count(), currentVertex =>
            {
                var (currentX, currentY) = seatCoordinates.ElementAt(currentVertex);

                var label = "e";

                if (seatMatrix[currentX][currentY] == 0)
                {
                    // s for space
                    label = "s";
                }

                emptyUDG.SetLabel(currentVertex, label);
                emptyUDG.SetPoint(currentVertex, new Graphs.Point(currentX, currentY));

                var invalidSeats = instance.GetInvalidSeats(currentX, currentY, 0, excludeDiagnol);

                emptyUDG.SetDegree(currentVertex, invalidSeats.Count());

                foreach (var invalidSeat in invalidSeats)
                {
                    var (invalidX, invalidY) = invalidSeat;

                    var weight = 1;

                    if (currentY != invalidY)
                    {
                        weight = 2;
                    }

                    var targetVertex = seatCoordinates.FindIndex(s => s == invalidSeat);

                    if (targetVertex != currentVertex)
                    {
                        emptyUDG.AddEdge(currentVertex, targetVertex, weight);
                    }
                }
            });

            return emptyUDG;
        }
    }

    public static class EnumerableExtensions
    {
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the zero-based index of the first occurrence within the entire <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="predicate"/>, if found; otherwise it'll throw.
        /// </returns>
        public static int FindIndex<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            var idx = list.Select((value, index) => new { value, index }).Where(x => predicate(x.value)).Select(x => x.index).First();
            return idx;
        }
    }
}
