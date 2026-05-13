using Godot;
using System;

public partial class Projectile : RigidBody3D
{
	[Export]
	public int Damage { get; set; } = 20;
	[Export]
	public int AreaOfEffectMeters {get; set;} = 2;
	
	public void OnBodyEntered(Node3D node)
	{
		// For now, just direct fire.
		if(node is EnemyHealth enemy)
		{
			enemy.Damage(Damage);
			// Do spherecast and boom for all.	
		}

		// Probably hit something else

		// VFX
		QueueFree();
	}

}
