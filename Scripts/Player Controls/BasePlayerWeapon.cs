using Godot;
using System;

public partial class BasePlayerWeapon : Node3D
{
	#region Nested

	public enum ArcStyle
	{
		MinimalTrajectory,
		MaximumTrajectory,
		PreciseFlightTime
	}

	#endregion

	#region Serialized Fields
	[Export]
	public PackedScene Projectile { get; set; }

	[Export] public Node3D _originTransform { get; set; }
	[Export] public float _maximumVelocity { get; set; } = 30f;
	[Export] public ArcStyle _style { get; set; } = ArcStyle.MinimalTrajectory;
	[Export] public float _heightMax { get; set; } = 30f;
	[Export] public float _minimumFlightTime { get; set; } = 1f;
	[Export] public float _desiredFlightTime { get; set; } = 2f;
	[Export] public float _maximumFlightTime { get; set; } = 5f;
	[Export] public Vector3 _windAcceleration { get; set; } = Vector3.Zero;
	[Export] public RayCast3D _rayCast {get; set;}

	#endregion

	#region Properties

	public Vector3 SpawnPosition => _originTransform.GlobalPosition;
	public bool ShowArcInEditor { get => showArcInEditor; set => showArcInEditor = value; }

    #endregion

    #region Fields

    private Vector3 _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity_vector").AsVector3() * (float)ProjectSettings.GetSetting("physics/3d/default_gravity").AsDouble();
	private float _gravityMagnitudeSquared = 0f;
	private Vector3 _gravityAcceleration = Vector3.Zero;
	private bool showArcInEditor = true;

	#endregion

	#region Start
	public override void _Ready()
	{
		_gravityMagnitudeSquared = _gravity.LengthSquared();
		_gravityAcceleration = _gravity * -1;
		_rayCast.Enabled = false;
		if (_originTransform == null) throw new NullReferenceException(nameof(_originTransform));
	}

 	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			switch (mouseEvent.ButtonIndex)
			{
				case MouseButton.Left:
					_rayCast.ForceRaycastUpdate();
					if(!_rayCast.IsColliding()) return;
					var targetLocation = _rayCast.GetCollisionPoint();
					Vector3 velocity = CalculateVelocity(targetLocation);
					var instance = Projectile.Instantiate<Projectile>();
					instance.Position = _originTransform.GlobalPosition;
					GetTree().Root.AddChild(instance);
					instance.LinearVelocity = velocity;
					break;
				default:
					break;
			}
		}
	}

	#endregion

	#region Public

	/// <summary>
	/// Calculate the velocity to hit a target.
	/// </summary>
	/// <param name="target">The global position of the target.</param>
	/// <returns>The velocity required to hit the target</returns>
	public Vector3 CalculateVelocity(Vector3 target)
	{
		Vector3 deltaPosition = target - SpawnPosition;
		float distance = deltaPosition.Length();

		float discriminant = Mathf.Pow(_maximumVelocity, 4) - _gravityMagnitudeSquared * Mathf.Pow(distance, 2);
		if (discriminant < 0) return Vector3.Zero; // NO POSSIBLE TRAJECTORIES

		return CalculateLaunchVelocity(deltaPosition, FlightTime(discriminant));
	}

	/// <summary>
	/// Calculate the velocity to hit a target.
	/// </summary>
	/// <param name="target">The global position of the target.</param>
	/// <param name="velocity">The target's velocity.</param>
	/// <returns>The velocity required to hit the target</returns>
	public Vector3 CalculateVelocity(Vector3 target, Vector3 velocity)
	{
		Vector3 deltaPosition = target - SpawnPosition + velocity * _desiredFlightTime;
		float distance = deltaPosition.Length();

		float discriminant = Mathf.Pow(_maximumVelocity, 4) - _gravityMagnitudeSquared * Mathf.Pow(distance, 2);
		if (discriminant < 0) return Vector3.Zero; // NO POSSIBLE TRAJECTORIES

		return CalculateLaunchVelocity(deltaPosition, FlightTime(discriminant));
	}

	/// <summary>
	/// Calculates the maximum range the system can hit, with the given maximum velocity.
	/// </summary>
	/// <returns>The maximum distance in standard Unity Units.</returns>
	public float CalculateRange()
	{
		float maxDistance = _maximumVelocity * _maximumVelocity / _gravity.Length();
		return maxDistance;
	}

	/// <summary>
	/// Set the windspeed variable for CalculateVelocity
	/// </summary>
	/// <param name="windX">The windspeed in the X direction.</param>
	/// <param name="windZ">The windspeed in the Z direction.</param>
	public void SetEnviromentalParameters(float windX, float windZ)
	{
		_windAcceleration = new Vector3(windX, 0, windZ);
	}

	/// <summary>
	/// Calculates the position of the projectile at the a given point in time.
	/// </summary>
	/// <param name="origin">The origin position.</param>
	/// <param name="velocity">The velocity of the projectile.</param>
	/// <param name="time">The time post launch.</param>
	/// <returns>Calculates the position of the projectile, at a given time during flight.</returns>
	public Vector3 CalculatePositionAtTime(Vector3 origin, Vector3 velocity, float time)
	{
		// Kinematic equation: position = origin + velocity * t + 0.5 * t^2 * gravity 
		return origin + velocity * time + 0.5f * Mathf.Pow(time, 2) * GetEnvironmentalAcceleration();
	}

	#endregion

	#region Private

	private Vector3 CalculateLaunchVelocity(Vector3 deltaPosition, float flightTime)
	{
		return deltaPosition / flightTime + GetEnvironmentalAcceleration() * (flightTime / 2.0f);
	}

	private float GetMaximumFlightTime(float velocitySquared, float sqrtDiscriminant)
	{
		float t = Mathf.Sqrt((velocitySquared + sqrtDiscriminant) / _gravityMagnitudeSquared);
		t = Mathf.Min(t, _maximumFlightTime);
		return t;
	}

	private float GetMinimumFlightTime(float velocitySquared, float sqrtDiscriminant)
	{
		float t = Mathf.Sqrt((velocitySquared - sqrtDiscriminant) / _gravityMagnitudeSquared);
		t = Mathf.Max(t, _minimumFlightTime);
		return t;
	}

	private float FlightTime(float discriminant)
	{
		return _style switch
		{
			ArcStyle.MinimalTrajectory => GetMinimumFlightTime(Mathf.Pow(_maximumVelocity, 2),
															   Mathf.Sqrt(discriminant)),
			ArcStyle.MaximumTrajectory => GetMaximumFlightTime(Mathf.Pow(_maximumVelocity, 2),
															   Mathf.Sqrt(discriminant)),
			ArcStyle.PreciseFlightTime => _desiredFlightTime,
			_ => throw new NotImplementedException()
		};
	}

	private Vector3 GetEnvironmentalAcceleration()
	{
		return new(_windAcceleration.X, _gravityAcceleration.Y, _windAcceleration.Z);
	}

	#endregion
}
