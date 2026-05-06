using Godot;
using System;

public partial class Projectile : Area3D
{
	[Export]
	public int Damage { get; set; } = 20;
}
