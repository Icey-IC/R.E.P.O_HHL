using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [Header("��Ʒ����")]
    public float value = 100f;
    public float minImpactVelocity = 2f; // ���ڴ��ٶȲ��۷�
    public float damageMultiplier = 5f;  // ��ײ�ٶ� �� ��ֵ = �۳���ֵ
    public Camera playerCamera;

    [Header("UI ��ʾ")]
    public TMP_Text valueText;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UpdateValueDisplay();
    }

    void Update()
    {
        //ui�������
        Vector3 dirToCam = valueText.transform.position - playerCamera.transform.position;
        valueText.transform.position = transform.position + dirToCam * -0.5f;
        valueText.transform.rotation = Quaternion.LookRotation(dirToCam);
        if (value == 0)
            gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed > minImpactVelocity)
        {
            float damage = (impactSpeed - minImpactVelocity) * damageMultiplier;
            value -= damage;
            value = Mathf.Max(value, 0); // ������0
            UpdateValueDisplay();
        }
    }


    void UpdateValueDisplay()
    {
        if (valueText != null)
        {
            valueText.text = $"Value: {Mathf.RoundToInt(value)}";
        }
    }
}
