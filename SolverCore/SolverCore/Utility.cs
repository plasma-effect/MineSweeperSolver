using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolverCore
{
    public static class Utility
    {
        public static IEnumerable<(int i, int j)> SquareRange(int xs, int ys, int xc, int yc)
        {
            foreach(var x in Enumerable.Range(xs, xc))
            {
                foreach(var y in Enumerable.Range(ys, yc))
                {
                    yield return (x, y);
                }
            }
        }

        public static int BitFlag(this int v)
        {
            var c = 0;
            foreach(var i in Enumerable.Range(0, 32))
            {
                if (((1 << i) & v) != 0) 
                {
                    ++c;
                }
            }
            return c;
        }

        public class RoundIterate
        {
            int xmin, ymin;
            int xmax, ymax;

            public RoundIterate(int xmin, int ymin, int xmax, int ymax)
            {
                this.xmin = xmin;
                this.ymin = ymin;
                this.xmax = xmax;
                this.ymax = ymax;
            }

            public IEnumerable<(int i,int j)> MakeRoundIterator(int pi,int pj)
            {
                var xs = Math.Max(this.xmin, pi - 1);
                var xe = Math.Min(this.xmax, pi + 2);
                var ys = Math.Max(this.ymin, pj - 1);
                var ye = Math.Min(this.ymax, pj + 2);
                for(var i = xs; i < xe; ++i)
                {
                    for(var j = ys; j < ye; ++j)
                    {
                        yield return (i, j);
                    }
                }
            }
        }

        public static void DebugMessage(string message)
        {
            Console.Error.WriteLine(message);
        }

        public static void DebugMessage(string message, object obj)
        {
            Console.Error.WriteLine(message, obj);
        }

        public static void DebugMessage(string message, object obj1, object obj2)
        {
            Console.Error.WriteLine(message, obj1, obj2);
        }

        public static void DebugMessage(string message, object obj1, object obj2, object obj3)
        {
            Console.Error.WriteLine(message, obj1, obj2, obj3);
        }

        public static void DebugMessage(string message, object obj1, object obj2, object obj3, object obj4)
        {
            Console.Error.WriteLine(message, obj1, obj2, obj3, obj4);
        }

        public static void DebugMessage(object obj)
        {
            Console.Error.WriteLine(obj);
        }
    }
}
