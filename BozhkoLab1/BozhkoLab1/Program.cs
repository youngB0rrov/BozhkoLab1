using BozhkoLab1.Models;
using System.Diagnostics.CodeAnalysis;

var solution = new Tag();
solution.FindSolution();

class Tag
{
	private int H = 10000; 
	private readonly int _maxIterationsNumber = 1000;
	private List<State> Open { get; set; } = new();
	private List<State> History { get; set; } = new();
	private void Initialize()
	{
		var StartPosition = new State(0);
		History.Add(StartPosition);
		Open.Add(StartPosition);
	}
	private State FindStateViaMinimalF()
	{
		var minimalF = Open.Where(x => !x.IsOpened).Select(x => x.G + x.H).Min();
		var stateViaMinimalF = Open.Where(x => !x.IsOpened).First(x => (x.G + x.H) == minimalF);
		stateViaMinimalF.IsOpened = true;
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
				var isAlreadyOpen = generatedDescendent.IsAlreadyOpen(History.ToList());
				if (!isAlreadyOpen)
				{
					History.Add(generatedDescendent);
				}
			}
			Console.WriteLine($"H: {stateViaMinimalF.H}, G: {stateViaMinimalF.G}");
		}
	}
}

