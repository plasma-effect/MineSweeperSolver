using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SolverCore.Utility;

namespace SolverCore
{
    public class NormalSolver
    {
        protected enum BombState
        {
            Unknown,
            Bomb,
            None
        }
        
        protected int?[,] field;
        protected BombState[,] state;
        protected int n, m;
        protected int k;
        protected RoundIterate round;
        protected Random random;
        protected int depth;

        public NormalSolver(int n, int m, int k, int depth)
        {
            this.n = n;
            this.m = m;
            this.field = new int?[n, m];
            this.state = new BombState[this.n, this.m];
            this.k = k;
            this.round = new RoundIterate(0, 0, this.n, this.m);
            this.random = new Random();
            this.depth = depth;
        }

        private IEnumerable<bool> NextState(int i, int j, int roundsize, BombState[,] state, List<(int i, int j)> list)
        {
            foreach(var f in Enumerable.Range(0, 1 << list.Count))
            {
                if (f.BitFlag() != roundsize)
                {
                    continue;
                }
                foreach(var index in Enumerable.Range(0, list.Count))
                {
                    var (a, b) = list[index];
                    if ((f & (1 << index)) != 0)
                    {
                        state[a, b] = BombState.Bomb;
                    }
                    else
                    {
                        state[a, b] = BombState.None;
                    }
                }
                yield return true;
            }
            foreach (var (a, b) in list) 
            {
                state[a, b] = BombState.Unknown;
            }
        }

        private bool UpdateImpl(int dep, int pi, int pj, Stack<(int i, int j)> stack, BombState[,] state)
        {
            if (dep == 0)
            {
                return true;
            }
            if (this.field[pi, pj] is int size)
            {
                var list = new List<(int i, int j)>();
                foreach (var (i, j) in this.round.MakeRoundIterator(pi, pj))
                {
                    if (state[i, j] == BombState.Bomb)
                    {
                        --size;
                    }
                    else if (state[i, j] == BombState.Unknown)
                    {
                        list.Add((i, j));
                    }
                }
                if (size < 0)
                {
                    return false;
                }
                if (list.Count == 0)
                {
                    return true;
                }
                stack.Push((pi, pj));

                var ret = false;
                foreach (var _ in NextState(pi, pj, size, state, list))
                {
                    var flag = true;
                    foreach (var (i, j) in this.round.MakeRoundIterator(pi, pj).Where(a => !stack.Contains(a)))  
                    {
                        if (!UpdateImpl(dep - 1, i, j, stack, state))
                        {
                            flag = false;
                            break;
                        }
                    }
                    ret |= flag;
                }
                stack.Pop();
                return ret;
            }
            return true;
        }

        protected void Update()
        {
            var memo = this.state;
            var count = -1;
            while (count != 0)
            {
                count = 0;
                foreach (var (pi, pj) in Utility.SquareRange(0, 0, this.n, this.m))
                {
                    if (this.field[pi, pj] is int val)
                    {
                        var exist = new bool[3, 3];
                        var notexist = new bool[3, 3];
                        var stack = new Stack<(int i, int j)>();
                        var list = new List<(int i, int j)>();
                        stack.Push((pi, pj));
                        foreach (var (i, j) in this.round.MakeRoundIterator(pi, pj))
                        {
                            if (memo[i, j] == BombState.Bomb)
                            {
                                --val;
                            }
                            else if (memo[i, j] == BombState.Unknown)
                            {
                                list.Add((i, j));
                            }
                        }
                        if (val == 0)
                        {
                            foreach (var (i, j) in this.round.MakeRoundIterator(pi, pj))
                            {
                                if (memo[i, j] == BombState.Unknown)
                                {
                                    memo[i, j] = BombState.None;
                                }
                            }
                            continue;
                        }
                        else if (list.Count == 0)
                        {
                            continue;
                        }
                        foreach (var _ in NextState(pi, pj, val, memo, list))
                        {
                            var f = true;
                            foreach (var (i, j) in this.round.MakeRoundIterator(pi, pj))
                            {
                                f &= UpdateImpl(depth - 1, i, j, stack, memo);
                            }
                            if (f)
                            {
                                foreach (var (i, j) in this.round.MakeRoundIterator(pi, pj))
                                {
                                    exist[i - (pi - 1), j - (pj - 1)] |= memo[i, j] == BombState.Bomb;
                                    notexist[i - (pi - 1), j - (pj - 1)] |= memo[i, j] == BombState.None;
                                }
                            }
                        }
                        foreach (var (i, j) in this.round.MakeRoundIterator(pi, pj))
                        {
                            if (memo[i, j] == BombState.Unknown)
                            {
                                if (!exist[i - (pi - 1), j - (pj - 1)])
                                {
                                    memo[i, j] = BombState.None;
                                    ++count;
                                }
                                else if (!notexist[i - (pi - 1), j - (pj - 1)]) 
                                {
                                    memo[i, j] = BombState.Bomb;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Input(int i, int j, int num)
        {
            this.field[i, j] = num;
            this.state[i, j] = BombState.None;
            Update();
        }

        public (int i, int j) Open()
        {
            var list = new List<(int i, int j)>();
            foreach (var (i, j) in Utility.SquareRange(0, 0, this.n, this.m)) 
            {
                if (!this.field[i, j].HasValue)
                {
                    if (this.state[i, j] == BombState.None)
                    {
                        return (i, j);
                    }
                    else if (this.state[i, j] == BombState.Unknown)
                    {
                        list.Add((i, j));
                    }
                }
            }
            if (list.Count == 0)
            {
                throw new Exception("開くことができるマスがありません");
            }
            var (a, b) = list[this.random.Next(list.Count)];
            return (a, b);
        }

        public void OutputState(System.IO.TextWriter writer)
        {
            foreach(var i in Enumerable.Range(0, this.n))
            {
                foreach(var j in Enumerable.Range(0, this.m))
                {
                    writer.Write(this.state[i, j] == BombState.Bomb ? "x" : this.state[i, j] == BombState.None ? "o" : "?");
                }
                foreach (var j in Enumerable.Range(0, this.m))
                {
                    writer.Write("[{0}]", this.field[i, j]?.ToString() ?? " ");
                }
                writer.WriteLine();
            }
        }
    }
}
