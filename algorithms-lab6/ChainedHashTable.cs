using System;
using System.Collections.Generic;

namespace algorithms_lab6;

public class ChainedHashTable<K, V> {
    private readonly IHashStrategy<K> _hash;
    private Node?[] _buckets;

    public int Count { get; private set; }
    public int Capacity => _buckets.Length;
    public double LoadFactor => (double)Count / Capacity;
    public double MaxLoadFactor { get; }

    private sealed class Node {
        public K Key { get; }
        public V Value { get; set; }
        public Node? Next { get; set; }

        public Node(K key, V value, Node? next) {
            Key = key;
            Value = value;
            Next = next;
        }
    }

    public ChainedHashTable(IHashStrategy<K> hashStrategy, int capacity = 16, double maxLoadFactor = 0.75) {
        if (hashStrategy is null) {
            throw new ArgumentNullException(nameof(hashStrategy));
        }

        if (capacity <= 0) {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        if (maxLoadFactor <= 0 || maxLoadFactor >= 1) {
            throw new ArgumentOutOfRangeException(nameof(maxLoadFactor));
        }

        _hash = hashStrategy;
        _buckets = new Node?[capacity];
        MaxLoadFactor = maxLoadFactor;
    }

    public void AddOrUpdate(K key, V value) {
        if (NeedsResize(Count + 1)) {
            Resize(Capacity * 2);
        }

        var idx = _hash.Index(key, Capacity);
        var current = _buckets[idx];

        while (current != null) {
            if (EqualityComparer<K>.Default.Equals(current.Key, key)) {
                current.Value = value;
                return;
            }

            current = current.Next;
        }

        _buckets[idx] = new Node(key, value, _buckets[idx]);
        Count++;
    }

    public bool TryGetValue(K key, out V value) {
        var idx = _hash.Index(key, Capacity);
        var current = _buckets[idx];

        while (current != null) {
            if (EqualityComparer<K>.Default.Equals(current.Key, key)) {
                value = current.Value;
                return true;
            }

            current = current.Next;
        }

        value = default!;
        return false;
    }

    public bool Search(K key, out V value, out int comparisons) {
        comparisons = 0;

        var idx = _hash.Index(key, Capacity);
        var current = _buckets[idx];

        while (current != null) {
            comparisons++;

            if (EqualityComparer<K>.Default.Equals(current.Key, key)) {
                value = current.Value;
                return true;
            }

            current = current.Next;
        }

        value = default!;
        return false;
    }

    public void GetChainLengthStats(bool ignoreEmpty, out int min, out int max) {
        if (Count == 0) {
            min = 0;
            max = 0;
            return;
        }

        var minAll = int.MaxValue;
        int? minNonEmpty = null;
        max = 0;

        for (var i = 0; i < _buckets.Length; i++) {
            var length = 0;
            var current = _buckets[i];

            while (current != null) {
                length++;
                current = current.Next;
            }

            if (length > max) {
                max = length;
            }

            if (!ignoreEmpty) {
                if (length < minAll) {
                    minAll = length;
                }

                continue;
            }

            if (length == 0) {
                continue;
            }

            if (minNonEmpty == null || length < minNonEmpty.Value) {
                minNonEmpty = length;
            }
        }

        min = ignoreEmpty ? (minNonEmpty ?? 0) : (minAll == int.MaxValue ? 0 : minAll);
    }

    public bool Remove(K key) {
        var idx = _hash.Index(key, Capacity);

        Node? previous = null;
        var current = _buckets[idx];

        while (current != null) {
            if (EqualityComparer<K>.Default.Equals(current.Key, key)) {
                if (previous == null) {
                    _buckets[idx] = current.Next;
                }

                if (previous != null) {
                    previous.Next = current.Next;
                }

                Count--;
                return true;
            }

            previous = current;
            current = current.Next;
        }

        return false;
    }

    private bool NeedsResize(int newCount) {
        return (double)newCount / Capacity > MaxLoadFactor;
    }

    private void Resize(int newCapacity) {
        if (newCapacity <= 0) {
            throw new ArgumentOutOfRangeException(nameof(newCapacity));
        }

        var old = _buckets;
        _buckets = new Node?[newCapacity];
        Count = 0;

        for (var i = 0; i < old.Length; i++) {
            var current = old[i];
            while (current != null) {
                InsertRehashed(current.Key, current.Value);
                current = current.Next;
            }
        }
    }

    private void InsertRehashed(K key, V value) {
        var idx = _hash.Index(key, Capacity);
        _buckets[idx] = new Node(key, value, _buckets[idx]);
        Count++;
    }
}
