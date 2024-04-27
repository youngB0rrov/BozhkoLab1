using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
	static void Main()
	{
		var solver = new AStarMagicSquareSolver();
		var solution = solver.Solve();
		if (solution != null)
		{
			solver.PrintGrid(solution);
		}
		else
		{
			Console.WriteLine("No solution found.");
		}
	}
}

public class AStarMagicSquareSolver
{
	private const int Size = 4;
	private const int TargetSum = 30;
	private const int Empty = 0;

	public int[,] Solve()
	{
		var initialGrid = new int[Size, Size] {
			{1, 2, 3, 4},
			{5, 6, 7, 8},
			{9, 10, 11, 12},
			{13, 15, 14, Empty}
		};

		var openSet = new SortedSet<Node>(Comparer<Node>.Create((a, b) => a.F.CompareTo(b.F)));
		var closedSet = new HashSet<string>();
		var startNode = new Node(initialGrid, 0, CalculateH(initialGrid));
		openSet.Add(startNode);
		closedSet.Add(GridToString(initialGrid));

		while (openSet.Count > 0)
		{
			var currentNode = openSet.Min;
			openSet.Remove(currentNode);

			if (IsGoal(currentNode.Grid))
			{
				return currentNode.Grid;
			}

			foreach (var neighbor in GetNeighbors(currentNode))
			{
				var gridString = GridToString(neighbor.Grid);
				if (!closedSet.Contains(gridString))
				{
					openSet.Add(neighbor);
					closedSet.Add(gridString);
				}
			}
		}

		return null;
	}

	private IEnumerable<Node> GetNeighbors(Node node)
	{
		var (grid, g) = (node.Grid, node.G);
		var emptyPos = FindEmptyPosition(grid);
		var possibleMoves = GetPossibleMoves(emptyPos);
		foreach (var move in possibleMoves)
		{
			var newGrid = (int[,])grid.Clone();
			Swap(newGrid, emptyPos, move);
			var h = CalculateH(newGrid);
			yield return new Node(newGrid, g + 1, h);
		}
	}

	private string GridToString(int[,] grid)
	{
		return string.Join(",", grid.Cast<int>());
	}

	private List<(int, int)> GetPossibleMoves((int, int) pos)
	{
		var (x, y) = pos;
		var moves = new List<(int, int)> {
			(x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)
		};
		return moves.Where(m => m.Item1 >= 0 && m.Item1 < Size && m.Item2 >= 0 && m.Item2 < Size).ToList();
	}

	private (int, int) FindEmptyPosition(int[,] grid)
	{
		for (int i = 0; i < Size; i++)
		{
			for (int j = 0; j < Size; j++)
			{
				if (grid[i, j] == Empty)
				{
					return (i, j);
				}
			}
		}
		throw new Exception("No empty position in the grid.");
	}

	private void Swap(int[,] grid, (int, int) pos1, (int, int) pos2)
	{
		var (x1, y1) = pos1;
		var (x2, y2) = pos2;
		var temp = grid[x1, y1];
		grid[x1, y1] = grid[x2, y2];
		grid[x2, y2] = temp;
	}

	private int CalculateH(int[,] grid)
	{
		int sumDiff = 0;
		for (int i = 0; i < Size; i++)
		{
			int rowSum = 0, colSum = 0;
			for (int j = 0; j < Size; j++)
			{
				rowSum += grid[i, j];
				colSum += grid[j, i];
			}
			sumDiff += Math.Abs(TargetSum - rowSum);
			sumDiff += Math.Abs(TargetSum - colSum);
		}
		int diag1Sum = 0, diag2Sum = 0;
		for (int i = 0; i < Size; i++)
		{
			diag1Sum += grid[i, i];
			diag2Sum += grid[i, Size - 1 - i];
		}
		sumDiff += Math.Abs(TargetSum - diag1Sum);
		sumDiff += Math.Abs(TargetSum - diag2Sum);
		return sumDiff;
	}

	private bool IsGoal(int[,] grid)
	{
		int sumDiff = CalculateH(grid);
		return sumDiff == 0;
	}

	public void PrintGrid(int[,] grid)
	{
		for (int i = 0; i < Size; i++)
		{
			for (int j = 0; j < Size; j++)
			{
				Console.Write(grid[i, j].ToString().PadRight(3));
			}
			Console.WriteLine();
		}
		Console.WriteLine();
	}

	private class Node
	{
		public int[,] Grid { get; }
		public int G { get; }
		public int H { get; }
		public int F => G + H;

		public Node(int[,] grid, int g, int h)
		{
			Grid = grid;
			G = g;
			H = h;
		}
	}
}
