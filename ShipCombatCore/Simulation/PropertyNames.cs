using System.Numerics;
using Myre;
using ShipCombatCore.Simulation.Behaviours;
using ShipCombatCore.Simulation.Entities;

namespace ShipCombatCore.Simulation
{
    public static class PropertyNames
    {
        public static readonly TypedName<float> Mass = new("mass");
        public static readonly TypedName<float> ExtraMass = new("extra_mass");

        public static readonly TypedName<Vector3> Force = new("force");
        public static readonly TypedName<Vector3> Torque = new("torque");
        public static readonly TypedName<Vector3> Position = new("position");
        public static readonly TypedName<Vector3> Velocity = new("velocity");

        public static readonly TypedName<Quaternion> Orientation = new("orientation");
        public static readonly TypedName<Vector3> AngularVelocity = new("angular_velocity");

        public static readonly TypedName<YololContext> YololContext = new("yolol_context");
        public static readonly TypedName<bool> RunningLightState = new("running_light_state");
        public static readonly TypedName<float> CosmicRadiation = new("cosmic_radiation");

        public static readonly TypedName<Vector3> RadarDirection = new("radar_direction");
        public static readonly TypedName<float> RadarRange = new("radar_range");
        public static readonly TypedName<float> RadarAngle = new("radar_angle");
        public static readonly TypedName<Vector3> RadarTarget = new("radar_target");

        public static readonly TypedName<float> FuelLitersInTank = new("fuel_quantity");
        public static readonly TypedName<float> FuelConsumptionRate = new("fuel_consumption");
        public static readonly TypedName<float> MaxEngineForce = new("max_engine_force");
        public static readonly TypedName<float> MaxWheelTorque = new("max_wheel_torque");
        public static readonly TypedName<float> ActualThrottle = new("actual_throttle");

        public static readonly TypedName<float> Lifetime = new("remaining_lifetime");

        public static readonly TypedName<string> UniqueName = new("unique_name");
        public static readonly TypedName<EntityType> EntityType = new("entity_type");
        public static readonly TypedName<uint> TeamOwner = new("entity_team");
        public static readonly TypedName<string> TeamName = new("entity_team_name");

        public static readonly TypedName<float> DamageSphereAmount = new("damage_sphere_amount");
        public static readonly TypedName<float> DamageSphereRange = new("damage_sphere_range");

        public static readonly TypedName<uint> MissileLauncherAmmo = new("missile_launcher_ammo");

        public static readonly TypedName<float> SphereRadius = new("sphere_collider_radius");

        public static readonly TypedName<Vector3> DebugSpherePosition = new("debug_sphere_position");
        public static readonly TypedName<float> DebugSphereRadius = new("debug_sphere_radius");
        public static readonly TypedName<Vector3> DebugSphereColor = new("debug_sphere_color");
    }
}
