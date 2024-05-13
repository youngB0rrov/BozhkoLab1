using System;
using System.Collections.Generic;
using System.Linq;

class BeamSearchSudokuSolver
{
	const int Size = 9; // Размер сетки судоку
	const int BlockSize = 3; // Размер блока
	const int BeamWidth = 50; // Количество лучших путей, которое мы будем учитывать на каждом шаге
	int IterationsCount = 0;
	private int[,] initialBoard;
	private readonly int[,] _startBoard;

	public BeamSearchSudokuSolver(int[,] board)
	{
		initialBoard = (int[,])board.Clone();
		_startBoard = (int[,])board.Clone();
	}

	// Заполнение пустых ячеек числами, отсутствующими в строке
	public void FillEmptyCellsUniquePerRow()
	{
		for (int r = 0; r < Size; r++)
		{
			// Определим уже присутствующие числа в строке
			HashSet<int> existingNumbers = new HashSet<int>();
			for (int c = 0; c < Size; c++)
			{
				if (initialBoard[r, c] != 0)
				{
					existingNumbers.Add(initialBoard[r, c]);
				}
			}

			// Заполним каждую пустую ячейку числами, отсутствующими в строке
			for (int c = 0; c < Size; c++)
			{
				if (initialBoard[r, c] == 0)
				{
					for (int num = 1; num <= Size; num++)
					{
						if (!existingNumbers.Contains(num))
						{
							initialBoard[r, c] = num;
							existingNumbers.Add(num);
							break; // Прекращаем поиск при первой успешной замене
						}
					}
				}
			}
		}
	}

	// Функция для вычисления эвристической оценки доски
	private int Heuristic(int[,] board)
	{
		int conflicts = 0;

		// Проверка строк на конфликты
		for (int r = 0; r < Size; r++)
		{
			HashSet<int> rowNumbers = new HashSet<int>();
			for (int c = 0; c < Size; c++)
			{
				rowNumbers.Add(board[r, c]);
			}
			conflicts += Size - rowNumbers.Count;
		}

		// Проверка столбцов на конфликты
		for (int c = 0; c < Size; c++)
		{
			HashSet<int> columnNumbers = new HashSet<int>();
			for (int r = 0; r < Size; r++)
			{
				columnNumbers.Add(board[r, c]);
			}
			conflicts += Size - columnNumbers.Count;
		}

		// Проверка блоков на конфликты
		for (int br = 0; br < BlockSize; br++)
		{
			for (int bc = 0; bc < BlockSize; bc++)
			{
				HashSet<int> blockNumbers = new HashSet<int>();
				for (int r = br * BlockSize; r < (br + 1) * BlockSize; r++)
				{
					for (int c = bc * BlockSize; c < (bc + 1) * BlockSize; c++)
					{
						blockNumbers.Add(board[r, c]);
					}
				}
				conflicts += Size - blockNumbers.Count;
			}
		}

		return conflicts;
	}

	private List<int[,]> GenerateNeighbors(int[,] board)
	{
		List<int[,]> neighbors = new List<int[,]>();
		bool foundImpovement = false;

		// Проходим по каждой строке
		for (int r = 0; r < Size; r++)
		{
			// Собираем список всех изменяемых ячеек в строке
			List<int> mutableColumns = new List<int>();
			for (int c = 0; c < Size; c++)
			{
				if (_startBoard[r, c] == 0)
				{
					mutableColumns.Add(c);
				}
			}

			// Определяем изначальное количество конфликтов для этой строки
			int initialConflicts = Heuristic(board);

			// Перестановка пар значений внутри строки с учетом минимизации конфликтов
			for (int i = 0; i < mutableColumns.Count - 1; i++)
			{
				for (int j = i + 1; j < mutableColumns.Count; j++)
				{
					int[,] neighbor = (int[,])board.Clone();
					int c1 = mutableColumns[i];
					int c2 = mutableColumns[j];

					// Меняем два числа местами в пределах строки
					int temp = neighbor[r, c1];
					neighbor[r, c1] = neighbor[r, c2];
					neighbor[r, c2] = temp;

					// Проверяем, уменьшились ли конфликты после перестановки
					if (Heuristic(neighbor) < initialConflicts)
					{
						neighbors.Add(neighbor);
						foundImpovement = true;
					}
				}
			}
		}

		if (!foundImpovement)
		{
			for (int r = 0; r < Size; r++)
			{
				List<int> mutableColumns = new List<int>();
				for (int c = 0; c < Size; c++)
				{
					if (_startBoard[r, c] == 0)
					{
						mutableColumns.Add(c);
					}
				}

				// Перестановка пар значений внутри строки с учетом минимизации конфликтов
				for (int i = 0; i < mutableColumns.Count - 1; i++)
				{
					for (int j = i + 1; j < mutableColumns.Count; j++)
					{
						int[,] neighbor = (int[,])board.Clone();
						int c1 = mutableColumns[i];
						int c2 = mutableColumns[j];

						// Меняем два числа местами в пределах строки
						int temp = neighbor[r, c1];
						neighbor[r, c1] = neighbor[r, c2];
						neighbor[r, c2] = temp;
						neighbors.Add(neighbor);
					}
				}
			}
		}

		return neighbors;
	}

	// Лучевой поиск для нахождения решения
	public void BeamSearchSolve()
	{
		List<int[,]> beam = new List<int[,]> { initialBoard };
		HashSet<string> visited = new HashSet<string>();

		while (true)
		{
			IterationsCount++;
			List<int[,]> nextBeam = new List<int[,]>();

			foreach (var board in beam)
			{
				List<int[,]> neighbors = GenerateNeighbors(board);

				foreach (var neighbor in neighbors)
				{
					string serializedBoard = string.Join(",", neighbor.Cast<int>());
					if (!visited.Contains(serializedBoard))
					{
						visited.Add(serializedBoard);
						nextBeam.Add(neighbor);
					}
				}
			}

			nextBeam = nextBeam.OrderBy(board => Heuristic(board)).Take(BeamWidth).ToList();

			if (nextBeam.Count > 0 && Heuristic(nextBeam[0]) == 0)
			{
				PrintBoard(nextBeam[0]);
				Console.WriteLine($"Всего глубина вложенности: {IterationsCount}");
				return;
			}

			Console.WriteLine("-------------------");
			Console.WriteLine("-------------------");
			foreach (var beam1 in nextBeam)
			{
				PrintBoard(beam1);
			}
			Console.WriteLine("-------------------");
			Console.WriteLine("-------------------");

			beam = nextBeam;

			if (beam.Count == 0)
			{
				Console.WriteLine("Решение не найдено.");
				return;
			}
		}
	}

	// Печать доски
	private void PrintBoard(int[,] board)
	{
		Console.WriteLine($"Количество повторов: {Heuristic(board)}");
		for (int r = 0; r < Size; r++)
		{
			if (r % 3 == 0 && r != 0 && r != Size - 1)
			{
				for (var i = 0; i < Size; i++)
				{
					Console.Write("--");
				}
				Console.Write('-');
				Console.WriteLine();
			}
			for (int c = 0; c < Size; c++)
			{
				if (c % 3 == 0 && c != 0 && c != Size - 1)
				{
					Console.Write("|");
				}
				Console.Write($"{board[r, c]} ");
			}
			Console.WriteLine();
		}
		Console.WriteLine();
	}
}

class Program
{
	static void Main()
	{
		// Предварительное заполнение судоку
		int[,] initialBoard = {
			{ 6, 0, 0, 8, 0, 0, 0, 0, 0 },
			{ 2, 8, 0, 0, 0, 0, 0, 0, 3 },
			{ 0, 0, 0, 0, 0, 0, 0, 8, 4 },
			{ 0, 0, 1, 7, 0, 0, 0, 5, 0 },
			{ 8, 0, 0, 0, 0, 0, 0, 0, 7 },
			{ 0, 4, 0, 0, 0, 9, 3, 0, 0 },
			{ 4, 7, 0, 0, 0, 0, 0, 0, 0 },
			{ 1, 0, 0, 0, 0, 0, 0, 4, 6 },
			{ 0, 0, 0, 0, 0, 7, 0, 0, 9 }
		};

		// Второй вариант
		//		int[,] initialBoard = {
		//			{ 8, 0, 5, 0, 6, 0, 0, 2, 0 },
		//			{ 0, 9, 0, 3, 0, 0, 0, 6, 0 },
		//			{ 0, 0, 0, 0, 7, 2, 0, 0, 0 },
		//			{ 0, 0, 8, 0, 0, 0, 0, 0, 0 },
		//			{ 0, 0, 1, 0, 0, 0, 6, 0, 0 },
		//			{ 0, 0, 0, 0, 0, 0, 8, 0, 0 },
		//			{ 0, 0, 0, 7, 8, 0, 0, 0, 0 },
		//			{ 0, 8, 0, 0, 0, 1, 0, 7, 0 },
		//			{ 0, 1, 0, 9, 0, 1, 2, 0, 5 }
		//		};

		// Создаем экземпляр класса-решателя и запускаем алгоритм
		BeamSearchSudokuSolver solver = new BeamSearchSudokuSolver(initialBoard);
		solver.FillEmptyCellsUniquePerRow();
		solver.BeamSearchSolve();
	}
}
