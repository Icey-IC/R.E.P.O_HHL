using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [Header("物品属性")]
    public float value = 100f;
    public float minImpactVelocity = 2f; // 低于此速度不扣分
    public float damageMultiplier = 5f;  // 碰撞速度 × 该值 = 扣除数值
    public Camera playerCamera;

    [Header("UI 显示")]
    public TMP_Text valueText;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UpdateValueDisplay();
    }

    void Update()
    {
        //ui面向玩家
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
            value = Mathf.Max(value, 0); // 不低于0
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
