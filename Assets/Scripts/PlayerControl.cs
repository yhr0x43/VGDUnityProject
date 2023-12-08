using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 90f;
    public float ropeWalkSpeedMultiplier = 0.4f;
    public float airSpeedMultiplier = 0.5f;
    public float ledgeSpeedMultiplier = 0.5f;
    // FIXME jumpDistance is not working as intended
    public float jumpDistance = 0.04f;
    public float gravity = 0.08f;

    int jumpCounter = 1;

    int nextIndex, prevIndex;
    Vector3[] lineIndexes;

    float yVelocity = 0f;

    /* 
     * InputSystem: https://youtu.be/HmXU4dZbaMw
     * https://learn.unity.com/tutorial/configuring-an-xbox-controller-for-user-input-2019-2
     */
    Vector2 moveDirection;
    Vector2 lookDirection;

    Vector3 movement;
    PauseGame pauseGame;

    public GameObject cameralFocal, virtualCamera;
    CharacterController controller;
    Animator anim;
    PlayerStateManager stateManager;

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
        pauseGame = GetComponent<PauseGame>();
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
        anim = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        // for getting input from the controller, see OnMove and OnLook method of this class, they update
        // class members moveDirection and lookDirection as a callback of Player Input component
        // These two Vector2 should be input of the sticks where x component is horizontal and y is vertical, values between -1 and 1
        movement = Vector3.zero;
        float cameraYaw = -cameralFocal.transform.eulerAngles.y * Mathf.PI / 180;
        Vector2 alignedMovement = Rotate(moveDirection, cameraYaw) * Time.deltaTime * moveSpeed;

        switch (stateManager.state)
        {
            case PlayerState.Freemove:
                {   
                    // adjust the camera direction using the right analog stick
                    cameralFocal.transform.Rotate(Vector3.up * lookDirection.x * turnSpeed * Time.deltaTime);

                    // move the player forward
                    movement.x = alignedMovement.x;
                    movement.z = alignedMovement.y;
                    
                    playerRotation(movement);

                    anim.SetFloat("PlayerVelocity", controller.velocity.magnitude);
                    break;
                }
            case PlayerState.Ropewalk:
                {
                    Vector3 lineSegment = (lineIndexes[nextIndex] - lineIndexes[prevIndex]).normalized;
                    float forwardInput = Vector3.Dot(lineSegment, new Vector3(alignedMovement.x, 0, alignedMovement.y)) * ropeWalkSpeedMultiplier;
                    movement = lineSegment * forwardInput;

                    playerRotation(movement);

                    anim.SetFloat("PlayerVelocity", controller.velocity.magnitude);
                    anim.SetBool("IsGrounded", true);

                    if (Vector3.Distance(lineIndexes[prevIndex], lineIndexes[nextIndex]) < Vector3.Distance(lineIndexes[prevIndex], transform.position))
                    {
                        if (nextIndex + 1 < lineIndexes.Length)
                        {
                            nextIndex += 1;
                            prevIndex += 1;
                        }
                        else
                        {
                            stateManager.state = PlayerState.Freemove;
                            lineIndexes = null;
                        }
                    }
                    else if (Vector3.Distance(lineIndexes[prevIndex], lineIndexes[nextIndex]) < Vector3.Distance(lineIndexes[nextIndex], transform.position))
                    {
                        if (prevIndex > 0)
                        {
                            nextIndex -= 1;
                            prevIndex -= 1;
                        }
                        else
                        {
                            stateManager.state = PlayerState.Freemove;
                            lineIndexes = null;
                        }
                    }
                    break;
                }
            case PlayerState.LedgeWalk:
                {
                    // An invisible wall is used to prevent the player from falling off ledge, relevant code found in PlayerStateManager
                    cameralFocal.transform.Rotate(Vector3.up * lookDirection.x * turnSpeed * Time.deltaTime);
                    alignedMovement *= ledgeSpeedMultiplier;
                    movement.x = alignedMovement.x;
                    movement.z = alignedMovement.y;

                    playerRotation(movement);
                    anim.SetFloat("PlayerVelocity", controller.velocity.magnitude);
                    break;
                }
            default: break;
        }
        if (stateManager.state != PlayerState.Ropewalk)
        {
            if (!controller.isGrounded)
            {
                yVelocity -= gravity * Time.deltaTime;
                movement *= airSpeedMultiplier;
            }
            else
            {
                jumpCounter = 1;
            }
            anim.SetBool("IsGrounded", controller.isGrounded);
            movement.y = yVelocity;
        }
        
        if(pauseGame.isGamePaused == false)
        {
            controller.Move(movement);
        }
    }

    // rotates the player model based on input
    public void playerRotation(Vector3 movement)
    {
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
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
        if(pauseGame.isGamePaused == true) return;
        
        // explain context.performed https://forum.unity.com/threads/player-input-component-triggering-events-multiple-times.851959/
        if (!context.performed) return;

        // TODO jumping via CharacterControlUtilities.StandardJump (requires KinematicCharacterBody?)
        // https://docs.unity3d.com/Packages/com.unity.charactercontroller@1.0/manual/jumping.html
        switch (stateManager.state)
        {
            case PlayerState.Ropewalk:
                {
                    stateManager.state = PlayerState.Freemove;
                    lineIndexes = null;
                    controller.enabled = true;
                    yVelocity = jumpDistance;
                    break;
                }
            default:
                {
                    if (jumpCounter > 0)
                    {
                        jumpCounter--;
                        yVelocity = jumpDistance;
                    }
                    break;
                }
        }

        anim.SetBool("IsGrounded", false);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(pauseGame.isGamePaused == true) return;
        
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

                    nextIndex = Array.IndexOf(lineIndexes, newPlayerPos);
                    // Assume the line has at least 2 points
                    nextIndex = nextIndex == 0 ? 1 : nextIndex;
                    prevIndex = nextIndex - 1;

                    this.lineIndexes = lineIndexes;

                    stateManager.state = PlayerState.Ropewalk;

                    // https://forum.unity.com/threads/does-transform-position-work-on-a-charactercontroller.36149/#post-4132021
                    controller.enabled = false;
                    transform.position = newPlayerPos;
                    controller.enabled = true;

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
