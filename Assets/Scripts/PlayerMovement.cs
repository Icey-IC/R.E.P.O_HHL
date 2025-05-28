using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class playermovement : MonoBehaviour
{
    [Header("视角")]
    public float sensX = 1000f;
    public float sensY = 1000f;

    [Header("移动")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;
    //体力
    private int stamina;
    public int maxStamina;
    public TMP_Text staminaText;
    private bool isIncreasingStamina = true;
    Coroutine staminaCoroutine;

    [Header("跳跃")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("潜行")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("按键绑定")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("地面接触")]
    public float playerHeight;
    public LayerMask Ground;
    bool grounded;
    float originalHeight;

    public Transform PlayerCamera;

    float xRotation;
    float yRotation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    //玩家状态机,主要进行速度改变
    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        originalHeight = playerHeight;
        startYScale = transform.localScale.y;

        stamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        //第一人称视角
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        PlayerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //触地检测
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);

        MyInput();
        SpeedControl();
        StateHandler();
        staminaText.text = "Stamina: " + stamina;

        //地面摩擦力
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //跳跃
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //开始潜行
        if(Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            playerHeight = originalHeight * crouchYScale;
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        //停止潜行
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            playerHeight = originalHeight;
        }
    }

    private void StateHandler()
    {
        if (grounded && Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (grounded && Input.GetKey(sprintKey) && stamina > 0)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }


        if (Input.GetKey(sprintKey))
            StartStaminaChange(false); // 消耗
        else
            StartStaminaChange(true); // 恢复

    }

    
    private void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        //地面移动
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        //空中移动
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //手动限速
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        //重置y速度，防止二段跳过高
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
    private void StartStaminaChange(bool increase)
    {
        if (staminaCoroutine != null && increase == isIncreasingStamina)
            return; // 同方向正在进行中，不重复开启

        if (staminaCoroutine != null)
            StopCoroutine(staminaCoroutine);

        isIncreasingStamina = increase;
        staminaCoroutine = StartCoroutine(StaminaChange(increase));
    }
    private IEnumerator StaminaChange(bool increase)
    {
        while (true)
        {
            if (increase && stamina < maxStamina)
                stamina++;
            else if (!increase && stamina > 0)
                stamina--;

            yield return new WaitForSeconds(increase ? 1f : 0.2f);
        }

    }
}
