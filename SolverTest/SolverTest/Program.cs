using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using static System.Math;

namespace SolverTest
{
    class Program
    {
        static bool Run(int n, int m, int k, int si, int sj, int depth, Random random)
        {
            var bombs = new bool[n, m];
            var field = new int[n, m];
            var opened = new bool[n, m];
            var round = new SolverCore.Utility.RoundIterate(0, 0, n, m);
            foreach (var _ in Enumerable.Range(0, k))
            {
                while (true)
                {
                    var i = random.Next(n);
                    var j = random.Next(m);
                    if (bombs[i, j])
                    {
                        continue;
                    }
                    if (Max(Abs(i - si), Abs(j - sj)) <= 1)
                    {
                        continue;
                    }
                    bombs[i, j] = true;
                    foreach (var (a, b) in round.MakeRoundIterator(i, j))
                    {
                        ++field[a, b];
                    }
                    break;
                }
            }

            var solver = new SolverCore.NormalSolver(n, m, k, depth);

            var size = n * m - k;
            foreach (var (a, b) in round.MakeRoundIterator(si, sj))
            {
                opened[a, b] = true;
                solver.Input(a, b, field[a, b]);
                --size;
            }
            foreach (var _ in Enumerable.Range(0, size))
            {
                var (i, j) = solver.Open();
                if (bombs[i, j])
                {
                    return false;
                }
                opened[i, j] = true;
                solver.Input(i, j, field[i, j]);
            }
            return true;
        }

        static void Main(string[] args)
        {
            Error.WriteLine("サイズを入力");
            var s = ReadLine().Split(' ');
            var n = Max(6, int.Parse(s[0]));
            var m = Max(6, int.Parse(s[1]));
            var k = Max(4, Min(n * m - 9, int.Parse(s[2])));
            const int challenge = 10000;
            foreach (var depth in Enumerable.Range(2, 4))
            {
                var ranking = new List<(int clear, int i, int j)>();
                using (var stream = new System.IO.StreamWriter("result" + depth + ".txt"))
                {
                    foreach (var si in Enumerable.Range(0, n / 2 + n % 2))
                    {
                        foreach (var sj in Enumerable.Range(0, n == m ? si + 1 : m / 2 + m % 2)) 
                        {
                            var random = new Random();
                            var clear = 0;
                            foreach (var c in Enumerable.Range(1, challenge))
                            {
                                if (Run(n, m, k, si, sj, depth, random))
                                {
                                    ++clear;
                                }
                            }
                            ranking.Add((clear, si, sj));
                            Error.WriteLine("depth={3},({0},{1}):{2}%", si + 1, sj + 1, (double)clear * 100 / challenge, depth);
                        }
                    }
                    foreach (var (clear, i, j) in ranking.OrderByDescending(a => a.clear))
                    {
                        stream.WriteLine("({0},{1}):{2}%", i + 1, j + 1, (double)clear * 100 / challenge);
                    }
                }
            }
        }
    }
}
