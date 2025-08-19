using System.Collections;
using UnityEngine;

namespace Scripts.Utils
{
    public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
    {
        public Coroutine StartCoroutine(IEnumerator routine) => base.StartCoroutine(routine);
    }
}