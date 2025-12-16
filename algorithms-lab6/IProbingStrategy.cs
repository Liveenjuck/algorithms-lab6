namespace algorithms_lab6;

public interface IProbingStrategy<K> {
    int Index(K key, int i, int capacity);
}
