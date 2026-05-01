using Godot;
using System;

public partial class PlayerRotation : Node3D
{
	[Export(PropertyHint.Range, "-1f,1f,")]
	public float RotationSpeed { get; set; } = 0.05f;
	[Export]
	public Node3D RotationTarget { get; set; }
	[Export]
	public Node3D OffsetTarget { get; set; }
	[Export]
	public float RotationDistance {get; set;} = 10f;
	[Export]
	public float RotationHeight {get; set;} = 10f;
	
	public override void _Process(double delta)
	{
		UpdateDistance();
		RotationTarget.Rotate(Vector3.Up, RotationSpeed * (float)delta);
	}

	public void UpdateDistance()
	{
		OffsetTarget.Position = new Vector3(RotationTarget.Position.X, RotationHeight, RotationDistance);
	}
}
