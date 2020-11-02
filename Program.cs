#define SHOW_STEPS

using System;

namespace Canonical
{
    using fractional = Double;
    class Program
    {
        static void PrintSteps(string format = "", params object[] args)
        {
#if SHOW_STEPS
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(format, args);
            Console.ResetColor();
#endif
        }
        static void Main(string[] args)
        {
            Console.ResetColor();

            uint rows, columns;
            do
            {
                Console.Write("rows: ");
            } while (!uint.TryParse(Console.ReadLine(), out rows));
            do
            {
                Console.Write("columns: ");
            } while (!uint.TryParse(Console.ReadLine(), out columns));

            var matrix = new fractional[rows, columns];
            try
            {
                for (int i = 0; i < rows; i++)
                {
                    var line = Console.ReadLine();
                    var splitted = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (splitted.Length != columns)
                        throw new ArgumentException();

                    for (int j = 0; j < columns; j++)
                    {
                        matrix[i, j] = fractional.Parse(splitted[j]);
                    }
                }
            }
            catch
            {
                Console.Error.WriteLine("Matrix parsing error.");
                Environment.Exit(1);
            }

            {
                int i = 0, j = 0;
                while (i < rows && j < columns)
                {
                    Console.WriteLine();
                    PrintMatrix(rows, columns, matrix, (i, j));

                    if (matrix[i, j] == 0)
                    {
                        int k;
                        for (k = i + 1; k < rows; k++)
                        {
                            if (matrix[k, j] != 0)
                            {
                                break;
                            }
                        }

                        // No such rows.
                        if (k == rows)
                        {
                            bool notNullAbove = false;
                            for (k = 0; k < i; k++)
                            {
                                notNullAbove |= matrix[k, j] != 0;
                            }

                            if (notNullAbove)
                            {
                                for (k = j + 1; k < columns; ++k)
                                {
                                    if (matrix[i, k] != 0)
                                    {
                                        break;
                                    }
                                }

                                // No such columns.
                                if (k == columns)
                                {
                                    PrintSteps($"Skip column {j + 1}.");
                                    j++;
                                }
                                else
                                {
                                    PrintSteps($"Swap columns {j + 1} and {k + 1}.");
                                    for (int l = 0; l < rows; l++)
                                    {
                                        (matrix[l, j], matrix[l, k]) = (matrix[l, k], matrix[l, j]);
                                    }
                                }
                            }
                            else
                            {
                                PrintSteps($"Skip column {j + 1}.");
                                j++;
                            }
                        }
                        else
                        {
                            PrintSteps($"Swap rows {i + 1} and {k + 1}.");
                            for (int l = j; l < columns; l++)
                            {
                                (matrix[i, l], matrix[k, l]) = (matrix[k, l], matrix[i, j]);
                            }
                        }
                    }
                    else
                    {
                        fractional coeff = matrix[i, j];
                        PrintSteps($"M[{i + 1}] /= {coeff}.");
                        for (int k = j; k < columns; ++k)
                        {
                            matrix[i, k] /= coeff;
                        }

                        for (int k = 0; k < rows; k++)
                        {
                            if (k == i)
                            {
                                continue;
                            }

                            coeff = matrix[k, j] / matrix[i, j];
                            PrintSteps($"M[{k + 1}] -= M[{i + 1}] * {coeff}.");
                            for (int l = j; l < columns; l++)
                            {
                                matrix[k, l] -= coeff * matrix[i, l];
                            }
                        }

                        i++; j++;
                    }
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Result:");
                PrintMatrix(rows, columns, matrix);
                Console.ResetColor();
            }
        }

        private static void PrintMatrix(uint rows, uint columns, fractional[,] matrix, (int, int)? hightlightElement = null)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if ((i, j) == hightlightElement)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    Console.Write("{0,8:G4} ", matrix[i, j]);
                    if ((i, j) == hightlightElement)
                    {
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
            }
        }
    }
}

/*
4
6
6 4 5 2 3 1
3 2 -2 1 0 -7
9 6 1 3 2 2
3 2 4 1 2 3
*/

/*
3
5
-6 9 3 2 4
-2 3 5 4 2
-4 6 4 3 3
*/

/*
3
6
1 -1 4 3 0 0
3 -2 1 2 0 1
2 -1 -3 -3 2 1
*/

/*
4
5
5 21 1 -9 -13
5 41 -4 -14 2
1 -11 4 2 -14
-4 -40 5 13 -7
*/

/*
5
6
81 -27 9 -3 1 4
16 -8 4 -2 1 0
1 -1 1 -1 1 3
1 1 1 1 1 3
256 64 16 4 1 1
*/

/*
3
4
11 -7 61 -3
0 6 -24 1
3 -2 17 1
*/
