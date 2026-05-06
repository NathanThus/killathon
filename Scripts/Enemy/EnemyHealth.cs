using Godot;
using System;

public partial class EnemyHealth : Health
{
    public override void HandleHealthHitBox_Entered(Area3D area)
    {
        if (area is Projectile projectile)
        {
            Damage(projectile.Damage);
        }
    }

}
