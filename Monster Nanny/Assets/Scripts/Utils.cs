using System.Collections;
using UnityEngine;

public static class Utils {
    public static void ShuffleArray<T> (T[] array) {
        System.Array.Sort(array, RandomSort<T>);
    }

    private static int RandomSort<T>(T a, T b){
        return Random.Range(-1, 2);
    }
}
