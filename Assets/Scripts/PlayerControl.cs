using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 90f;
    // FIXME jumpDistance is not working as intended
    public float jumpDistance = 0.04f;
    public float gravity = 0.08f;

    /* 
     * InputSystem: https://youtu.be/HmXU4dZbaMw
     * https://learn.unity.com/tutorial/configuring-an-xbox-controller-for-user-input-2019-2
     */
    Vector2 moveDirection;
    Vector2 lookDirection;

    public GameObject cameralFocal, virtualCamera;
    CharacterController controller;
    PlayerStateManager stateManager;

    float yVelocity = 0f;

    private static Vector2 Rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    void Awake()
    {
        stateManager = GetComponent<PlayerStateManager>();
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 movement = Vector3.zero;
        switch (stateManager.state)
        {
            case PlayerState.Freemove:
                {
                    cameralFocal.transform.Rotate(Vector3.up * lookDirection.x * turnSpeed * Time.deltaTime);
                    float cameraYaw = -virtualCamera.transform.rotation.y * Mathf.PI;
                    Vector2 alignedMovement = Rotate(moveDirection, cameraYaw) * moveSpeed * Time.deltaTime;
                    movement.x = alignedMovement.x;
                    movement.z = alignedMovement.y;
                    break;
                }
            case PlayerState.Jumping:
                {
                    cameralFocal.transform.Rotate(Vector3.up * lookDirection.x * turnSpeed * Time.deltaTime);
                    float cameraYaw = -virtualCamera.transform.rotation.y * Mathf.PI;
                    Vector2 alignedMovement = Rotate(moveDirection, cameraYaw) * moveSpeed * Time.deltaTime / 2;
                    movement.x = alignedMovement.x;
                    movement.z = alignedMovement.y;
                    break;
                }
            case PlayerState.Ropewalk:
            {
                // TODO
                break;
            }
            default: break;
        }
        if (!controller.isGrounded)
        {
            yVelocity -= gravity * Time.deltaTime;
        }
        movement.y = yVelocity;
        controller.Move(movement);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookDirection = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // explain context.performed https://forum.unity.com/threads/player-input-component-triggering-events-multiple-times.851959/
        if (!context.performed) return;

        // TODO jumping via CharacterControlUtilities.StandardJump (requires KinematicCharacterBody?)
        // https://docs.unity3d.com/Packages/com.unity.charactercontroller@1.0/manual/jumping.html
        if (controller.isGrounded)
        {
            yVelocity = jumpDistance;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        switch (stateManager.interact)
        {
            case InteractAction.AttachRope:
                {
                    Debug.Log("Attaching to Rope");
                    GameObject startObject = stateManager.collidingRope;

                    LineRenderer lineRenderer = startObject.GetComponent<LineRenderer>();
                    Vector3[] lineIndexes = new Vector3[lineRenderer.positionCount];
                    lineRenderer.GetPositions(lineIndexes);
                    Vector3 newPlayerPos = new List<Vector3>(lineIndexes).OrderBy(point => Vector3.Distance(point, transform.position)).First();

                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, newPlayerPos - transform.position, out hit, Vector3.Magnitude(newPlayerPos - transform.position)))
                        break;

                    Debug.Log(newPlayerPos);

                    // https://forum.unity.com/threads/does-transform-position-work-on-a-charactercontroller.36149/#post-4132021
                    controller.enabled = false;
                    transform.position = newPlayerPos;
                    controller.enabled = true;
                    break;
                }
        }
    }
}
