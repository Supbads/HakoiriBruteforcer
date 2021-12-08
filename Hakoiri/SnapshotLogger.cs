using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hakoiri
{
    public class SnapshotLogger
    {
        public void PrintSnapshot(Snapshot snapshot)
        {
            for (int i = 0; i <= Utils.MaxRow; i++)
            {
                for (int j = 0; j <= Utils.MaxCol; j++)
                {
                    Console.Write(snapshot.Board[i, j].ToString().PadRight(3));
                }
                Console.WriteLine();
            }
        }

        public string FormatSnapshotAsString(Snapshot snapshot)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i <= Utils.MaxRow; i++)
            {
                for (int j = 0; j <= Utils.MaxCol; j++)
                {
                    sb.Append(snapshot.Board[i, j].ToString().PadRight(3));
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public void SaveRefSolutionsToTxt(List<Snapshot> backtrackSolutions)
        {
            Console.WriteLine("saving ref solutions steps");
            int step = 1;
            StringBuilder sb = new StringBuilder();
            foreach (var solution in backtrackSolutions)
            {
                sb.AppendLine("step " + step);
                sb.AppendLine(FormatSnapshotAsString(solution));
                sb.AppendLine();
                step++;
            }

            File.WriteAllText($"{Directory.GetCurrentDirectory()}\\..\\..\\..\\solution.txt", sb.ToString());
        }

        public void PrintAllRefSolutions(List<Snapshot> backtrackSolutions)
        {
            Console.WriteLine("printing ref solutions steps");
            int step = 1;
            foreach (var solution in backtrackSolutions)
            {
                Console.WriteLine("step " + step);
                PrintSnapshot(solution);
                Console.WriteLine();
                step++;
            }
        }
    }
}
