using System;

namespace algorithms_lab6;

public sealed class DivisionHashStrategy : IHashStrategy<int> {
    public int Index(int key, int capacity) {
        if (capacity <= 0) {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        // h(k) = k mod m, normalized to [0..m)
        var idx = key % capacity;
        if (idx < 0) {
            idx += capacity;
        }

        return idx;
    }
}
