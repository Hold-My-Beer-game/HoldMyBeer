
using UnityEngine;
using UnityEngine.InputSystem;





public class PlayerWalk : MonoBehaviour
{
   public InputActionAsset InputAction;

   private InputAction moveAction;
   private InputAction jumpAction;

   private Vector2 move;

   private Rigidbody rb;

   public float WalkSpeed = 5f;
   public float JumpForce = 5f;

   private void OnEnable()
   {
      InputAction.FindActionMap("ZombieRoot").Enable();
   }

   private void OnDisable()
   {
      InputAction.FindActionMap("ZombieRoot").Disable();
   }

   private void Awake()
   {
      moveAction = InputAction.FindAction("Move");
      jumpAction = InputAction.FindAction("Jump");

      rb = GetComponent<Rigidbody>();
   }

   private void Update()
   {
      move = moveAction.ReadValue<Vector2>();
   }

   private void FixedUpdate()
   {
      MovePlayer();
   }

   private void MovePlayer()
   {
      Vector3 direction =
         transform.forward * move.y +
         transform.right * move.x;

      rb.MovePosition(rb.position + direction * (WalkSpeed * Time.fixedDeltaTime));
   }
}
