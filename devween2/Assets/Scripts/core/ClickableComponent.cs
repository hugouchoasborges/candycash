/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using util;

namespace core
{
    [RequireComponent(typeof(Image))]
    public class ClickableComponent : MonoBehaviour, IPointerDownHandler
    {
        public UnityEvent onPointerDown;
        public bool touchable
        {
            get => mImage.raycastTarget;
            set => mImage.raycastTarget = value;
        }

        protected Image mImage;

        private void Awake()
        {
            mImage = GetComponent<Image>();
            mImage.raycastTarget = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            GameDebug.Log($"Clicked: {gameObject.name}", util.LogType.Click);
            onPointerDown?.Invoke();
        }
    }
}