using System;

namespace Engine.Utility.Enums;

public static class EnumExtensions {

    public static float GetAngle(this Rotation rot) {
        return (float)((int)rot * 90.0f);
    }

    public static Rotation GetOpposite(this Rotation rot) {
        return (Rotation)((((int)rot * 90) + 180) % 360 / 90);
    }

    public static Rotation RotateClockwise(this Rotation rot) {
        return (Rotation)((((int)rot * 90) + 90) % 360 / 90);
    }

    public static Rotation RotateCounterClockwise(this Rotation rot) {
        return (Rotation)((((int)rot * 90) + 270) % 360 / 90);
    }

    public static Rotation GetClosestRotation(float angle) {
        return (Rotation)((int)MathF.Round(angle % 360.0f / 90.0f) * 90 % 360 / 90);
    }


}
