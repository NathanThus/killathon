using Godot;
using System;

public partial class PlayerController : Node3D
{

    /// <summary>The point in the world the camera orbits around.</summary>
    [Export] public Node3D Target;

    /// <summary>Mouse sensitivity in radians per pixel.</summary>
    [Export] public float Sensitivity = 0.005f;

    /// <summary>Maximum vertical angle above the horizon (degrees).</summary>
    [Export] public float VerticalMaxDeg  =  89.0f;

    /// <summary>Maximum vertical angle below the horizon (degrees).</summary>
    [Export] public float VerticalMinDeg  = -89.0f;

    /// <summary>
    /// Total horizontal rotation is clamped to ±90 ° (i.e. 180 ° arc).
    /// Set to 0 to allow free horizontal spin.
    /// </summary>
    [Export] public float HorizontalClampDeg = 90.0f;

    private float _yaw;    // horizontal accumulated rotation (radians)
    private float _pitch;  // vertical  accumulated rotation (radians)

    private bool _capturing = false;

    public override void _Ready()
    {
        // Initialise yaw / pitch from whatever rotation the node already has,
        // so the camera doesn't snap on the first frame.
        Vector3 euler = RotationDegrees;
        _pitch = Mathf.DegToRad(euler.X);
        _yaw   = Mathf.DegToRad(euler.Y);

        SetCapture(true);
    }

	// DEBUG
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Escape)
        {
            SetCapture(false);
            return;
        }

        if (@event is InputEventMouseButton mouseBtn
            && mouseBtn.ButtonIndex == MouseButton.Left
            && mouseBtn.Pressed)
        {
            SetCapture(true);
            return;
        }

        if (!_capturing) return;

        if (@event is InputEventMouseMotion motion)
        {
            ApplyMouseDelta(motion.Relative);
        }
    }

    // public override void _Process(double delta)
    // {
    //     if (Target == null) return;

    //     // LookAt(Target.GlobalPosition, Vector3.Up);
    // }

    /// <summary>Accumulate mouse movement into yaw / pitch with clamping.</summary>
    private void ApplyMouseDelta(Vector2 delta)
    {
        _yaw   -= delta.X * Sensitivity;
        _pitch -= delta.Y * Sensitivity;

        float pitchMin = Mathf.DegToRad(VerticalMinDeg);
        float pitchMax = Mathf.DegToRad(VerticalMaxDeg);
        _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);

        if (HorizontalClampDeg > 0.0f)
        {
            float yawLimit = Mathf.DegToRad(HorizontalClampDeg);
            _yaw = Mathf.Clamp(_yaw, -yawLimit, yawLimit);
        }

        Rotation = new Vector3(_pitch, _yaw, 0f);
    }

    /// <summary>Enable or disable mouse capture.</summary>
    private void SetCapture(bool capture)
    {
        _capturing = capture;
        Input.MouseMode = capture
            ? Input.MouseModeEnum.Captured
            : Input.MouseModeEnum.Visible;
    }
}
