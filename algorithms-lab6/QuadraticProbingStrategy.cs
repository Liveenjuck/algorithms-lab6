using System;

namespace algorithms_lab6;

public sealed class QuadraticProbingStrategy<K> : IProbingStrategy<K> {
    private readonly IHashStrategy<K> _hash;
    private readonly int _c1;
    private readonly int _c2;

    public QuadraticProbingStrategy(IHashStrategy<K> hashStrategy, int c1, int c2) {
        if (hashStrategy is null) {
            throw new ArgumentNullException(nameof(hashStrategy));
        }

        if (c1 <= 0) {
            throw new ArgumentOutOfRangeException(nameof(c1));
        }

        if (c2 <= 0) {
            throw new ArgumentOutOfRangeException(nameof(c2));
        }

        _hash = hashStrategy;
        _c1 = c1;
        _c2 = c2;
    }

    public int Index(K key, int i, int capacity) {
        if (capacity <= 0) {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        if (i < 0 || i >= capacity) {
            throw new ArgumentOutOfRangeException(nameof(i));
        }

        // h(k,i) = (h'(k) + c1*i + c2*i^2) mod m
        var baseIdx = _hash.Index(key, capacity);
        var offset = (long)_c1 * i + (long)_c2 * i * i;
        return (int)((baseIdx + offset) % capacity);
    }
}
