using Godot;
using System;

public partial class MouseZoom : Node
{
	[Export]
	public Camera3D playerCamera {get; set;}
	[Export]
	public int MaximumFov {get; set;} = 90;
	[Export]
	public int MinimumFov {get; set;} = 10;
	[Export]
	public int FovSensitivity {get; set;} = 2;
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			switch (mouseEvent.ButtonIndex)
			{
				case MouseButton.WheelUp:
				if(playerCamera.Fov <= MinimumFov) break;
				playerCamera.Fov -= FovSensitivity;
					break;
				case MouseButton.WheelDown:
				if(playerCamera.Fov >= MaximumFov) break;
				playerCamera.Fov += FovSensitivity;
					break;
				default:
					break;
			}
			
		}
	}

}
