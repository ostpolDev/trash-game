using Engine.Utility.Maths;

namespace Engine.UI;

public struct AspectRatio {

    public int TargetWidth;
    public int TargetHeight;

    public readonly string Aspect {
        get {
            return MathHelpers.GetAspectRatioWhole(TargetWidth, TargetHeight);
        }
    }

    public readonly float Ratio {
        get {
            return (float)TargetWidth / TargetHeight;
        }
    }

    public readonly bool IsSameRatioAs(int width, int height) {
        return ((float)width / height) == Ratio;
    }

    public override readonly string ToString() {
        return $"{TargetWidth}x{TargetHeight} ({Aspect})";
    }

}
