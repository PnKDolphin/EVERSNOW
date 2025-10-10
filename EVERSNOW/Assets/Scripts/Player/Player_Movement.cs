using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    // Sprinting increases Max Velocity?

    // Serialized Variables
    [SerializeField] private AnimationCurve acceleration_curve;
    [SerializeField] private AnimationCurve deceleration_curve;

    [SerializeField] private float max_speed;
    [SerializeField] private Vector3 current_velocity;

    [SerializeField] private float accel_time; // Current Progress of Accel Curve
    [SerializeField] private float decel_time; // Current Progress of Decel Curve
    [SerializeField] private float accel_duration; // Total Time of Accel Curve
    [SerializeField] private float decel_duration; // Total Time of Decel Curve

    // Private Variables
    private PlayerInput input; // Player Input Component
    private InputActionMap action_map; // Movement Action Map
    private CharacterController char_controller;
    private Vector3 movement_input;

    private bool is_moving;


    void Start()
    {
        char_controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();

        action_map = input.currentActionMap;
        action_map.Enable();
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
    }

    void HandleInput()
    {
        float input_x = action_map.FindAction("Movement").Readvalue<Vector2>().x;
        float input_z = action_map.FindAction("Movement").Readvalue<Vector2>().y;

        movement_input = new Vector3(input_x, 0.0f, input_z).normalized;

        // Sprinting increases player max speed "max_speed = is_sprinting ? 5 : 10"
        // Jumping
        // Crouching
        // (Prone?)
    }


    void HandleMovement()
    {
        if(movement_input.magnitude > 0.01f)
        {
            is_moving = true;
            decel_time = 0;

            accel_time += Time.deltaTime;
            float t = Mathf.Clamp01(accel_time / accel_duration);
            float curve_value = acceleration_curve.Evaluate(t);
            
            current_velocity = movement_input * (max_speed * curve_value);
        }
        else 
        {
            is_moving = false;
            accel_time = 0;

            decel_time = += Time.deltaTime;
            float t = Mathf.Clamp01(decel_time / decel_duration);
            float curve_value = deceleration_curve.Evaluate(t);

            current_velocity = movement_input * (max_speed * curve_value);
        }

        char_controller.move(current_velocity * Time.deltaTime);
    }
}
