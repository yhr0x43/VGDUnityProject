using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 90f;
    public float airSpeedMultiplier = 0.5f;
    public float ledgeSpeedMultiplier = 0.5f;
    // FIXME jumpDistance is not working as intended
    public float jumpDistance = 0.04f;
    public float gravity = 0.08f;

    /* 
     * InputSystem: https://youtu.be/HmXU4dZbaMw
     * https://learn.unity.com/tutorial/configuring-an-xbox-controller-for-user-input-2019-2
     */
    Vector2 moveDirection;
    Vector2 lookDirection;

    Vector3 movement;

    public GameObject cameralFocal, virtualCamera;
    CharacterController controller;
    PlayerStateManager stateManager;

    Vector3[] lineIndexes;
    int currentIndex, nextIndex, prevIndex;

    float playerHeight;
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
        playerHeight = GetComponent<CapsuleCollider>().bounds.size.y;
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
        movement = Vector3.zero;
        switch (stateManager.state)
        {
            case PlayerState.Freemove:
                {
                    cameralFocal.transform.Rotate(Vector3.up * lookDirection.x * turnSpeed * Time.deltaTime);
                    float cameraYaw = -virtualCamera.transform.rotation.y * Mathf.PI;
                    Vector2 alignedMovement = Rotate(moveDirection, cameraYaw) * moveSpeed * Time.deltaTime;
                    movement.x = alignedMovement.x;
                    movement.z = alignedMovement.y;
                    
                    
                    //controller.transform.Rotate(Vector3.up * movement.x * (720f * Time.deltaTime));
                    playerRotation();

                    break;
                }
            case PlayerState.Jumping:
                {
                    cameralFocal.transform.Rotate(Vector3.up * lookDirection.x * turnSpeed * Time.deltaTime);
                    float cameraYaw = -virtualCamera.transform.rotation.y * Mathf.PI;
                    Vector2 alignedMovement = Rotate(moveDirection, cameraYaw) * moveSpeed * Time.deltaTime * airSpeedMultiplier;
                    movement.x = alignedMovement.x;
                    movement.z = alignedMovement.y;
                    break;
                }
            case PlayerState.Ropewalk:
                {
                    float forwardInput = moveDirection.x;
                    Vector3 currentLine = lineIndexes[nextIndex] - lineIndexes[prevIndex];
                    movement = transform.position - Vector3.up * playerHeight / 2;
                    if (forwardInput > 0)
                    {
                        // move player to position of next index in lineRenderer
                        transform.position = Vector3.MoveTowards(transform.position, lineIndexes[nextIndex], (moveSpeed * Time.deltaTime * forwardInput));
                    }

                    // while player is holding back, move towards previous vertice
                    if (forwardInput < 0)
                    {
                        // move player to position of next index in lineRenderer
                        transform.position = Vector3.MoveTowards(transform.position, lineIndexes[prevIndex], (moveSpeed * Time.deltaTime * -forwardInput));
                    }

                    // check if player is close enough to the nextIndex position
                    if (Vector3.Distance(transform.position, lineIndexes[nextIndex]) < 0.001f)
                    {
                        // change currentIndex, nextIndex, and prevIndex
                        currentIndex = nextIndex;

                        // if theres more rope to travel on going forwards
                        if (nextIndex < lineIndexes.Length - 1)
                        {
                            nextIndex = currentIndex + 1;
                        }
                        else
                        {
                            transform.position = Vector3.MoveTowards(transform.position, lineIndexes[prevIndex], (moveSpeed * Time.deltaTime * forwardInput));
                        }
                        // if theres more rope to travel on going backwards
                        if (prevIndex > 0)
                        {
                            prevIndex = currentIndex - 1;
                        }
                        else
                        {
                            transform.position = Vector3.MoveTowards(transform.position, lineIndexes[nextIndex], (moveSpeed * Time.deltaTime * forwardInput));
                        }
                    }
                    if (Vector3.Distance(transform.position, lineIndexes[prevIndex]) < 0.001f)
                    {
                        // change currentIndex, nextIndex, and prevIndex
                        currentIndex = prevIndex;

                        // if theres more rope to travel on going forwards
                        if (nextIndex < lineIndexes.Length - 1)
                        {
                            nextIndex = currentIndex + 1;
                        }
                        else
                        {
                            transform.position = Vector3.MoveTowards(transform.position, lineIndexes[prevIndex], (moveSpeed * Time.deltaTime * forwardInput));
                        }
                        // if theres more rope to travel on going backwards
                        if (prevIndex > 0)
                        {
                            prevIndex = currentIndex - 1;
                        }
                        else
                        {
                            transform.position = Vector3.MoveTowards(transform.position, lineIndexes[nextIndex], (moveSpeed * Time.deltaTime * forwardInput));
                        }
                    }
                    // transform.LookAt(endObject.transform);
                    break;
                }
            case PlayerState.LedgeWalk:
                {
                    // An invisible wall is used to prevent the player from falling off ledge, relevant code found in PlayerStateManager
                    cameralFocal.transform.Rotate(Vector3.up * lookDirection.x * turnSpeed * Time.deltaTime);
                    float cameraYaw = -virtualCamera.transform.rotation.y * Mathf.PI;
                    Vector2 alignedMovement = Rotate(moveDirection, cameraYaw) * moveSpeed * Time.deltaTime * ledgeSpeedMultiplier;
                    movement.x = alignedMovement.x;
                    movement.z = alignedMovement.y;
                    break;
                }
            default: break;
        }
        if (controller.enabled)
        {
            if (!controller.isGrounded)
            {
                yVelocity -= gravity * Time.deltaTime;
            }
            movement.y = yVelocity;
            controller.Move(movement);
        }
    }

    // rotates the player model based on input
    public void playerRotation()
    {
        Debug.Log(moveDirection);
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(movement.x, 0, movement.z);
        //movementDirection.Normalize();

        //transform.Translate(movementDirection * moveSpeed * Time.deltaTime, Space.World);
        
        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 360f * Time.deltaTime);            
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
                    LineRenderer lineRenderer = stateManager.collidingObject.GetComponent<LineRenderer>();
                    Vector3[] lineIndexes = new Vector3[lineRenderer.positionCount];
                    lineRenderer.GetPositions(lineIndexes);
                    Vector3 newPlayerPos = new List<Vector3>(lineIndexes).OrderBy(point => Vector3.Distance(point, transform.position)).First();

                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, newPlayerPos - transform.position, out hit, Vector3.Magnitude(newPlayerPos - transform.position)))
                        break;

                    currentIndex = Array.IndexOf(lineIndexes, newPlayerPos);
                    prevIndex = currentIndex - 1;
                    nextIndex = currentIndex + 1;

                    this.lineIndexes = lineIndexes;

                    stateManager.state = PlayerState.Ropewalk;

                    // https://forum.unity.com/threads/does-transform-position-work-on-a-charactercontroller.36149/#post-4132021
                    controller.enabled = false;
                    transform.position = newPlayerPos;
                    break;
                }
            case InteractAction.DetachRope:
                {
                    stateManager.state = PlayerState.Freemove;
                    lineIndexes = null;
                    controller.enabled = true;
                    break;
                }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other == GameObject.Find("Death box"))
        {
            transform.position = Vector3.zero;
        }
    }
}
