/*
 * Create by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using UnityEngine;

namespace util
{
    public class GameDebug : Singleton<GameDebug>
    {
        // ----------------------------------------------------------------------------------
        // ========================== System Log ============================
        // ----------------------------------------------------------------------------------

        [SerializeField] private bool systemLogEnabled = true;

        private void Awake()
        {
            updateLogConf();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            updateLogConf();
        }
#endif

        private void updateLogConf()
        {
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = systemLogEnabled;
#else
            Debug.unityLogger.logEnabled = false;
#endif
        }


        // ----------------------------------------------------------------------------------
        // ========================== Logging Methods ============================
        // ----------------------------------------------------------------------------------

        [SerializeField] private bool logEnabled = true;

        public static void Log(string message)
        {
            if (Instance.logEnabled) Debug.Log(message);
        }

    }
}