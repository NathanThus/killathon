using Godot;
using System;

public partial class PlayerRotation : Node3D
{
	[Export(PropertyHint.Range, "-1f,1f,")]
	public float RotationSpeed {get; set;} = 0.05f;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.Rotate(Vector3.Up, RotationSpeed * (float)delta);
	}
}
