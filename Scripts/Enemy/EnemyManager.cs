using Godot;
using System;
using System.Threading.Tasks;

public partial class EnemyManager : Node
{
	[Export]
	public Node3D SpawnPoint { get; set; }
	[Export]
	public Node3D Target { get; set; }
	[Export]
	public int SpawnLimit { get; set; }
	[Export]
	public int SpawnCount { get; set; }
	[Export]
	public int SpawnDelayTimeMilliSeconds {get; set;} = 500;

	private readonly PackedScene template = GD.Load<PackedScene>("res://BaseEnemy.tscn");
	public override void _Ready()
	{
		SpawnZombie();
	}

	private async void SpawnZombie()
	{
		if (SpawnCount < SpawnLimit)
		{
			for (int i = SpawnCount; i < SpawnLimit; i++)
			{
				var instance = template.Instantiate<ZombieLogic>();
				AddChild(instance);
				instance.Position = SpawnPoint.Position;
				instance.MovementTarget = Target.Position;
				SpawnCount++;
				await Task.Delay(SpawnDelayTimeMilliSeconds);
			}
		}
	}

	private void OnEnemyDeath()
	{
		// Stop listening to event
	}
}
