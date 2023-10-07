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

    public GameObject cameralFocal, virtualCamera;
    CharacterController controller;
    PlayerStateManager stateManager;

    public GameObject startObject, endObject;

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
        stateManager = GetComponent<PlayerStateManager>();
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
        /* explain context.performed https://forum.unity.com/threads/player-input-component-triggering-events-multiple-times.851959/ */
        if (!context.performed) return;

        /* TODO jumping via CharacterControlUtilities.StandardJump (requires KinematicCharacterBody?)
         * https://docs.unity3d.com/Packages/com.unity.charactercontroller@1.0/manual/jumping.html
         */
        if (controller.isGrounded)
        {
            yVelocity = jumpDistance;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        Vector3[] lineIndexes;
        float distanceFromIndex, shortestDistance = 10000;
        Vector3 newPlayerPos = Vector3.zero;


        RaycastHit hit;
        // if the player and the startObject are within light of sight of each other
        if (Physics.Raycast(transform.position, startObject.transform.position - transform.position, out hit, Vector3.Magnitude(startObject.transform.position - transform.position)))
        {
            Debug.Log(hit.collider.gameObject);

            // get the number of line indexes created from line renderer
            lineIndexes = new Vector3[startObject.GetComponent<LineRenderer>().positionCount];

            // put all line indexes in a vector3 array
            startObject.GetComponent<LineRenderer>().GetPositions(lineIndexes);

            // find closet lineIndex to the player, and attach player to that lineIndex
            for (int i = 0; i < lineIndexes.Length - 1; i++)
            {
                // find the closet lineIndex to the player
                distanceFromIndex = Vector3.Distance(lineIndexes[i], transform.position);

                //Debug.Log(distanceFromIndex);
                // determine if the latest player/lineIndex distance is shorter than the previous one
                if (distanceFromIndex < shortestDistance)
                {
                    newPlayerPos = lineIndexes[i];
                    shortestDistance = distanceFromIndex;
                }
                //Debug.Log(lineIndexes[i]);
            }

            Debug.Log(newPlayerPos);
            // update the player's transform to the nearest lineIndex
            controller.enabled = false;
            transform.position = newPlayerPos;
            controller.enabled = true;
        }
        else
        {
            Debug.Log("Did not Hit");
        }
    }
}
