using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pick : MonoBehaviour
{
    //ʰȡ
    public Camera playerCamera;
    public float pickupRange = 5f;
    public float holdDistance = 3f;
    public float moveForce = 1000f;
    public LayerMask pickableLayer;

    private Rigidbody heldObject;
    private Transform holdPoint;

    private Quaternion holdRotationOffset;

    //����Զ��
    public float minHoldDistance = 1.5f;
    public float maxHoldDistance = 5f;
    public float scrollSpeed = 4f; // �����������ٶ�



    void Start()
    {
        holdPoint = new GameObject("HoldPoint").transform;
        holdPoint.SetParent(playerCamera.transform);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPickup();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            DropObject();
        }

        if (heldObject)
        {
            MoveHeldObject();
        }

        // ���ֵ�������
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            holdDistance += scroll * scrollSpeed;
            holdDistance = Mathf.Clamp(holdDistance, minHoldDistance, maxHoldDistance);
        }

        // ÿ֡���� holdPoint ��λ�ã�ʼ���������ǰ�� holdDistance��
        holdPoint.position = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;
    }

    void TryPickup()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickableLayer))
        {
            if (hit.rigidbody != null)
            {
                heldObject = hit.rigidbody;

                // ���� holdPoint ��λ�ú���תΪ����ĵ�ǰλ��
                holdPoint.position = heldObject.position;

                // ?? ��¼��ʼ��ת��ֵ
                holdRotationOffset = Quaternion.Inverse(playerCamera.transform.rotation) * heldObject.rotation;

                heldObject.useGravity = false;
                heldObject.drag = 10f;
            }
        }
    }

    void MoveHeldObject()
    {
        // λ��
        Vector3 direction = (holdPoint.position - heldObject.position);
        heldObject.velocity = direction * moveForce * Time.deltaTime;

        // ��ת�������������ӽ�һ��
        Quaternion targetRotation = playerCamera.transform.rotation * holdRotationOffset;
        heldObject.MoveRotation(Quaternion.Slerp(heldObject.rotation, targetRotation, Time.deltaTime * 10f));
    
        //������Զ��

    }

    void DropObject()
    {
        if (heldObject)
        {
            heldObject.useGravity = true;
            heldObject.drag = 0f;
            heldObject = null;

        }
    }
}
