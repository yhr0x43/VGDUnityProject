using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    None,
    Freemove,
    Jumping,
    Ropewalk
}

public enum InteractAction
{
    None,
    AttachRope,
    DetachRope
}

public class PlayerStateManager : MonoBehaviour
{
    public PlayerState state = PlayerState.None;
    public InteractAction interact = InteractAction.None;

    public GameObject collidingRope;

    private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        state = PlayerState.Freemove;
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (PlayerState.Jumping == state && controller.isGrounded)
        {
            state = PlayerState.Freemove;
        }
    }

    /* This needs to be registered with Player Input Events */
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        state = PlayerState.Jumping;
    }

    void OnTriggerEnter(Collider collider)
    {
        GameObject other = collider.gameObject;
        if (other.CompareTag("Rope"))
        {
            collidingRope = other;
            interact = InteractAction.AttachRope;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject == collidingRope)
        {
            collidingRope = null;
            interact = InteractAction.None;
        }
    }
}
