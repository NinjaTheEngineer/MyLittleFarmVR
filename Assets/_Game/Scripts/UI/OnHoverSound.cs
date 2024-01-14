using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverSound : MonoBehaviour, IPointerEnterHandler {
    public void OnPointerEnter(PointerEventData eventData) {
        AudioManager.Instance.PlaySFX(SoundType.ButtonHover, transform.position);
    }
}
