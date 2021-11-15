using System;
using System.Text;

namespace Hakoiri
{
    public class Snapshot
    {
        // look indices
        public byte Row { get; set; }
        public byte Col { get; set; }
        public int Step { get; set; }

        public byte[,] BoardSnapshot;
        public Snapshot refSnap;
        public Snapshot(Snapshot snapshot)
        {
            BoardSnapshot = Utils.CopyBoard(snapshot.BoardSnapshot);
            this.refSnap = snapshot;
            Row = 0;
            Col = 0;
            Step = snapshot.Step + 1;
        }

        public Snapshot(byte[,] board)
        {
            BoardSnapshot = Utils.CopyBoard(board);
            Row = 0;
            Col = 0;
            Step = 1;
        }

        public Snapshot(byte[,] board, byte x, byte y)
        {
            BoardSnapshot = Utils.CopyBoard(board);
            Row = x;
            Col = y;
        }

        public bool Increment()
        {
            if(Row == Utils.MaxRow && Col == Utils.MaxCol)
            { // we are at the end
                return false;
            }

            if(Col == Utils.MaxCol)
            {
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
            return BoardSnapshot[Row, Col];
        }

        public bool IsSolved()
        {
            return BoardSnapshot[3, 1] == Utils.Red && BoardSnapshot[4, 2] == Utils.Red;
        }

        public string GetSnapshotHash()
        {
            var sb = new StringBuilder(); // can be extracted as static for eprf
            
            for (int i = 0; i <= Utils.MaxRow; i++)
            {
                for (int j = 0; j <= Utils.MaxCol; j++)
                {
                    sb.Append((int)Utils.GetShape(BoardSnapshot[i, j]));
                }
            }

            return sb.ToString();
        }

        public bool IsTopLeftSquare()
        {
            if(Row < Utils.MaxRow && Col < Utils.MaxCol && BoardSnapshot[Row + 1, Col + 1] == Utils.Red)
            {
                return true;
            }

            return false;
        }

        public bool IsTopmostVertical()
        {
            if (Row < Utils.MaxRow && BoardSnapshot[Row + 1, Col] == BoardSnapshot[Row, Col])
            {
                return true;
            }

            return false;
        }

        public bool IsLeftmostHorizontal()
        {
            if (Col < Utils.MaxCol && BoardSnapshot[Row, Col + 1] == BoardSnapshot[Row, Col])
            {
                return true;
            }

            return false;
        }

        // try move indexes
        // care outbounds
        // handle end of possibilities -> aka backward

        // save solution

    }
}
