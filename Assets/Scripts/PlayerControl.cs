using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 90f;
    /* FIXME jumpDistance is not working as intended */
    public float jumpDistance = 0.04f;
    public float gravity = 0.08f;

    /* 
     * InputSystem: https://youtu.be/HmXU4dZbaMw
     * https://learn.unity.com/tutorial/configuring-an-xbox-controller-for-user-input-2019-2
     */
    Vector2 moveDirection;
    Vector2 lookDirection;

    public GameObject cameralFocal;
    public GameObject virtualCamera;
    CharacterController controller;

    float yVelocity = 0f;

    private static Vector2 Rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Start()
    {
        
    }

    private void Update()
    {
        cameralFocal.transform.Rotate(Vector3.up * lookDirection.x * turnSpeed * Time.deltaTime);
        float cameraYaw = -virtualCamera.transform.rotation.y * Mathf.PI;
        Vector2 alignedMovement = Rotate(moveDirection, cameraYaw) * moveSpeed * Time.deltaTime;
        Debug.Log(alignedMovement);
        if (!controller.isGrounded)
        {
            yVelocity -= gravity * Time.deltaTime;
        }
        controller.Move(new Vector3(alignedMovement.x, yVelocity, alignedMovement.y));
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
        /* TODO jumping via CharacterControlUtilities.StandardJump (requires KinematicCharacterBody?)
         * https://docs.unity3d.com/Packages/com.unity.charactercontroller@1.0/manual/jumping.html
         */
        /* https://forum.unity.com/threads/player-input-component-triggering-events-multiple-times.851959/ */
        if (controller.isGrounded && context.performed)
        {
            yVelocity = jumpDistance;
        }
    }
}
