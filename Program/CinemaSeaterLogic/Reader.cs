using CinemaSeaterLogic.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CinemaSeaterLogic
{
    public class Reader
    {
        public static Cinema Read(string filepath)
        {
            var groups = new Dictionary<int, int>(8);
            int[][] seatMatrix = null;
            List<(int, int)> coordinates = new List<(int, int)>();
            int height = 0;
            int width = 0;

            using (StreamReader reader = File.OpenText(filepath))
            {
                height = int.Parse(reader.ReadLine());
                width = int.Parse(reader.ReadLine());

                seatMatrix = new int[width][];

                for (int i = 0; i < seatMatrix.Length; i++)
                {
                    seatMatrix[i] = new int[height];
                }

                var lines = new string[height];

                for (int i = 0; i < height; i++)
                {
                    lines[i] = reader.ReadLine();
                }

                lines = lines.Reverse().ToArray();

                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];

                    for (int j = 0; j < width; j++)
                    {
                        seatMatrix[j][i] = line[j] - 48;
                        coordinates.Add((j, i));
                    }

                }

                var groupsAsString = reader.ReadLine();
                var groupSize = 1;

                foreach (var groupCount in groupsAsString.Split(' '))
                {
                    if (!string.IsNullOrWhiteSpace(groupCount))
                    {
                        groups.Add(groupSize, int.Parse(groupCount));
                        groupSize++;
                    }
                }
            }

            return new Cinema(groups, seatMatrix, coordinates.ToArray(), width, height, Path.GetFileName(filepath));
        }
    }
}
