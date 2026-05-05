using Godot;
using System;
using System.Threading.Tasks;

public partial class EnemyManager : Node
{
	[Export]
	public Node3D[] SpawnPoints { get; set; }
	[Export]
	public Node3D Target { get; set; }
	[Export]
	public int SpawnLimit { get; set; }
	[Export]
	public int SpawnCount { get; set; }
	[Export]
	public int SpawnDelayTimeMilliSeconds { get; set; } = 500;
	[Export]
	public int UpdateTimer { get; set; } = 2000;

	private bool isSpawning = false;

	private readonly PackedScene template = GD.Load<PackedScene>("res://BaseEnemy.tscn");
	public override void _Ready()
	{
		SpawnZombie();
	}

	private async void SpawnZombie()
	{
		while (true)
		{
			if (SpawnCount < SpawnLimit && !isSpawning)
			{
				isSpawning = true;
				for (int i = SpawnCount; i < SpawnLimit; i++)
				{
					var instance = template.Instantiate<ZombieLogic>();
					AddChild(instance);
					instance.Position = GetSpawnPosition();
					instance.MovementTarget = Target.Position;
					instance.OnDeath += HandleEnemyDeath;
					SpawnCount++;
					await Task.Delay(SpawnDelayTimeMilliSeconds);
				}
				isSpawning = false;
			}

			await Task.Delay(UpdateTimer);
		}
	}

	private int index;
	private Vector3 GetSpawnPosition()
	{
		index++;
		if (index >= SpawnPoints.Length) index = 0;
		return SpawnPoints[index].Position;
	}

	private void HandleEnemyDeath(ZombieLogic instance)
	{
		instance.OnDeath -= HandleEnemyDeath;
		SpawnCount--;
	}
}
