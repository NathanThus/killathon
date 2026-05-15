using Godot;
using System;

public partial class Projectile : RigidBody3D
{
	[Export]
	public int Damage { get; set; } = 20;
	[Export]
	public ShapeCast3D ShapeCast {get; set;}

	public void OnBodyEntered(Node3D node)
	{
		// For now, just direct fire.
		if (node is EnemyHealth enemy)
		{
			ShapeCast.ForceShapecastUpdate();
			if(!ShapeCast.IsColliding()) return;
			for (int i = 0; i < ShapeCast.GetCollisionCount(); i++)
			{
				if(ShapeCast.GetCollider(i) is EnemyHealth enemyHealth)
				{
					enemyHealth.Damage(Damage);
				}
			}
		}

		// Probably hit something else

		// VFX
		QueueFree();
	}

}
