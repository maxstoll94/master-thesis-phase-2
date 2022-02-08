using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CinemaSeaterLogic.Models
{
    public enum SeatingResult
    {
        HorizontalViolation = 1,
        VerticalViolation = 2,
        DiagnolViolation = 3,
        NoViolation = 4
    }

    public class Cinema
    {
        public string Name { get; set; }
        public Dictionary<int, int> Groups { get; set; }
        public int[][] SeatMatrix { get; set; }
        public IEnumerable<(int, int)> Coordinates { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Cinema(Dictionary<int, int> groups, int[][] seats, IEnumerable<(int, int)> coordinates, int width, int height, string name)
        {
            SeatMatrix = seats;
            Coordinates = coordinates;
            Width = width;
            Height = height;
            Groups = groups;
            Name = name;
        }

        public void SeatGroups(Graph graph)
        {
            var labels = graph.GetLabels();

            var x = 0;
            var y = 0;
            var labelIndex = 0;

            while (y != Height)
            {
                var label = labels.ElementAt(labelIndex);

                if (label != "e" && label != "s" && label != "o")
                {
                    SeatMatrix[x][y] = 2;
                }

                labelIndex++;

                if (x == Width - 1)
                {
                    y++;
                    x = 0;
                }
                else
                {
                    x++;
                }
            }
        }

        public bool Verify()
        {
            var seatedGroups = FindAllSeatedGroups();

            foreach (var seatedGroup1 in seatedGroups)
            {
                (var x1, var y1) = seatedGroup1.Key;
                var s1 = seatedGroup1.Value;

                foreach (var seatedGroup2 in seatedGroups)
                {
                    (var x2, var y2) = seatedGroup2.Key;
                    var s2 = seatedGroup2.Value;

                    if (!(x1 == x2 && y1 == y2))
                    {
                        var validationResult = AreTwoSeatedGroupsValid(x1, y1, x2, y2, s1, s2);

                        if (validationResult != SeatingResult.NoViolation)
                        {
                            return false;
                        }
                    }

                }
            }

            return true;
        }

        public bool AllGroupsSeated()
        {
            return GetNumberOfGroupsSeated() == GetNumberOfGroups();
         }

        public int GetNumberOfGroupsSeated()
        {
            return FindAllSeatedGroups().Count;
        }

        public int GetNumberOfPeopleSeated()
        {
            return FindAllSeatedGroups().Values.Sum();
        }

        public int GetNumberOfGroups()
        {
            return Groups.Values.Sum();
        }

        public int GetCapacity()
        {
            int res = 0;
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    res += SeatMatrix[i][j] == 1 ? 1 : 0;
            return res;
        }

        public int GetSize()
        {
            return Width * Height;
        }

        public int GetNumberOfPeople()
        {
            var value = 0;

            foreach (var group in Groups)
            {
                value += group.Key * group.Value;
            }

            return value;
        }

        public IEnumerable<int> ToGroupList()
        {
            var result = new List<int>();

            foreach (var group in Groups)
            {
                for (int i = 0; i < group.Value; i++)
                {
                    result.Add(group.Key);
                }
            }

            return result;
        }

        private Dictionary<(int, int), int> FindAllSeatedGroups()
        {
            var firstPerson = (-1, -1);
            var groupSize = 0;
            var seatedGroups = new Dictionary<(int, int), int>();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (SeatMatrix[x][y] == 2)
                    {
                        if (firstPerson == (-1, -1))
                        {
                            firstPerson = (x, y);
                            groupSize++;
                        }
                        else
                        {
                            groupSize++;
                        }
                    }
                    else
                    {
                        if (firstPerson != (-1, -1))
                        {
                            seatedGroups[firstPerson] = groupSize;
                            groupSize = 0;
                            firstPerson = (-1, -1);
                        }
                    }
                }

                if (firstPerson != (-1, -1))
                {
                    seatedGroups[firstPerson] = groupSize;
                    groupSize = 0;
                    firstPerson = (-1, -1);
                }
            }

            return seatedGroups;
        }

        private SeatingResult AreTwoSeatedGroupsValid(int x1, int y1, int x2, int y2, int s1, int s2)
        {
            if (x1 == x2 && y1 == y2)
            {
                return SeatingResult.HorizontalViolation;
            }

            if (y1 == y2)
            {
                if (x1 < x2)
                {
                    if (x2 - (x1 + (s1 - 1)) <= 2)
                    {
                        return SeatingResult.HorizontalViolation;
                    }
                }
                else if (x1 > x2)
                {
                    if (x1 - (x2 + (s2 - 1)) <= 2)
                    {
                        return SeatingResult.HorizontalViolation;
                    }
                }
            }
            else if (x1 == x2)
            {
                if (Math.Abs(y2 - y1) < 2)
                {
                    return SeatingResult.VerticalViolation;
                }
            }
            else if (Math.Abs(y1 - y2) == 1)
            {
                if (x1 < x2)
                {
                    if (x2 - (x1 + (s1 - 1)) <= 1)
                    {
                        return SeatingResult.DiagnolViolation;
                    }
                }
                else if (x1 > x2)
                {
                    if (x1 - (x2 + (s2 - 1)) <= 1)
                    {
                        return SeatingResult.DiagnolViolation;
                    }
                }
            }

            return SeatingResult.NoViolation;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    stringBuilder.Append(SeatMatrix[x][y]);
                }

                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine(string.Join(',', Groups.Values));

            return stringBuilder.ToString();
        }

        public IEnumerable<(int, int)> GetInvalidSeats(int x, int y, int factor = 0, bool excludeDiagnol = false)
        {
            var result = new List<(int, int)>();

            for (int x2 = (x - 2 - factor); x2 < (x + 3 + factor); x2++)
            {
                if (x2 >= 0 && x2 < Width)
                {
                    result.Add((x2, y));
                }
            }

            var start = (x - 1 - factor);
            var end = (x + 2 + factor);

            if (excludeDiagnol)
            {
                start = (x - factor);
                end = (x + 1 - factor);
            }

            for (int x2 = start; x2 < end; x2++)
            {
                if (x2 >= 0 && x2 < Width)
                {
                    var above = y + 1;

                    if (above >= 0 && above < Height)
                    {
                        result.Add((x2, above));
                    }

                    var below = y - 1;

                    if (below >= 0)
                    {
                        result.Add((x2, below));
                    }
                }
            }

            return result;
        }

        public IEnumerable<int> GetNumberOfSeatsPerRow()
        {
            var result = new int[Height];

            for (int i = 0; i < Height; i++)
            {
                var size = Width;

                if (SeatMatrix[0][i] == 0)
                {
                    size--;
                }

                if (SeatMatrix[Width - 1][i] == 0)
                {
                    size--;
                }

                result[i] = size;
            }

            return result;
        }

        //public IEnumerable<int> GetSeatsPerRow(int rowIndex)
        //{
        //    return SeatMatrix[][rowIndex];
        //}
    }
}
