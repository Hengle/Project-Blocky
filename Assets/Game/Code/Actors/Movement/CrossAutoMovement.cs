using Sirenix.OdinInspector;
using UnityEngine;

namespace ProjectBlocky.Actors
{
    [HideMonoScript]
    [RequireComponentWarning(typeof(MovementController))]
    public class CrossAutoMovement : MonoBehaviour
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
            UpRight,
            UpLeft,
            DownRight,
            DownLeft,
            Random
        }

        [SerializeField]
        private Direction direction = 0;

        private Vector2 vectorDirection;

        private MovementController movementController;

        private void Awake()
        {
            movementController = GetComponent<MovementController>();

			switch (direction)
            {
                case Direction.UpLeft:
                    vectorDirection = new Vector2(-1, 1);
                    break;
                case Direction.UpRight:
                    vectorDirection = new Vector2(1, 1);
                    break;
                case Direction.DownLeft:
                    vectorDirection = new Vector2(-1, -1);
                    break;
                case Direction.DownRight:
                    vectorDirection = new Vector2(1, -1);
                    break;
                case Direction.Random:
					var randomNum = UnityEngine.Random.Range(0, 4);

                    if (randomNum == 1)
						vectorDirection = new Vector2(-1, 1);
					else if (randomNum == 2)
						vectorDirection = new Vector2(1, 1);
					else if (randomNum == 3)
						vectorDirection = new Vector2(-1, -1);
					else
						vectorDirection = new Vector2(1, -1);
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

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (useDelayOnCollision) collisionDelayTimer = collisionDelayTime;

			var contactNormal = collision.contacts[0].normal;
			const float FloatingPointValue = 0.0001f;
			var moveVert = (contactNormal.y > FloatingPointValue || contactNormal.y < -FloatingPointValue);

			if (moveVert)
			{
				if (contactNormal.y > FloatingPointValue) vectorDirection.y = 1;
				else vectorDirection.y = -1;
			}
			else
			{
				if (contactNormal.x < FloatingPointValue) vectorDirection.x = -1;
				else vectorDirection.x = 1;
			}
		}
	}
}