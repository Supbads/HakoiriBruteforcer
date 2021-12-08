namespace Hakoiri
{
    public static class MovementAlgorithm
    {
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
