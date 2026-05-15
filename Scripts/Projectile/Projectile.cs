using Godot;
using System;

public partial class Projectile : RigidBody3D
{
	[Export]
	public int Damage { get; set; } = 20;
	[Export]
	public ShapeCast3D ShapeCast { get; set; }

	public void OnBodyEntered(Node3D node)
	{
		ShapeCast.ForceShapecastUpdate();
		
		if (!ShapeCast.IsColliding())
		{
			QueueFree();
			return;
		} 

		for (int i = 0; i < ShapeCast.GetCollisionCount(); i++)
		{
			if (ShapeCast.GetCollider(i) is EnemyHealth enemyHealth)
			{
				enemyHealth.Damage(Damage);
			}
		}
		// Probably hit something else
		// VFX
		QueueFree();
	}

}
