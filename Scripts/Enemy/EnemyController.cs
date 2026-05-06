using System;
using Godot;

public partial class EnemyController : CharacterBody3D
{
	[Export]
	public NavigationAgent3D NavigationAgent { get; set; }
	[Export]
	public EnemyHealth Health { get; set; }
	[Export]
	public CharacterBody3D Body { get; set; }
	[Export]
	public float MovementSpeed { get; set; } = 2.0f;

	public Vector3 MovementTarget
	{
		get { return NavigationAgent.TargetPosition; }
		set { NavigationAgent.TargetPosition = value; }
	}

	public override void _Ready()
	{
		Health.OnDeath += HandleDeath;
		base._Ready();
	}


	public override void _PhysicsProcess(double delta)
	{
		if (NavigationAgent.IsNavigationFinished())
		{
			return;
		}

		Vector3 currentAgentPosition = Body.GlobalTransform.Origin;
		Vector3 nextPathPosition = NavigationAgent.GetNextPathPosition();

		this.Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * MovementSpeed;
		Body.MoveAndSlide();
		Body.LookAt(Vector3.Zero);
	}

	public void HandleDeath(Health health)
	{
		health.OnDeath -= HandleDeath;
		QueueFree();
	}

}
