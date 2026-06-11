namespace Engine.Utility.Randomization;

/// <summary>
/// Used to define randomization weight in <see cref="RandomWeightedPicker"/>
/// </summary>
public interface IWeighted {
    int Weight { get; }
}
