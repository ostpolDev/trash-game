using Engine.Utility.Maths;

namespace Engine.UI;

public struct AspectRatio(int width, int height) {

    public static readonly AspectRatio _16x9 = new(1280, 720);
    public static readonly AspectRatio _4x3 = new(800, 600);
    public static readonly AspectRatio _3x2 = new(1080, 720);

    public int TargetWidth = width;
    public int TargetHeight = height;

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
