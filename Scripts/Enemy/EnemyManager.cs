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
	public PackedScene template {get; set;} = GD.Load<PackedScene>("res://BaseEnemy.tscn");

	private bool isSpawning = false;

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
					var instance = template.Instantiate<EnemyController>();
					AddChild(instance);

					instance.Position = GetSpawnPosition();
					instance.MovementTarget = Target.Position;
					instance.Health.OnDeath += HandleEnemyDeath;

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

	private void HandleEnemyDeath(EnemyHealth instance)
	{
		instance.OnDeath -= HandleEnemyDeath;
		SpawnCount--;
	}
}
