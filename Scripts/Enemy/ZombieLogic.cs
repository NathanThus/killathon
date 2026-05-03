using Godot;

public partial class ZombieLogic : CharacterBody3D
{
    [Export]
    public NavigationAgent3D _navigationAgent {get; set;}
    public float _movementSpeed { get; set; } = 2.0f;

    public Vector3 MovementTarget
    {
        get { return _navigationAgent.TargetPosition; }
        set { _navigationAgent.TargetPosition = value; }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_navigationAgent.IsNavigationFinished())
        {
            // Send Alert
            QueueFree();
        }

        Vector3 currentAgentPosition = GlobalTransform.Origin;
        Vector3 nextPathPosition = _navigationAgent.GetNextPathPosition();

        this.Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * _movementSpeed;
        MoveAndSlide();
    }

}