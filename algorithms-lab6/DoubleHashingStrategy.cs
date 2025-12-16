using System;

namespace algorithms_lab6;

public sealed class DoubleHashingStrategy<K> : IProbingStrategy<K> {
    private readonly IHashStrategy<K> _h1;
    private readonly IHashStrategy<K> _h2;

    public DoubleHashingStrategy(IHashStrategy<K> h1, IHashStrategy<K> h2) {
        if (h1 is null) {
            throw new ArgumentNullException(nameof(h1));
        }

        if (h2 is null) {
            throw new ArgumentNullException(nameof(h2));
        }

        _h1 = h1;
        _h2 = h2;
    }

    public int Index(K key, int i, int capacity) {
        if (capacity <= 0) {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        if (i < 0 || i >= capacity) {
            throw new ArgumentOutOfRangeException(nameof(i));
        }

        // h(k,i) = (h1(k) + i*h2(k)) mod m
        // To guarantee non-zero step: h2(k) = 1 + (h2'(k) mod (m-1)).
        var start = _h1.Index(key, capacity);

        if (capacity == 1) {
            return 0;
        }

        var step = 1 + _h2.Index(key, capacity - 1);
        return (int)((start + (long)i * step) % capacity);
    }
}
