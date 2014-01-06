using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication4
{ 
    static class Program
    {
		static IEnumerable<T> MergePreserveOrder4OP<T, TOrder>(this IEnumerable<IEnumerable<T>> aa, Func<T, TOrder> orderFunc) where TOrder : IComparable<TOrder>
        {
            var items = aa.Select(xx => xx.GetEnumerator()).Where(ee => ee.MoveNext()).OrderBy(ee => ee.Current).ToList();

            while (items.Count > 0)
            {
                yield return items[0].Current;

                var head = items[0];
                items.RemoveAt(0);
                if (head.MoveNext())
                { 
                    var finalPosition = 0;
                    while (orderFunc(head.Current).CompareTo(orderFunc(items[finalPosition].Current)) > 0 && items.Count-1 > finalPosition++) ;
                    items.Insert(finalPosition, head);
                }
            }
        }

		
        static IEnumerable<T> MergePreserveOrder4<T, TOrder>(this IEnumerable<IEnumerable<T>> aa, Func<T, TOrder> orderFunc) where TOrder : IComparable<TOrder>
        {
            var items = aa.Select(xx => xx.GetEnumerator())
                          .Where(ee => ee.MoveNext())
                          .Select(ee => Tuple.Create(orderFunc(ee.Current), ee))
                          .OrderBy(ee => ee.Item1).ToList();

            while (items.Count > 0)
            {
                yield return items[0].Item2.Current;

                var next = items[0];
                items.RemoveAt(0);
                if (next.Item2.MoveNext())
                {
                    var value = orderFunc(next.Item2.Current);
                    var ii = 0;
                    for (; ii < items.Count; ++ii)
                    {
                        if (value.CompareTo(items[ii].Item1) <= 0)
                        {   // NB: using a tuple to minimize calls to orderFunc
                            items.Insert(ii, Tuple.Create(value, next.Item2));
                            break;
                        }
                    }

                    if (ii == items.Count) items.Add(Tuple.Create(value, next.Item2));
                }
                else next.Item2.Dispose(); // woops! can't forget IDisposable
            }
        }

        static void Main(string[] args)
        {
            IEnumerable<int> line1 = new List<int>(){1,7,8,9,56,93};
            IEnumerable<int> line2 = new List<int>() {2,29,36,78,91};
            IEnumerable<int> line3 = new List<int>() { 29, 36, 78, 91 };

            IEnumerable<IEnumerable<int>> l = new List<IEnumerable<int>>{line1,line2,line3};

            foreach (var v in l.MergePreserveOrder4(x=>x))
            {
                Console.WriteLine(v);
            }

            int useless = Console.Read();
        }
    }
}



