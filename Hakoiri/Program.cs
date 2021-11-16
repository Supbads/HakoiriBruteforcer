using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hakoiri
{
    class Program
    {
        static bool Solved = false;
        static int ditchedSolutions = 0;
        static HashSet<string> visitedBoards = new HashSet<string>(1000);
        static Dictionary<string, int> visitedBoardsOptimal = new Dictionary<string, int>();
        static Stack<Snapshot> SnapshotsStack = new Stack<Snapshot>(2000);
        static List<Snapshot> backtrackSolutions = new List<Snapshot>(500);
        static int solveInvocations = 0;

        static int currentBestSolution = int.MaxValue;

        static void Main()
        {
            var level = Levels.Level5;

            var firstSnapshot = new Snapshot(level);
            SnapshotsStack.Push(firstSnapshot);
            SolveStack();

            if (Solved)
            {
                Console.WriteLine("Solved");
                Console.WriteLine("ditched " + ditchedSolutions);

                PrepareSolutionsChain();
                Console.WriteLine("steps " + backtrackSolutions.Count);
                //PrintAllRefSolutions();
                SaveRefSolutionsToTxt();

            }
            else
            {
                Console.WriteLine("No solution found");
            }

            Console.WriteLine("Solve Invocs: " + solveInvocations);
        }

        private static void SaveRefSolutionsToTxt()
        {
            Console.WriteLine("saving ref solutions steps");
            int step = 1;
            StringBuilder sb = new StringBuilder();
            foreach (var solution in backtrackSolutions)
            {
                sb.AppendLine("step " + step);
                sb.AppendLine(StrSnapshot(solution));
                sb.AppendLine();
                step++;
            }

            File.WriteAllText($"{Directory.GetCurrentDirectory()}\\..\\..\\..\\solution.txt", sb.ToString());
        }

        private static void PrintAllRefSolutions()
        {
            Console.WriteLine("printing ref solutions steps");
            int step = 1;
            foreach (var solution in backtrackSolutions)
            {
                Console.WriteLine("step " + step);
                PrintBoard(solution);
                Console.WriteLine();
                step++;
            }
        }

        private static void PrepareSolutionsChain()
        {
            var thingie = backtrackSolutions.FirstOrDefault();
            while (thingie.refSnap != null)
            {
                backtrackSolutions.Add(thingie.refSnap);
                thingie = thingie.refSnap;
            }
            backtrackSolutions.Reverse();
        }

        public static void SolveStack()
        {
            while (SnapshotsStack.Count != 0)
            {
                if(SnapshotsStack.Count == 1)
                {
                    Console.WriteLine("tuke");
                }
                var snapshot = SnapshotsStack.Pop();
                FindMostOptimalSolution(snapshot);
                //Solve(snapshot);
            }
        }

        public static void FindMostOptimalSolution(Snapshot snapshot)
        {
            if (snapshot.IsSolved())
            {
                if(snapshot.Step < currentBestSolution)
                {
                    currentBestSolution = snapshot.Step;
                    backtrackSolutions.Clear();
                    backtrackSolutions.Add(snapshot);
                    Solved = true;
                }

                //PrintBoard(snapshot);
                return;
            }

            solveInvocations++;
            if (snapshot.Step >= currentBestSolution)
            {
                ditchedSolutions++;
                snapshot.refSnap = null;
                return;
            }

            var hash = snapshot.GetSnapshotHash();
            if (visitedBoardsOptimal.ContainsKey(hash) && visitedBoardsOptimal[hash] <= snapshot.Step)
            {
                ditchedSolutions++;
                snapshot.refSnap = null;
                return;
            }

            if (!visitedBoardsOptimal.ContainsKey(hash))
            {
                visitedBoardsOptimal.Add(hash, snapshot.Step);
            }
            else
            {
                visitedBoardsOptimal[hash] = snapshot.Step;
            }
            

            if (visitedBoardsOptimal.Count % 1000 == 0)
            {
                Console.WriteLine($"visited Boards {visitedBoardsOptimal.Count}");
            }

            do
            {
                var cell = snapshot.GetCell();
                var shape = Utils.GetShape(cell);
                (bool possible, Snapshot snapshot) pos;
                switch (shape)
                {//try move down, right, left, up

                    case Utils.Shape.Empty:
                        continue;
                    case Utils.Shape.Square:
                        if (!snapshot.IsTopLeftSquare())
                            continue;

                        
                        pos = TryMoveRedRight(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveRedUp(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveRedLeft(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveRedDown(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        break;
                    case Utils.Shape.Vertical:
                        if (!snapshot.IsTopmostVertical())
                        {
                            continue;
                        }

                        pos = TryMoveVerticalLeft(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveVerticalRight(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveVerticalUp(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveVerticalDown(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        break;
                    case Utils.Shape.Horizontal:
                        if (!snapshot.IsLeftmostHorizontal())
                        {
                            continue;
                        }

                        pos = TryMoveHorizontalUp(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveHorizontalLeft(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveHorizontalRight(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveHorizontalDown(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        break;

                    case Utils.Shape.Single:
                        pos = TryMoveSingleUp(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveSingleLeft(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveSingleRight(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveSingleDown(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        break;
                    default:
                        break;
                }

            } while (snapshot.Increment());
            //while (!Solved && snapshot.Increment());
        }

        public static void Solve(Snapshot snapshot)
        {
            solveInvocations++;
            var hash = snapshot.GetSnapshotHash();
            if (visitedBoards.Contains(hash))
            {
                ditchedSolutions++;
                snapshot.refSnap = null;
                return;
            }

            visitedBoards.Add(hash);

            if (visitedBoards.Count % 50 == 0)
            {
                Console.WriteLine($"visited Boards {visitedBoards.Count}");
            }

            if (snapshot.IsSolved())
            {
                PrintBoard(snapshot);
                backtrackSolutions.Add(snapshot);
                Solved = true;
                return;
            }

            do
            {
                var cell = snapshot.GetCell();
                var shape = Utils.GetShape(cell);
                (bool possible, Snapshot snapshot) pos;
                switch (shape)
                {//try move down, right, left, up

                    case Utils.Shape.Empty:
                        continue;
                    case Utils.Shape.Square:
                        if (!snapshot.IsTopLeftSquare())
                            continue;

                        pos = TryMoveRedDown(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveRedRight(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);


                        pos = TryMoveRedLeft(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveRedUp(snapshot);
                        if (pos.possible)
                        {
                            SnapshotsStack.Push(pos.snapshot);
                        }

                        break;

                    case Utils.Shape.Vertical:
                        if (!snapshot.IsTopmostVertical())
                        {
                            continue;
                        }

                        pos = TryMoveVerticalLeft(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveVerticalRight(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveVerticalUp(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveVerticalDown(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        break;
                    case Utils.Shape.Horizontal:
                        if (!snapshot.IsLeftmostHorizontal())
                        {
                            continue;
                        }

                        pos = TryMoveHorizontalUp(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveHorizontalLeft(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveHorizontalRight(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveHorizontalDown(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        break;

                    case Utils.Shape.Single:
                        pos = TryMoveSingleUp(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveSingleLeft(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveSingleRight(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        pos = TryMoveSingleDown(snapshot);
                        if (pos.possible)
                            SnapshotsStack.Push(pos.snapshot);

                        break;
                    default:
                        break;
                }

            } while (!Solved && snapshot.Increment());
        }

        #region blueMovement
        public static (bool, Snapshot) TryMoveSingleUp(Snapshot board)
        {
            if (board.Row > 0 && board.BoardSnapshot[board.Row - 1, board.Col] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row - 1, board.Col] = 1;
                newSnapshot.BoardSnapshot[board.Row, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveSingleLeft(Snapshot board)
        {
            if (board.Col > 0 && board.BoardSnapshot[board.Row, board.Col - 1] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row, board.Col - 1] = 1;
                newSnapshot.BoardSnapshot[board.Row, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveSingleRight(Snapshot board)
        {
            if (board.Col < Utils.MaxCol && board.BoardSnapshot[board.Row, board.Col + 1] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row, board.Col + 1] = 1;
                newSnapshot.BoardSnapshot[board.Row, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveSingleDown(Snapshot board)
        {
            if (board.Row < Utils.MaxRow && board.BoardSnapshot[board.Row + 1, board.Col] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row + 1, board.Col] = 1;
                newSnapshot.BoardSnapshot[board.Row, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        #endregion

        #region redMovement
        //we always assume that we're at top left of square and its position is valid
        public static (bool, Snapshot) TryMoveRedDown(Snapshot board)
        {
            if (board.Row + 1 < Utils.MaxRow && board.BoardSnapshot[board.Row + 2, board.Col] == 0 && board.BoardSnapshot[board.Row + 2, board.Col + 1] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row + 2, board.Col] = Utils.Red;
                newSnapshot.BoardSnapshot[board.Row + 2, board.Col + 1] = Utils.Red;

                newSnapshot.BoardSnapshot[board.Row, board.Col] = 0;
                newSnapshot.BoardSnapshot[board.Row, board.Col + 1] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveRedRight(Snapshot board)
        {
            if (board.Col + 1 < Utils.MaxCol && board.BoardSnapshot[board.Row, board.Col + 2] == 0 && board.BoardSnapshot[board.Row + 1, board.Col + 2] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row, board.Col + 2] = Utils.Red;
                newSnapshot.BoardSnapshot[board.Row + 1, board.Col + 2] = Utils.Red;

                newSnapshot.BoardSnapshot[board.Row, board.Col] = 0;
                newSnapshot.BoardSnapshot[board.Row + 1, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveRedLeft(Snapshot board)
        {
            if (board.Col > 0 && board.BoardSnapshot[board.Row, board.Col - 1] == 0 && board.BoardSnapshot[board.Row + 1, board.Col - 1] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row, board.Col - 1] = Utils.Red;
                newSnapshot.BoardSnapshot[board.Row + 1, board.Col - 1] = Utils.Red;

                newSnapshot.BoardSnapshot[board.Row, board.Col + 1] = 0;
                newSnapshot.BoardSnapshot[board.Row + 1, board.Col + 1] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveRedUp(Snapshot board)
        {
            if (board.Row > 0 && board.BoardSnapshot[board.Row - 1, board.Col] == 0 && board.BoardSnapshot[board.Row - 1, board.Col + 1] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row - 1, board.Col] = Utils.Red;
                newSnapshot.BoardSnapshot[board.Row - 1, board.Col + 1] = Utils.Red;

                newSnapshot.BoardSnapshot[board.Row + 1, board.Col] = 0;
                newSnapshot.BoardSnapshot[board.Row + 1, board.Col + 1] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        #endregion

        #region horizontalMovement
        public static (bool, Snapshot) TryMoveHorizontalUp(Snapshot board)
        {
            if (board.Row > 0 && board.BoardSnapshot[board.Row - 1, board.Col] == 0 && board.BoardSnapshot[board.Row - 1, board.Col + 1] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row - 1, board.Col] = current;
                newSnapshot.BoardSnapshot[board.Row - 1, board.Col + 1] = current;

                newSnapshot.BoardSnapshot[board.Row, board.Col] = 0;
                newSnapshot.BoardSnapshot[board.Row, board.Col + 1] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveHorizontalDown(Snapshot board)
        {
            if (board.Row < Utils.MaxRow && board.BoardSnapshot[board.Row + 1, board.Col] == 0 && board.BoardSnapshot[board.Row + 1, board.Col + 1] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row + 1, board.Col] = current;
                newSnapshot.BoardSnapshot[board.Row + 1, board.Col + 1] = current;

                newSnapshot.BoardSnapshot[board.Row, board.Col] = 0;
                newSnapshot.BoardSnapshot[board.Row, board.Col + 1] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveHorizontalLeft(Snapshot board)
        {
            if (board.Col > 0 && board.BoardSnapshot[board.Row, board.Col - 1] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row, board.Col - 1] = current;

                newSnapshot.BoardSnapshot[board.Row, board.Col + 1] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveHorizontalRight(Snapshot board)
        {
            if (board.Col + 1 < Utils.MaxCol && board.BoardSnapshot[board.Row, board.Col + 2] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row, board.Col + 2] = current;

                newSnapshot.BoardSnapshot[board.Row, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        #endregion

        #region verticalMovement
        public static (bool, Snapshot) TryMoveVerticalLeft(Snapshot board)
        {
            if (board.Col > 0 && board.BoardSnapshot[board.Row, board.Col - 1] == 0 && board.BoardSnapshot[board.Row + 1, board.Col - 1] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row, board.Col - 1] = current;
                newSnapshot.BoardSnapshot[board.Row + 1, board.Col - 1] = current;

                newSnapshot.BoardSnapshot[board.Row, board.Col] = 0;
                newSnapshot.BoardSnapshot[board.Row + 1, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveVerticalRight(Snapshot board)
        {
            if (board.Col < Utils.MaxCol && board.BoardSnapshot[board.Row, board.Col + 1] == 0 && board.BoardSnapshot[board.Row + 1, board.Col + 1] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row, board.Col + 1] = current;
                newSnapshot.BoardSnapshot[board.Row + 1, board.Col + 1] = current;

                newSnapshot.BoardSnapshot[board.Row, board.Col] = 0;
                newSnapshot.BoardSnapshot[board.Row + 1, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveVerticalUp(Snapshot board)
        {
            if (board.Row > 0 && board.BoardSnapshot[board.Row - 1, board.Col] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row - 1, board.Col] = current;

                newSnapshot.BoardSnapshot[board.Row + 1, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveVerticalDown(Snapshot board)
        {
            if (board.Row + 1 < Utils.MaxRow && board.BoardSnapshot[board.Row + 2, board.Col] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.BoardSnapshot[board.Row + 2, board.Col] = current;

                newSnapshot.BoardSnapshot[board.Row, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }
        #endregion

        public static void PrintBoard(Snapshot snapshot)
        {
            for (int i = 0; i <= Utils.MaxRow; i++)
            {
                for (int j = 0; j <= Utils.MaxCol; j++)
                {
                    Console.Write(snapshot.BoardSnapshot[i, j].ToString().PadRight(3));
                }
                Console.WriteLine();
            }
        }

        public static string StrSnapshot(Snapshot snapshot)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i <= Utils.MaxRow; i++)
            {
                for (int j = 0; j <= Utils.MaxCol; j++)
                {
                    sb.Append(snapshot.BoardSnapshot[i, j].ToString().PadRight(3));
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

    }
}
