using BozhkoLab1.Models;
using System.Diagnostics.CodeAnalysis;

var solution = new Tag();
solution.FindSolution();

class Tag
{
	private int H = 10000; 
	private readonly int _maxIterationsNumber = 1000;
	private List<State> Open { get; set; } = new();
	private List<State> Close { get; set; } = new();
	private void Initialize()
	{
		var StartPosition = new State(0);
		Open.Add(StartPosition);
	}
	private State FindStateViaMinimalF()
	{
		var minimalF = Open.Select(x => x.G + x.H).Min();
		var stateViaMinimalF = Open.First(x => (x.G + x.H) == minimalF);
		return stateViaMinimalF;
	}
	public void FindSolution()
	{
		// Инициализировать начальное состояние
		Initialize();

		while(H > 1)
		{
			// Найти позицию с минимальной F, от которой надо порождать потомков
			var stateViaMinimalF = FindStateViaMinimalF();
			H = stateViaMinimalF.H;

			// Породить всех потомков из текущего состояния
			// Проверить, присутствуют ли они в Open
			// Если не присутствуют - добавить в Open
			var generatedDescendents = stateViaMinimalF.GenarateDescendants();
			foreach (var generatedDescendent in generatedDescendents)
			{
				var isAlreadyOpen = generatedDescendent.IsAlreadyOpen(Open, Close);
				if (!isAlreadyOpen)
				{
					Open.Add(generatedDescendent);
				}
			}
			Open.Remove(stateViaMinimalF);
			Close.Add(stateViaMinimalF);
			Console.WriteLine($"H: {stateViaMinimalF.H}, G: {stateViaMinimalF.G}");
			if (H <= 10)
			{
				stateViaMinimalF.PrintState();
				Console.WriteLine($"H: {stateViaMinimalF.H}, G: {stateViaMinimalF.G}");
				Console.WriteLine();
			}
		}
	}
}

