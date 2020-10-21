#define SHOW_STEPS

using System;
using System.Collections.Generic;
using System.Linq;

namespace Canonical
{
    using fractional = Decimal;
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
                            PrintSteps($"Skip column {j + 1}.");
                            j++;
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
