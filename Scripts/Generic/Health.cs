using Godot;
using System;

public partial class Health : Area3D
{
	public event Action<Health> OnDeath;

	[Export]
	public int MaxHealth { get; set; } = 100;
	[Export]
	public int CurrentHealth { get; set; } = 100;

	public virtual void Damage(int damage)
	{
		if (damage <= 0) return;

		CurrentHealth -= damage;

		if (CurrentHealth <= 0)
		{
			OnDeath?.Invoke(this);
		}

	}

	public virtual void Heal(int Health)
	{
		if (Health <= 0) return;

		if (CurrentHealth >= MaxHealth)
		{
			CurrentHealth = MaxHealth;
			return;
		}

		CurrentHealth += Health;

	}

	public override void _ExitTree()
	{
		OnDeath = null;
	}

	public void HandleHealthHitBox_Entered(Area3D area)
	{
		if (area is Projectile projectile) GD.Print("Honey, I fixed it! It can now parse damage: " + projectile.Damage);
		else GD.Print("Bing");

		OnDeath?.Invoke(this);
	}
}
