using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Hakoiri
{
    class Program
    {
        const bool UseOptimalSolutionConfig = true;

        static bool Solved = false;
        static int skippedSolutions = 0;
        static int solveInvocations = 0;
        static int currentBestSolution = int.MaxValue;
        static HashSet<string> visitedBoards = new HashSet<string>(1000);
        static List<Snapshot> backtrackSolutions = new List<Snapshot>(500);

        // should be 20k+ to avoid resizing
        static Dictionary<string, int> visitedBoardsOptimal = new Dictionary<string, int>(20000);

        // using a Stack instead of a Queue kills the performance of the alogirthm
        static Queue<Snapshot> SavedSnapshots = new Queue<Snapshot>(2000);


        static void Main()
        {
            Action<Snapshot> solveDelegate;

            if (UseOptimalSolutionConfig)
            {
                solveDelegate = FindMostOptimalSolution;
            }
            else
            {
                solveDelegate = Solve;
            }

            var looger = new SnapshotLogger();

            PrepareInitialBoardState(Levels.Level7);
            var sw = Stopwatch.StartNew();

            StartSolvingSoltuions(solveDelegate);

            if (Solved)
            {
                Console.WriteLine("Solved");
                Console.WriteLine("Skipped " + skippedSolutions);

                PrepareSolutionsChain();
                Console.WriteLine("steps " + backtrackSolutions.Count);
                looger.SaveRefSolutionsToTxt(backtrackSolutions);
                //looger.PrintAllRefSolutions();
            }
            else
            {
                Console.WriteLine("No solution found");
            }

            var elapsedMs = sw.ElapsedMilliseconds;
            Console.WriteLine($"Elapsed miliseconds: {elapsedMs}");
            Console.WriteLine("Solve Invocs: " + solveInvocations);
        }

        private static void PrepareInitialBoardState(byte[,] level)
        {
            var firstSnapshot = new Snapshot(level);
            SavedSnapshots.Enqueue(firstSnapshot);
        }

        private static void PrepareSolutionsChain()
        {
            var currentSnapshot = backtrackSolutions.FirstOrDefault();
            while (currentSnapshot.refSnap != null)
            {
                backtrackSolutions.Add(currentSnapshot.refSnap);
                currentSnapshot = currentSnapshot.refSnap;
            }

            backtrackSolutions.Reverse();
        }

        public static void StartSolvingSoltuions(Action<Snapshot> Solve)
        {
            while (SavedSnapshots.Count != 0)
            {
                var snapshot = SavedSnapshots.Dequeue();
                Solve(snapshot);
            }
        }

        public static void FindMostOptimalSolution(Snapshot snapshot)
        {
            solveInvocations++;
            if (snapshot.IsSolved())
            {
                if (snapshot.Step < currentBestSolution)
                {
                    currentBestSolution = snapshot.Step;
                    backtrackSolutions.Clear();
                    backtrackSolutions.Add(snapshot);
                    Solved = true;
                }

                return;
            }

            if (snapshot.Step >= currentBestSolution)
            {
                skippedSolutions++;
                snapshot.refSnap = null;
                return;
            }

            var positionHash = snapshot.GetSnapshotHash();
            if (visitedBoardsOptimal.ContainsKey(positionHash) && visitedBoardsOptimal[positionHash] <= snapshot.Step)
            {
                skippedSolutions++;
                snapshot.refSnap = null;
                return;
            }

            if (!visitedBoardsOptimal.ContainsKey(positionHash))
            {
                visitedBoardsOptimal.Add(positionHash, snapshot.Step);
            }
            else
            {
                visitedBoardsOptimal[positionHash] = snapshot.Step;
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
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveRedUp(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveRedLeft(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveRedDown(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        break;
                    case Utils.Shape.Vertical:
                        if (!snapshot.IsTopmostVertical())
                        {
                            continue;
                        }

                        pos = TryMoveVerticalLeft(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveVerticalRight(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveVerticalUp(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveVerticalDown(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        break;
                    case Utils.Shape.Horizontal:
                        if (!snapshot.IsLeftmostHorizontal())
                        {
                            continue;
                        }

                        pos = TryMoveHorizontalUp(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveHorizontalLeft(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveHorizontalRight(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveHorizontalDown(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        break;

                    case Utils.Shape.Single:
                        pos = TryMoveSingleUp(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveSingleLeft(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveSingleRight(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveSingleDown(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        break;
                    default:
                        break;
                }

            } while (snapshot.Increment());
        }

        public static void Solve(Snapshot snapshot)
        {
            solveInvocations++;
            var hash = snapshot.GetSnapshotHash();
            if (visitedBoards.Contains(hash))
            {
                skippedSolutions++;
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
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveRedRight(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);


                        pos = TryMoveRedLeft(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveRedUp(snapshot);
                        if (pos.possible)
                        {
                            SavedSnapshots.Enqueue(pos.snapshot);
                        }

                        break;

                    case Utils.Shape.Vertical:
                        if (!snapshot.IsTopmostVertical())
                        {
                            continue;
                        }

                        pos = TryMoveVerticalLeft(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveVerticalRight(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveVerticalUp(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveVerticalDown(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        break;
                    case Utils.Shape.Horizontal:
                        if (!snapshot.IsLeftmostHorizontal())
                        {
                            continue;
                        }

                        pos = TryMoveHorizontalUp(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveHorizontalLeft(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveHorizontalRight(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveHorizontalDown(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        break;

                    case Utils.Shape.Single:
                        pos = TryMoveSingleUp(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveSingleLeft(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveSingleRight(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        pos = TryMoveSingleDown(snapshot);
                        if (pos.possible)
                            SavedSnapshots.Enqueue(pos.snapshot);

                        break;
                    default:
                        break;
                }

            } while (!Solved && snapshot.Increment());
        }

        #region blueMovement
        public static (bool, Snapshot) TryMoveSingleUp(Snapshot board)
        {
            if (board.Row > 0 && board.Board[board.Row - 1, board.Col] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row - 1, board.Col] = 1;
                newSnapshot.Board[board.Row, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveSingleLeft(Snapshot board)
        {
            if (board.Col > 0 && board.Board[board.Row, board.Col - 1] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row, board.Col - 1] = 1;
                newSnapshot.Board[board.Row, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveSingleRight(Snapshot board)
        {
            if (board.Col < Utils.MaxCol && board.Board[board.Row, board.Col + 1] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row, board.Col + 1] = 1;
                newSnapshot.Board[board.Row, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveSingleDown(Snapshot board)
        {
            if (board.Row < Utils.MaxRow && board.Board[board.Row + 1, board.Col] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row + 1, board.Col] = 1;
                newSnapshot.Board[board.Row, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        #endregion

        #region redMovement
        //we always assume that we're at top left of square and its position is valid
        public static (bool, Snapshot) TryMoveRedDown(Snapshot board)
        {
            if (board.Row + 1 < Utils.MaxRow && board.Board[board.Row + 2, board.Col] == 0 && board.Board[board.Row + 2, board.Col + 1] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row + 2, board.Col] = Utils.Red;
                newSnapshot.Board[board.Row + 2, board.Col + 1] = Utils.Red;

                newSnapshot.Board[board.Row, board.Col] = 0;
                newSnapshot.Board[board.Row, board.Col + 1] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveRedRight(Snapshot board)
        {
            if (board.Col + 1 < Utils.MaxCol && board.Board[board.Row, board.Col + 2] == 0 && board.Board[board.Row + 1, board.Col + 2] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row, board.Col + 2] = Utils.Red;
                newSnapshot.Board[board.Row + 1, board.Col + 2] = Utils.Red;

                newSnapshot.Board[board.Row, board.Col] = 0;
                newSnapshot.Board[board.Row + 1, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveRedLeft(Snapshot board)
        {
            if (board.Col > 0 && board.Board[board.Row, board.Col - 1] == 0 && board.Board[board.Row + 1, board.Col - 1] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row, board.Col - 1] = Utils.Red;
                newSnapshot.Board[board.Row + 1, board.Col - 1] = Utils.Red;

                newSnapshot.Board[board.Row, board.Col + 1] = 0;
                newSnapshot.Board[board.Row + 1, board.Col + 1] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveRedUp(Snapshot board)
        {
            if (board.Row > 0 && board.Board[board.Row - 1, board.Col] == 0 && board.Board[board.Row - 1, board.Col + 1] == 0)
            {
                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row - 1, board.Col] = Utils.Red;
                newSnapshot.Board[board.Row - 1, board.Col + 1] = Utils.Red;

                newSnapshot.Board[board.Row + 1, board.Col] = 0;
                newSnapshot.Board[board.Row + 1, board.Col + 1] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        #endregion

        #region horizontalMovement
        //we always assume that we're at left of horizontal piece and its position is valid
        public static (bool, Snapshot) TryMoveHorizontalUp(Snapshot board)
        {
            if (board.Row > 0 && board.Board[board.Row - 1, board.Col] == 0 && board.Board[board.Row - 1, board.Col + 1] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row - 1, board.Col] = current;
                newSnapshot.Board[board.Row - 1, board.Col + 1] = current;

                newSnapshot.Board[board.Row, board.Col] = 0;
                newSnapshot.Board[board.Row, board.Col + 1] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveHorizontalDown(Snapshot board)
        {
            if (board.Row < Utils.MaxRow && board.Board[board.Row + 1, board.Col] == 0 && board.Board[board.Row + 1, board.Col + 1] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row + 1, board.Col] = current;
                newSnapshot.Board[board.Row + 1, board.Col + 1] = current;

                newSnapshot.Board[board.Row, board.Col] = 0;
                newSnapshot.Board[board.Row, board.Col + 1] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveHorizontalLeft(Snapshot board)
        {
            if (board.Col > 0 && board.Board[board.Row, board.Col - 1] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row, board.Col - 1] = current;

                newSnapshot.Board[board.Row, board.Col + 1] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveHorizontalRight(Snapshot board)
        {
            if (board.Col + 1 < Utils.MaxCol && board.Board[board.Row, board.Col + 2] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row, board.Col + 2] = current;

                newSnapshot.Board[board.Row, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        #endregion

        #region verticalMovement
        //we always assume that we're at top of vertical piece and its position is valid
        public static (bool, Snapshot) TryMoveVerticalLeft(Snapshot board)
        {
            if (board.Col > 0 && board.Board[board.Row, board.Col - 1] == 0 && board.Board[board.Row + 1, board.Col - 1] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row, board.Col - 1] = current;
                newSnapshot.Board[board.Row + 1, board.Col - 1] = current;

                newSnapshot.Board[board.Row, board.Col] = 0;
                newSnapshot.Board[board.Row + 1, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveVerticalRight(Snapshot board)
        {
            if (board.Col < Utils.MaxCol && board.Board[board.Row, board.Col + 1] == 0 && board.Board[board.Row + 1, board.Col + 1] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row, board.Col + 1] = current;
                newSnapshot.Board[board.Row + 1, board.Col + 1] = current;

                newSnapshot.Board[board.Row, board.Col] = 0;
                newSnapshot.Board[board.Row + 1, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveVerticalUp(Snapshot board)
        {
            if (board.Row > 0 && board.Board[board.Row - 1, board.Col] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row - 1, board.Col] = current;

                newSnapshot.Board[board.Row + 1, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }

        public static (bool, Snapshot) TryMoveVerticalDown(Snapshot board)
        {
            if (board.Row + 1 < Utils.MaxRow && board.Board[board.Row + 2, board.Col] == 0)
            {
                var current = board.GetCell();

                var newSnapshot = new Snapshot(board);
                newSnapshot.Board[board.Row + 2, board.Col] = current;

                newSnapshot.Board[board.Row, board.Col] = 0;
                return (true, newSnapshot);
            }

            return (false, null);
        }
        #endregion

    }
}
