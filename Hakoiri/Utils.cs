using System;

namespace Hakoiri
{
    public static class Utils
    {
        public enum Shape
        {
            Empty = 0,
            Square = 1,
            Single = 2,
            Vertical = 3,
            Horizontal = 4
        }

        public static byte MaxRow = 4; //max index
        public static byte MaxCol = 3; //max index

        public const byte Red = 50;

        public static Shape GetShape(byte cell)
        {
            if(cell == Red)
                return Shape.Square;

            if(cell == 0)
                return Shape.Empty;

            if(cell == 1)
                return Shape.Single;
            

            if (IsVertical(cell))
                return Shape.Vertical;
            

            if (IsHorizontal(cell))
                return Shape.Horizontal;
            
            throw new Exception("unrecognized cell - shouldn't happen");
        }

        public static bool IsBlue(byte num)
        {
            return num == 1;
        }

        public static bool IsRed(byte num)
        {
            return num == Red;
        }

        public static bool IsVertical(byte num)
        {
            return num % 2 == 1;
        }

        public static bool IsHorizontal(byte num)
        {
            return num % 2 == 0;
        }

        public static byte[,] CopyBoard(byte[,] board)
        {
            var copy = new byte[5, 4];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    copy[i, j] = board[i, j];
                }
            }

            return copy;
        }
    }
}
