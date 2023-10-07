using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float moveForce = 20f;
    public float turnSpeed = 60f;
    public float jumpForce = 50f;

    /* 
     * InputSystem: https://youtu.be/HmXU4dZbaMw
     * https://learn.unity.com/tutorial/configuring-an-xbox-controller-for-user-input-2019-2
     */
    Vector2 moveDirection;
    Vector2 lookDirection;

    public GameObject cameralFocal;
    public GameObject virtualCamera;
    Rigidbody rb;

    /* https://discussions.unity.com/t/how-do-i-check-if-my-rigidbody-player-is-grounded/33250/11 */
    bool isOnGround;

    private void OnCollisionEnter(Collision collision)
    {
        // NOTE consider downward facing collision as "touching ground" is this correct?
        if (!isOnGround)
        {
            isOnGround = (from contact in collision.contacts select contact.normal.y).Any(y => y > 0f);
        }
    }

    private void Awake()
    {

    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        cameralFocal.transform.Rotate(Vector3.up * Time.deltaTime * lookDirection.x * turnSpeed);
    }

    private void FixedUpdate()
    {
        if (moveDirection.sqrMagnitude > 0.0625f)
        {
            /* FIXME jittering when moving while camera rotating */
            float cameraYaw = virtualCamera.transform.rotation.y * 180;
            Vector3 transformedMove = (Quaternion.Euler(0, cameraYaw, 0) * new Vector3(moveDirection.x, 0, moveDirection.y)).normalized;
            rb.AddRelativeForce(transformedMove * moveForce);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookDirection = context.ReadValue<Vector2>();
    }

    /* https://gamedevbeginner.com/how-to-jump-in-unity-with-or-without-physics/ */
    public void OnJump(InputAction.CallbackContext context)
    {
        if (isOnGround)
        {
            /* TODO consider OnCollisionExit */
            isOnGround = false;
            rb.AddForce(Vector3.up * jumpForce);
        }
    }
}
