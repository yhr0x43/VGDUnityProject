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
    public InputMaster inputMaster;
    Vector2 moveDirection;
    Vector2 lookDirection;
    InputAction moveInput;
    InputAction lookInput;
    InputAction jumpInput;

    Rigidbody rb;
    bool touchedGround;

    /* https://discussions.unity.com/t/how-do-i-check-if-my-rigidbody-player-is-grounded/33250/11 */
    private bool IsOnGround()
    {
        if (touchedGround)
        {
            touchedGround = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // NOTE consider downward facing collision as "touching ground" is this correct?
        if (!touchedGround)
        {
            touchedGround = (from contact in collision.contacts select contact.normal.y).Any(y => y > 0f);
        }
    }

    private void Awake()
    {
        inputMaster = new InputMaster();
    }

    private void OnEnable()
    {
        moveInput = inputMaster.Player.Move;
        moveInput.Enable();

        lookInput = inputMaster.Player.Look;
        lookInput.Enable();

        jumpInput = inputMaster.Player.Jump;
        jumpInput.Enable();
        jumpInput.performed += Jump;

    }

    private void OnDisable()
    {
        moveInput.Disable();
        lookInput.Disable();
        jumpInput.Disable();
    }

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        // TODO actually this should rotate camera
        rb.transform.Rotate(Vector3.up * Time.deltaTime * lookDirection.x * turnSpeed);
    }

    private void FixedUpdate()
    {
        //Debug.Log($"{touchedGround}");
        moveDirection = moveInput.ReadValue<Vector2>();
        lookDirection = lookInput.ReadValue<Vector2>();
        rb.AddRelativeForce(new Vector3(moveDirection.x, 0, moveDirection.y) * moveForce);
        //rb.transform.Translate(new Vector3(moveDirection.x, 0, moveDirection.y) * Time.deltaTime * speed);
        //rb.transform.Rotate(((Vector3.up * lookDirection.x) + (Vector3.left * lookDirection.y)) * Time.deltaTime * turnSpeed);
    }

    /* https://gamedevbeginner.com/how-to-jump-in-unity-with-or-without-physics/ */
    private void Jump(InputAction.CallbackContext context)
    {
        if (IsOnGround())
        {
            rb.AddForce(Vector3.up * jumpForce);
        }
    }
}
