using Godot;
using System;

public partial class Health : Area3D
{
	public event Action<Health> OnDeath;
	public event Action OnHeal;
	public event Action OnDamage;

	[Export]
	public int MaxHealth { get; set; } = 100;
	[Export]
	public int CurrentHealth { get; set; } = 100;

	public virtual void Damage(int damage)
	{
		if (damage <= 0) return;

		CurrentHealth -= damage;
		OnDamage?.Invoke();

		if (CurrentHealth <= 0)
		{
			OnDeath?.Invoke(this);
		}

	}

	public virtual void Heal(int Health)
	{
		if (Health <= 0) return;

		OnHeal?.Invoke();
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

	public virtual void HandleHealthHitBox_Entered(Area3D area)
	{

	}
}
