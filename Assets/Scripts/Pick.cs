using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pick : MonoBehaviour
{
    //拾取
    public Camera playerCamera;
    public float pickupRange = 5f;
    public float holdDistance = 3f;
    public float moveForce = 1000f;
    public LayerMask pickableLayer;

    private Rigidbody heldObject;
    private Transform holdPoint;

    private Quaternion holdRotationOffset;

    //靠近远离
    public float minHoldDistance = 1.5f;
    public float maxHoldDistance = 5f;
    public float scrollSpeed = 4f; // 鼠标滚轮缩放速度



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

        // 滚轮调整距离
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            holdDistance += scroll * scrollSpeed;
            holdDistance = Mathf.Clamp(holdDistance, minHoldDistance, maxHoldDistance);
        }

        // 每帧更新 holdPoint 的位置（始终在摄像机前方 holdDistance）
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

                // 设置 holdPoint 的位置和旋转为物体的当前位置
                holdPoint.position = heldObject.position;

                // ?? 记录初始旋转差值
                holdRotationOffset = Quaternion.Inverse(playerCamera.transform.rotation) * heldObject.rotation;

                heldObject.useGravity = false;
                heldObject.drag = 10f;
            }
        }
    }

    void MoveHeldObject()
    {
        // 位置
        Vector3 direction = (holdPoint.position - heldObject.position);
        heldObject.velocity = direction * moveForce * Time.deltaTime;

        // 旋转：保持相对玩家视角一致
        Quaternion targetRotation = playerCamera.transform.rotation * holdRotationOffset;
        heldObject.MoveRotation(Quaternion.Slerp(heldObject.rotation, targetRotation, Time.deltaTime * 10f));
    
        //靠近和远离

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
