using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaSeaterLogic.SeatingStrategies
{
    public class Random : Ordering
    {
        private readonly System.Random _rnd;

        public Random(IEnumerable<int> list, System.Random rnd) : base(list)
        {
            _rnd = rnd;
        }

        public override bool GetNext(out int item)
        {
            item = -1;

            if (!HasNext())
            {
                return false;
            }

            int r = _rnd.Next(_list.Count());
            item = _list.ElementAt(r);
            _list.Remove(item);

            return true;
        }

        public override bool HasNext()
        {
            return _list.Any();
        }
    }
}
