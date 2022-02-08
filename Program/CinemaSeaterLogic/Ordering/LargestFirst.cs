using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaSeaterLogic.SeatingStrategies
{
    public class LargestFirst : Ordering
    {
        public LargestFirst(IEnumerable<int> list) : base(list) { }

        public override bool GetNext(out int item)
        {
            item = -1;

            if (!HasNext())
            {
                return false;
            }

            item = _list.Max();
            _list.Remove(item);

            return true;
        }

        public override bool HasNext()
        {
            return _list.Any();
        }
    }
}
