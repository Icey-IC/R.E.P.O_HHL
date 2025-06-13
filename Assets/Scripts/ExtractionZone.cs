using System.Collections.Generic;
using UnityEngine;
using TMPro;  // ʹ�� TextMeshPro

public class ExtractionZone : MonoBehaviour
{
    public float targetValue = 200f; // �ɹ������ܼ�ֵ
    private float currentValue = 0f;

    private HashSet<PickableItem> itemsInZone = new HashSet<PickableItem>();

    [Header("UI ��ʾ")]
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
            // �ɼ�������߼���������ء������ȣ�
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
