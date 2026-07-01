namespace Engine.Utility.Maths;

public class MathHelpers {

    /// <summary>
    /// Finds the greatest common divider from two numbers
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static ulong GCD(ulong a, ulong b) {
        while (a != 0 && b != 0) {
            if (a > b)
                a %= b;
            else
                b %= a;
        }

        return a | b;
    }

    public static string GetAspectRatioWhole(int width, int height) {
        float gcd = GCD((ulong)width, (ulong)height);

        float w = width / gcd;
        float h = height / gcd;

        return $"{w}:{h}";
    }

}
