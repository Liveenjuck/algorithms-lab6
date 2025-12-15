using System;

namespace algorithms_lab6;

public sealed class MultiplicationHashStrategy : IHashStrategy<int> {
    // s = floor(A * 2^w) for w=32 and A ~= (sqrt(5)-1)/2 (Knuth)
    private const uint S = 2654435769u;

    public int Index(int key, int capacity) {
        if (capacity <= 0) {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        // r0 = low w bits of k*s
        var r0 = (uint)((ulong)(uint)key * S);

        // Generalization for arbitrary m:
        // floor(m * r0 / 2^w) == ((r0 * m) >> w)
        return (int)(((ulong)r0 * (ulong)capacity) >> 32);
    }
}
