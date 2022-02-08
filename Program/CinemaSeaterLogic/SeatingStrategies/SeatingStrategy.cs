using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaSeaterLogic.SeatingStrategies
{
    public abstract class SeatingStrategy
    {
        protected List<int> Groups { get; set; }

        protected SeatingStrategy(List<int> groups)
        {
            Groups = groups;
        }

        public abstract bool GetNextGroup(out int groupSize, int maxSize);
        public abstract bool HasNextGroup();
    }
}
