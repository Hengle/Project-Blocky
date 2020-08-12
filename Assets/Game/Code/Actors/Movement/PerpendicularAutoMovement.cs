using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectBlocky.Actors
{
    [HideMonoScript]
    [RequireComponentWarning(typeof(MovementController))]
    public class PerpendicularAutoMovement : MonoBehaviour
    {
        // Serialize

        [SerializeField]
        private bool useDelayOnCollision = false;
        [SerializeField]
        [ShowIf("useDelayOnCollision")]
        [LabelText("Collision delay time:")]
        private float collisionDelayTime = 0;

        // Cache

        private float collisionDelayTimer = 0;

        // Properties

        private bool IsDelayed
        {
            get
            {
                return (collisionDelayTimer > 0);
            }
        }

        private enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            RandomY,
            RandomX
        }

        [SerializeField]
        private Direction direction = 0;

        private Vector2 vectorDirection;

        private MovementController movementController;

        private void Awake()
        {
            movementController = GetComponent<MovementController>();
			movementController.OnColliderStay += OnColliderStay;

			switch (direction)
            {
                case Direction.Up:
                    vectorDirection = Vector2.up;
                    break;
                case Direction.Down:
                    vectorDirection = Vector2.down;
                    break;
                case Direction.Left:
                    vectorDirection = Vector2.left;
                    break;
                case Direction.Right:
                    vectorDirection = Vector2.right;
                    break;
                case Direction.RandomX:
                    if (UnityEngine.Random.Range(0, 2) == 1)
                        vectorDirection = Vector2.right;
                    else
                        vectorDirection = Vector2.left;
                    break;
                case Direction.RandomY:
                    if (UnityEngine.Random.Range(0, 2) == 1)
                        vectorDirection = Vector2.up;
                    else
                        vectorDirection = Vector2.down;
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (IsDelayed)
            {
                collisionDelayTimer -= Time.deltaTime;
            }
            else
            {
                movementController.AddForce(vectorDirection);
            }
        }

        private void OnColliderStay(Collider2D collider)
        {
            if (useDelayOnCollision) collisionDelayTimer = collisionDelayTime;

            vectorDirection *= -1;
        }
    }
}