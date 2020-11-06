/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using util;

namespace core.ui
{
    public class TimerUIComponent : UIComponent
    {
        private Coroutine _timerCoroutine;
        private float _normalizeTime;

        [Header("Timer UI Components")]
        [Space]
        public Slider timeSlider;
        public Text multiplierText;

        private void Start()
        {
            timeSlider.gameObject.SetActive(false);
            multiplierText.gameObject.SetActive(false);
        }

        public void StartTimer(float time, Action callback)
        {
            StopTimer();
            timeSlider.gameObject.SetActive(true);
            multiplierText.gameObject.SetActive(true);
            _timerCoroutine = StartCoroutine(StartTimerCoroutine(time, callback));
        }

        public void StopTimer(Action<bool> multiplierCallback = null)
        {
            timeSlider.gameObject.SetActive(false);
            multiplierText.gameObject.SetActive(false);
            if (_timerCoroutine != null)
            {
                GameDebug.Log($"Timer Sopped.", util.LogType.Thread);
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;

                if (multiplierCallback != null)
                {
                    bool hasMultiplier = _normalizeTime >= 0.4f && _normalizeTime <= 0.6f;
                    GameDebug.Log($"StopTimer ---> MULTIPLIER ZONE -> {hasMultiplier}", util.LogType.Thread);
                    multiplierCallback.Invoke(hasMultiplier);
                }
            }
        }

        private IEnumerator StartTimerCoroutine(float duration, Action callback)
        {
            GameDebug.Log($"Started Timer. Callback: {callback.Target}", util.LogType.Thread);

            _normalizeTime = 0;
            while (_normalizeTime <= 1f)
            {
                timeSlider.value = 1 - _normalizeTime;
                _normalizeTime += Time.deltaTime / duration;
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