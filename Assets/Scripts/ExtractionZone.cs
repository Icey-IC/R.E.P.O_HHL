using System.Collections.Generic;
using UnityEngine;
using TMPro;  // 使用 TextMeshPro

public class ExtractionZone : MonoBehaviour
{
    public float targetValue = 200f; // 成功所需总价值
    private float currentValue = 0f;

    private HashSet<PickableItem> itemsInZone = new HashSet<PickableItem>();

    [Header("UI 显示")]
    public TMP_Text valueRemainingText;

    void Start()
    {
        UpdateValueDisplay();
    }

    void OnTriggerEnter(Collider other)
    {
        PickableItem item = other.GetComponent<PickableItem>();
        if (item != null && !itemsInZone.Contains(item))
        {
            itemsInZone.Add(item);
            currentValue += item.value;
            UpdateValueDisplay();
            CheckSuccess();
        }
    }

    void OnTriggerExit(Collider other)
    {
        PickableItem item = other.GetComponent<PickableItem>();
        if (item != null && itemsInZone.Contains(item))
        {
            itemsInZone.Remove(item);
            currentValue -= item.value;
            UpdateValueDisplay();
        }
    }

    void CheckSuccess()
    {
        if (currentValue >= targetValue)
        {
            Debug.Log("?? Extraction Successful! Total Value: " + currentValue);
            // 可加入额外逻辑（例如过关、弹窗等）
        }
    }

    void UpdateValueDisplay()
    {
        if (valueRemainingText != null)
        {
            float remaining = Mathf.Max(targetValue - currentValue, 0);
            valueRemainingText.text = $" {Mathf.RoundToInt(remaining)}$";
        }
    }
}
