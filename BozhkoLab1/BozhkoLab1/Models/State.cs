using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BozhkoLab1.Models
{
	public class State
	{
		private readonly int _n = 4;
		private readonly int _sum = 30;
		public int G { get; set; } = 0;
		public int H { get; set; } = 0;
		public bool IsOpened { get; set; }
		public State? Parent { get; set; }
		public List<List<int>> Position { get; set; } = new();
		public State(int g)
		{
			G = g;
			Position = new List<List<int>>
			{
				new List<int> { 1, 2, 3, 4},
				new List<int> { 5, 6, 7, 8},
				new List<int> { 9, 10,11, 12},
				new List<int> { 13, 15, 14, 0},
			};
			CalculateH();
		}
		public State(State parent, List<List<int>> position, int g)
		{
			G = g;
			Position = position;
			Parent = parent;
			CalculateH();
		}
		private static State MoveRight(State parent, int g, int i, int j)
		{
			var newPosition = parent.Position.ConvertAll(x => new List<int>(x));
			var temp = parent.Position[i][j + 1];
			newPosition[i][j + 1] = 0;
			newPosition[i][j] = temp;

			return new State(parent, newPosition, g);
		}
		private static State MoveLeft(State parent, int g, int i, int j)
		{
			var newPosition = parent.Position.ConvertAll(x => new List<int>(x));
			var temp = parent.Position[i][j - 1];
			newPosition[i][j - 1] = 0;
			newPosition[i][j] = temp;

			return new State(parent, newPosition, g);
		}
		private static State MoveUp(State parent, int g, int i, int j)
		{
			var newPosition =	parent.Position.ConvertAll(x => new List<int>(x));
			var temp = parent.Position[i - 1][j];
			newPosition[i - 1][j] = 0;
			newPosition[i][j] = temp;

			return new State(parent, newPosition, g);
		}
		private static State MoveDown(State parent, int g, int i, int j)
		{
			var newPosition = parent.Position.ConvertAll(x => new List<int>(x));
			var temp = parent.Position[i + 1][j];
			newPosition[i + 1][j] = 0;
			newPosition[i][j] = temp;

			return new State(parent, newPosition, g);
		}
		private int GetColumnSum(int columnIndex)
		{
			var sum = 0;
			for (var i = 0; i < _n; ++i)
			{
				sum += Position[i][columnIndex];
			}
			return sum;
		}
		private int GetRowSum(int rowIndex)
		{
			var sum = 0;
			for (var i = 0; i < _n; ++i)
			{
				sum += Position[rowIndex][i];
			}
			return sum;
		}
		private Tuple<int, int> GetDiagonalsSum()
		{
			var leftDiagonalSum = 0;
			var rightDiagonalSum = 0;

			for (var i = 0; i < _n; ++i)
			{
				leftDiagonalSum += Position[i][i];
			}
			for (var i = _n - 1; i <= 0; --i)
			{
				rightDiagonalSum += Position[i][_n - 1 - i];
			}
			return Tuple.Create(leftDiagonalSum, rightDiagonalSum);
		}
		private void CalculateH()
		{
			var h = 0;

			for (var i = 0; i < _n; ++i)
			{
				var columnDelta = Math.Abs(_sum - GetColumnSum(i));
				var rowDelta = Math.Abs(_sum - GetRowSum(i));
				h += columnDelta;
				h += rowDelta;
			}
			var diagonalsSum = GetDiagonalsSum();
			h += Math.Abs(_sum - diagonalsSum.Item1);
			h += Math.Abs(_sum - diagonalsSum.Item2);

			H = h;
		}
		private Tuple<int, int> GetEmptyPositionIndexes(List<List<int>> position)
		{
			var result = new Tuple<int, int>(0, 0);
			for (var i = 0; i < _n; ++i)
			{
				for (var j = 0; j < _n; j++)
				{
					if (position[i][j] == 0)
					{
						result = Tuple.Create(i, j);
					}
				}
			}

			return result;
		}
		public bool IsAlreadyOpen(List<State> opened)
		{
			var currentItemDigits = this.Position.SelectMany(x => x.Select(y => y)).ToList();
			
			foreach (var open in opened)
			{
				var openItemDigits = open.Position.SelectMany(x => x.Select(y => y)).ToList();
				var equalDigitsCount = 0;
				for (var i = 0; i < _n * _n; ++i)
				{
					if (openItemDigits[i] == currentItemDigits[i])
					{
						equalDigitsCount++;
					}
				}
				if (equalDigitsCount == _n * _n)
				{
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// Функция порождения потомков из указанного состояния
		/// </summary>
		/// <param name="position">List<List<int>></param>
		/// <returns></returns>
		public List<State> GenarateDescendants()
		{
			var result = new List<State>();
			var emptyFieldPositon = GetEmptyPositionIndexes(this.Position);
			var i = emptyFieldPositon.Item1;
			var j = emptyFieldPositon.Item2;
			var g = this.G + 1;

			// Нижний правый угол
			if (i == _n - 1 && j == _n - 1)
			{
				result.Add(MoveLeft(this, g, i, j));
				result.Add(MoveUp(this, g, i, j));
				return result;
			}

			// Нижный левый угол
			if (i == _n - 1 && j == 0)
			{
				result.Add(MoveRight(this, g, i, j));
				result.Add(MoveUp(this, g, i, j));
				return result;
			}

			// Верхний левый угол
			if (i == 0 && j == 0)
			{
				result.Add(MoveRight(this, g, i, j));
				result.Add(MoveDown(this, g, i, j));
				return result;
			}

			// Верхний правый угол
			if (i == 0 && j == _n - 1)
			{
				result.Add(MoveLeft(this, g, i, j));
				result.Add(MoveDown(this, g, i, j));
				return result;
			}

			// Верхная строка (по середине)
			if (i == 0)
			{
				result.Add(MoveRight(this, g, i, j));
				result.Add(MoveLeft(this, g, i, j));
				result.Add(MoveDown(this, g, i, j));
				return result;
			}

			// Нижняя строка (по середине)
			if (i == _n - 1)
			{
				result.Add(MoveRight(this, g, i, j));
				result.Add(MoveLeft(this, g, i, j));
				result.Add(MoveUp(this, g, i, j));
				return result;
			}

			// Левый столбец (по середине)
			if (j == 0)
			{
				result.Add(MoveRight(this, g, i, j));
				result.Add(MoveDown(this, g, i, j));
				result.Add(MoveUp(this, g, i, j));
				return result;
			}

			// Правый столбец (по середине)
			if (j == _n - 1)
			{
				result.Add(MoveDown(this, g, i, j));
				result.Add(MoveLeft(this, g, i, j));
				result.Add(MoveUp(this, g, i, j));
				return result;
			}

			result.Add(MoveDown(this, g, i, j));
			result.Add(MoveLeft(this, g, i, j));
			result.Add(MoveUp(this, g, i, j));
			result.Add(MoveRight(this, g, i, j));
			return result;
		}
	}
}
