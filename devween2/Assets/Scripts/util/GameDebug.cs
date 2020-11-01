/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using System.Collections.Generic;
using UnityEngine;

namespace util
{
    public class GameDebug : Singleton<GameDebug>
    {
        // ----------------------------------------------------------------------------------
        // ========================== Logging Methods ============================
        // ----------------------------------------------------------------------------------

        [SerializeField] private bool systemLogEnabled = true;

        public static void Log(string message, LogType logType = LogType.None)
        {
            if (!LogTypeEnabled(logType)) return;

            if (logType != LogType.None)
                message = $"[{logType.ToString().ToUpper()}] " + message;

            Debug.Log(message);
        }


        // ----------------------------------------------------------------------------------
        // ========================== Logging Checkers ============================
        // ----------------------------------------------------------------------------------

        [SerializeField] private List<LogType> logTypesEnabled;

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

        private static bool LogTypeEnabled(LogType logType)
        {
            return Instance.logTypesEnabled.Contains(logType);
        }

    }

    public enum LogType
    {
        None,
        Transition,
        Web
    }
}