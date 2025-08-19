using System.Collections;
using UnityEngine;

namespace Scripts.Utils
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator routine);
    }
}