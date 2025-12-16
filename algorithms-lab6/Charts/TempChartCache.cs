using System;
using System.Collections.Generic;
using System.IO;

namespace algorithms_lab6.Charts;

public static class TempChartCache {
    private static readonly object Lock = new();
    private static string? _dir;
    private static readonly Dictionary<string, string> Map = new(StringComparer.Ordinal);

    public static string RootDir {
        get {
            lock (Lock) {
                _dir ??= CreateDir();
                return _dir;
            }
        }
    }

    public static bool TryGet(string key, out string path) {
        lock (Lock) {
            return Map.TryGetValue(key, out path!);
        }
    }

    public static string GetOrCreate(string key, Func<string> make) {
        lock (Lock) {
            if (Map.TryGetValue(key, out var val)) {
                return val;
            }
        }

        var path = make();

        lock (Lock) {
            Map[key] = path;
        }

        return path;
    }

    public static void Cleanup() {
        string? dir;
        lock (Lock) {
            dir = _dir;
            _dir = null;
            Map.Clear();
        }

        if (dir == null) {
            return;
        }

        try {
            if (Directory.Exists(dir)) {
                Directory.Delete(dir, recursive: true);
            }
        } catch {
        }
    }

    private static string CreateDir() {
        var dir = Path.Combine(Path.GetTempPath(), "algorithms-lab6", "charts", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }
}
