/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace core
{
    [RequireComponent(typeof(Image))]
    public class ClickableComponent : MonoBehaviour, IPointerDownHandler
    {
        public UnityEvent onPointerDown;
        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown?.Invoke();
        }
    }
}