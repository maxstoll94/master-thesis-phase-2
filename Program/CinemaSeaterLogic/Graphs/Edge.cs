using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CinemaSeaterLogic.Graphs
{
    public class Edge
    {
        public int SourceId { get; set; }
        public int DestinationId { get; set; }
        public int Weight { get; set; }

        public Edge(int sourceId, int destinationId, int weight)
        {
            SourceId = sourceId;
            DestinationId = destinationId;
            Weight = weight;
        }

        public string ToXml()
        {
            return $@"<edge source=""{SourceId}"" target=""{DestinationId}"">
                <data key=""d3"">{Weight}</data>
            </edge>";
        }
    }
}
