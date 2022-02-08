using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaSeaterLogic.SeatingStrategies
{
    public abstract class Ordering
    {
        protected List<int> _list { get; set; }

        protected Ordering(IEnumerable<int> list)
        {
            _list = list.ToList();
        }

        public abstract bool GetNext(out int item);
        public abstract bool HasNext();
    }
}
