/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using UnityEngine;
using UnityEngine.EventSystems;

namespace core
{
    public class ClickableComponent : MonoBehaviour, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log($"Item Selected: {gameObject.name}");
        }
    }
}