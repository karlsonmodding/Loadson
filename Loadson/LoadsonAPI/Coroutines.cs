using System.Collections;
using UnityEngine;

namespace LoadsonAPI
{
    public class Coroutines
    {
        public static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return LoadsonInternal.Loader.MonoHooks.StartCoroutine(coroutine);
        }

        public static void StopCoroutine(Coroutine coroutine)
        {
            LoadsonInternal.Loader.MonoHooks.StopCoroutine(coroutine);
        }
    }
}
