using System.Collections.Generic;
using System.Linq;

namespace CinemaSeaterLogic.SeatingStrategies
{
    public class BestFitStrategy : SeatingStrategy
    {
        public BestFitStrategy(IEnumerable<int> groups) : base(groups.ToList()) {}

        public override bool GetNextGroup(out int groupSize, int maxSize)
        {
            groupSize = maxSize;

            while (!Groups.Contains(groupSize))
            {
                groupSize--;

                if (groupSize < 1)
                {
                    return false;
                }
            }

            Groups.Remove(groupSize);
            return true;
        }

        public override bool HasNextGroup()
        {
            return Groups.Any();
        }
    }
}
