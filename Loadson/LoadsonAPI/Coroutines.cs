using System.Collections;
using UnityEngine;

namespace LoadsonAPI
{
    public class Coroutines
    {
        /// <summary>
        /// Start a coroutine. Equivalent of <see cref="MonoBehaviour.StartCoroutine(IEnumerator)"/>
        /// </summary>
        /// <param name="coroutine">The coroutine</param>
        /// <returns>The started coroutine object</returns>
        public static Coroutine StartCoroutine(IEnumerator coroutine)
        {
#if !LoadsonAPI
            return LoadsonInternal.Loader.MonoHooks.StartCoroutine(coroutine);
#else
            return null;
#endif
        }

        /// <summary>
        /// Stop a running coroutine. Equivalent of <see cref="MonoBehaviour.StopCoroutine(Coroutine)"/>
        /// </summary>
        /// <param name="coroutine">The started coroutine</param>
        public static void StopCoroutine(Coroutine coroutine)
        {
#if !LoadsonAPI
            LoadsonInternal.Loader.MonoHooks.StopCoroutine(coroutine);
#endif
        }
    }
}
