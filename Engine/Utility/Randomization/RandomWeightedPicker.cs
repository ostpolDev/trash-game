using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Utility.Randomization;

// Author: Giovanni Costagliola <giovanni.costagliola@gmail.com>

/// <summary>
/// Utility class to randomly pick <see cref="IWeighted"/> items from a list
/// </summary>
public class RandomWeightedPicker<T> where T : IWeighted {

    private readonly IEnumerable<T> Items;
    private readonly int TotalWeight;
    public Random Random { get; private set; }

    /// <summary>
    /// Initiliaze the structure. O(1) or O(n) depending by the options, default O(n).
    /// </summary>
    /// <param name="items">The items</param>
    /// <param name="checkWeights">If <c>true</c> will check that the weights are positive. O(N)</param>
    /// <param name="shallowCopy">If <c>true</c> will copy the original collection structure (not the items). Keep in mind that items lifecycle is impacted.</param>
    public RandomWeightedPicker(IEnumerable<T> items, Random random, bool checkWeights = true, bool shallowCopy = true) {
        ArgumentNullException.ThrowIfNull(items, nameof(items));
        if (!items.Any()) throw new ArgumentException("items cannot be empty", nameof(items));

        Random = random;

        if (shallowCopy)
            Items = [..items];
        else
            Items = items;
        if (checkWeights && Items.Any(i => i.Weight <= 0)) {
            throw new ArgumentException("There exists some items with a non positive weight");
        }
        TotalWeight = Items.Sum(i => i.Weight);
    }
    /// <summary>
    /// Pick a random item based on its chance. O(n)
    /// </summary>
    /// <param name="defaultValue">The value returned in case the element has not been found</param>
    /// <returns></returns>
    public T PickAnItem() {
        int rnd = Random.Next(TotalWeight);
        return Items.First(i => (rnd -= i.Weight) < 0);
    }

    /// <summary>
    /// Resets the internal random generator. O(1)
    /// </summary>
    /// <param name="seed"></param>
    public void ResetRandomGenerator(int? seed) {
        Random = seed.HasValue ? new Random(seed.Value) : new Random();
    }


}
