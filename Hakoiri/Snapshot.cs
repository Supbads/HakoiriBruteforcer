using System.Text;

namespace Hakoiri
{
    public class Snapshot
    {
        public byte Row { get; set; }
        public byte Col { get; set; }
        public int Step { get; set; }

        public byte[,] Board;
        public Snapshot refSnap;
        public Snapshot(Snapshot snapshot)
        {
            Board = Utils.CopyBoard(snapshot.Board);
            this.refSnap = snapshot;
            Row = 0;
            Col = 0;
            Step = snapshot.Step + 1;
        }

        public Snapshot(byte[,] board)
        {
            Board = Utils.CopyBoard(board);
            Row = 0;
            Col = 0;
            Step = 1;
        }

        public bool Increment()
        {
            if (Row == Utils.MaxRow && Col == Utils.MaxCol)
            { // we are at the end
                return false;
            }

            if (Col == Utils.MaxCol)
            { //last column - fall one row down
                Col = 0;
                Row += 1;
            }
            else
            {
                Col += 1;
            }

            return true;
        }

        public byte GetCell()
        {
            return Board[Row, Col];
        }

        public bool IsSolved()
        {
            return Board[3, 1] == Utils.Red && Board[4, 2] == Utils.Red;
        }

        public string GetSnapshotHash()
        {
            var sb = new StringBuilder(); // can be extracted as static for perf

            for (int i = 0; i <= Utils.MaxRow; i++)
            {
                for (int j = 0; j <= Utils.MaxCol; j++)
                {
                    sb.Append((int)Utils.GetShape(Board[i, j]));
                }
            }

            return sb.ToString();
        }

        public bool IsTopLeftSquare()
        {
            if (Row < Utils.MaxRow && Col < Utils.MaxCol && Board[Row + 1, Col + 1] == Utils.Red)
            {
                return true;
            }

            return false;
        }

        public bool IsTopmostVertical()
        {
            if (Row < Utils.MaxRow && Board[Row + 1, Col] == Board[Row, Col])
            {
                return true;
            }

            return false;
        }

        public bool IsLeftmostHorizontal()
        {
            if (Col < Utils.MaxCol && Board[Row, Col + 1] == Board[Row, Col])
            {
                return true;
            }

            return false;
        }
    }
}
