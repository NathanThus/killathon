using Godot;
using System;

public partial class EnemyHealth : Area3D
{
	public event Action<EnemyHealth> OnDeath;

	[Export]
	public int MaxHealth { get; set; } = 100;
	[Export]
	public int CurrentHealth { get; set; } = 100;

    public override void _ExitTree()
	{
		OnDeath = null;
	}

	public void HandleHealthHitBox_Entered(Area3D area)
	{
		if(area is Projectile projectile) GD.Print("Honey, I fixed it! It can now parse damage: " + projectile.Damage);
		else GD.Print("Bing");

		OnDeath?.Invoke(this);
	}
}
