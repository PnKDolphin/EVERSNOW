using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    // Move Player When Movement Keys Are Pressed
    // Acceleration and Deceleration Occur According To Animation Curves Curves

    // Sprinting increases Max Velocity?

    // Serialized Variables
    [SerializeField] private AnimationCurve m_acceleration_curve;
    [SerializeField] private AnimationCurve m_deceleration_curve;

    [SerializeField] private Vector2 m_max_velocity;
    [SerializeField] private Vector2 m_current_velocity;

    [SerializeField] private float m_current_time; // Current Progress of Accel/Decel Curves

    // Private Variables
    private PlayerInput m_input; // Player Input Component
    private InputActionMap m_action_map; // Movement Action Map
    private CharacterController m_char_controller;

    private float m_accelerating = 0.0f; // 1 = Accelerating, -1 = Decelerating, 0 = Neither


    void Start()
    {
        m_char_controller = GetComponent<CharacterController>();
        m_input = GetComponent<PlayerInput>();

        m_action_map = m_input.currentActionMap;
        m_action_map.Enable();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        Vector2 movement_input = m_action_map.FindAction("Movement").ReadValue<Vector2>();
        StartCoroutine(HandleMovement(movement_input));
    }


    IEnumerator HandleMovement(Vector2 movement_vector)
    {
        if (movement_vector.y != 0 && Mathf.Sign(movement_vector.y) == Mathf.Sign(m_current_velocity.y)) // Moving Same Direction As Input
        {
            StopCoroutine(DecelerateY());
            StartCoroutine(AccelerateY(Mathf.Sign(movement_vector.y)));
        }
        else if (movement_vector.y != 0 && Mathf.Sign(movement_vector.y) != Mathf.Sign(m_current_velocity.y)) // Changing Directions
        {
            StopCoroutine(AccelerateY(Mathf.Sign(movement_vector.y)));
            StartCoroutine(DecelerateY());
        }
        else if (movement_vector.y == 0) // Just Decelerating
        {
            StopCoroutine(AccelerateY(Mathf.Sign(movement_vector.y)));
            StartCoroutine(DecelerateY());
        }



        Vector3 move = transform.right * movement_vector.x + transform.forward * movement_vector.y;
        m_char_controller.Move(move * m_current_velocity * Time.deltaTime);

        yield return null;
    }

    IEnumerator AccelerateY(float direction)
    {
        // Gradually Increase Speed Along Y To Desired Max Velocity (Depending On Input Direction)
        if (direction > 0) // Moving Forward
        {
            while (m_current_velocity.y < m_max_velocity.y * direction)
            {
                m_current_time += Time.deltaTime;
                m_current_velocity.y = m_acceleration_curve.Evaluate(m_current_time);
            }
            m_current_time = 0.0f;
            m_current_velocity.y = m_max_velocity.y * direction;
            yield return null;
        }
        else if (direction < 0) // Moving Backward
        {
            while (m_current_velocity.y > m_max_velocity.y * direction)
            {
                m_current_time += Time.deltaTime;
                m_current_velocity.y = m_acceleration_curve.Evaluate(m_current_time);
            }
            m_current_time = 0.0f;
            m_current_velocity.y = m_max_velocity.y * direction;
            yield return null;
        }
    }

    IEnumerator DecelerateY()
    {
        while (m_current_velocity.y > 0)
        {
            m_current_time += Time.deltaTime;
            m_current_velocity.y = m_deceleration_curve.Evaluate(m_current_time);
        }

        m_current_time = 0.0f;
        m_current_velocity.y = 0.0f;
        yield return null;
        // Gradually Decrease Speed Along Y To 0
        // If Changing Directions, Decelerate Faster

        // Goal To Approach Zero Based On Current Velocity, Context Not Needed
    }
}

 
// Just Getting A Number To Approach Zero
// Positive Numbers Would Subtract, Negative Numbers Would Add
// If current_velocity < 0, Add, Opposite For Other Side, Not Actually That bad


// Movement Input : Vector2 (-1 to 1, -1 to 1) (x = left/right, y = forward/back)
// Move : Transform Multiplied The Movement Input

// Gradually Increase The Speed To Max When Accelerating
// Gradually Decrease The Speed To 0 When Not Accelerating Or When Changing Directions
// When applying the movement, we multiply Move by the current velocity.

// Maybe We Separate Backwards And Forwards Velocities as well as Left and Right Velocities?
// This provides opportunity to have the player move slower backwards than forwards, etc.

// Don't need to worry about directional acceleration because we account for this only when applying.

// Set the desired volcity based on input.
// While the current velocity is not equal to the desired velocity, we add to T multiplied by the movement vector.