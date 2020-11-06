/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using System;
using System.Collections;
using UnityEngine;
using util;

namespace core.ui
{
    public class TimerUIComponent : UIComponent
    {
        private Coroutine _timerCoroutine;

        public void StartTimer(float time, Action callback)
        {
            StopTimer();
            _timerCoroutine = StartCoroutine(StartTimerCoroutine(time, callback));
        }

        public void StopTimer()
        {
            if (_timerCoroutine != null)
            {
                GameDebug.Log($"Timer Sopped.", util.LogType.Thread);
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }

        private IEnumerator StartTimerCoroutine(float time, Action callback)
        {
            GameDebug.Log($"Started Timer. Callback: {callback.Target}", util.LogType.Thread);
            float duration = time;
            float normalizedTime = 0;
            while (normalizedTime <= 1f)
            {
                //countdownImage.fillAmount = normalizedTime;
                normalizedTime += Time.deltaTime / duration;
                yield return null;
            }

            GameDebug.Log($"Timer finished", util.LogType.Thread);
            if (callback != null)
            {
                GameDebug.Log($"Invoked Callback: {callback.Target}", util.LogType.Thread);
                callback.Invoke();
            }
        }
    }
}