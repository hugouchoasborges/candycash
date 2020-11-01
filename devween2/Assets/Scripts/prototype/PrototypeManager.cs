/*
 * Create by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using util;

namespace prototype
{
    public class PrototypeManager : MonoBehaviour
    {
        [SerializeField] private Canvas _mainCanvas;

        public void LoadPrototype(int index)
        {
            GameDebug.Log($"Loading Prototype {index}...");
            _mainCanvas.enabled = false;
            SceneManager.LoadSceneAsync($"Prototype {index}", LoadSceneMode.Additive);
        }
    }
}