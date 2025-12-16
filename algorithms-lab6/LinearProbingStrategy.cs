using System;

namespace algorithms_lab6;

public sealed class LinearProbingStrategy<K> : IProbingStrategy<K> {
    private readonly IHashStrategy<K> _hash;

    public LinearProbingStrategy(IHashStrategy<K> hashStrategy) {
        if (hashStrategy is null) {
            throw new ArgumentNullException(nameof(hashStrategy));
        }

        _hash = hashStrategy;
    }

    public int Index(K key, int i, int capacity) {
        if (capacity <= 0) {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        if (i < 0 || i >= capacity) {
            throw new ArgumentOutOfRangeException(nameof(i));
        }

        // h(k,i) = (h'(k) + i) mod m
        var baseIdx = _hash.Index(key, capacity);
        return (baseIdx + i) % capacity;
    }
}
