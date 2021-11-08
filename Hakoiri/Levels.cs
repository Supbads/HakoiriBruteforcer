namespace Hakoiri
{
    public static class Levels
    {
        public static byte[,] LevelTest = new byte[5, 4]
        {
            { 0, Utils.Red, Utils.Red, 0 },
            { 1, Utils.Red, Utils.Red, 0 },
            { 0, 2, 2, 0 },
            { 0, 0, 0, 0 },
            { 0, 1, 1, 0 },
        };

        public static byte[,] LevelTestCompact = new byte[5, 4]
        {
            { 3, Utils.Red, Utils.Red, 5 },
            { 3, Utils.Red, Utils.Red, 5 },
            { 7, 0, 0, 1 },
            { 7, 0, 0, 9 },
            { 1, 0, 0, 9 },
        };

        // 1795 steps
        public static byte[,] Level0 = new byte[5, 4]
        {
            { 3, Utils.Red, Utils.Red, 5 },
            { 3, Utils.Red, Utils.Red, 5 },
            { 1, 1, 1, 1 },
            { 7, 2, 2, 9 },
            { 7, 0, 0, 9 },
        };
        
        // 7880 steps
        public static byte[,] Level1 = new byte[5, 4]
        {
            { 3, Utils.Red, Utils.Red, 5 },
            { 3, Utils.Red, Utils.Red, 5 },
            { 2, 2, 4, 4 },
            { 1, 1, 1, 1 },
            { 1, 0, 0, 1 },
        };

        // 7670 steps
        public static byte[,] Level2 = new byte[5, 4]
        {
            { 3, Utils.Red, Utils.Red, 5 },
            { 3, Utils.Red, Utils.Red, 5 },
            { 1, 1, 1, 1 },
            { 2, 2, 4, 4 },
            { 1, 0, 0, 1 },
        };

        // 271 steps
        public static byte[,] Level3 = new byte[5, 4]
        {
            { 1, Utils.Red, Utils.Red, 1 },
            { 1, Utils.Red, Utils.Red, 1 },
            { 2, 2, 4, 4 },
            { 6, 6, 8, 8 },
            { 1, 0, 0, 1 },
        };

        // 100 steps
        public static byte[,] Level4 = new byte[5, 4]
        {
            { 1, Utils.Red, Utils.Red, 1 },
            { 3, Utils.Red, Utils.Red, 9 },
            { 3, 1, 1, 9 },
            { 5, 2, 2, 7 },
            { 5, 0, 0, 7 },
        };

        // 3094 steps
        public static byte[,] Level5 = new byte[5, 4]
        {
            { 3, Utils.Red, Utils.Red, 7 },
            { 3, Utils.Red, Utils.Red, 7 },
            { 5, 1, 1, 9 },
            { 5, 1, 1, 9 },
            { 1, 0, 0, 1 },
        };

        // 1290 steps
        public static byte[,] Level6 = new byte[5, 4]
        {
            { 3, Utils.Red, Utils.Red, 7 },
            { 3, Utils.Red, Utils.Red, 7 },
            { 5, 2, 2, 9 },
            { 5, 1, 1, 9 },
            { 1, 0, 0, 1 },
        };

        // 2265 steps
        public static byte[,] Level7 = new byte[5, 4] 
        {
            { 3, Utils.Red, Utils.Red, 5 },
            { 3, Utils.Red, Utils.Red, 5 },
            { 2, 2, 4, 4 },
            { 1, 6, 6, 1 },
            { 1, 0, 0, 1 },
        };

        // 2588 steps
        public static byte[,] Level8 = new byte[5, 4]
        {
            { 3, Utils.Red, Utils.Red, 5 },
            { 3, Utils.Red, Utils.Red, 5 },
            { 1, 1, 1, 1 },
            { 1, 2, 2, 1 },
            { 1, 0, 0, 1 },
        };
    }
}

//hashing ideas
//10  objects
//4 types
// coordinates can be flattened -> 5*4 = 20 instead of x and y