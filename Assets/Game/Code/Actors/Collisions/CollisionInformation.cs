using System.Xml;
using Unity.Mathematics;
using UnityEngine;

namespace ProjectBlocky.Actors {
	public enum CollisionDirection : byte {
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3,
		None = 4
	}

	public struct DirectionalCollisionInformation {
		#region Static Fields
		// ----------------------------------------------------------------------------------------------------
		private static readonly ContactPoint2D[] ContactPoints = new ContactPoint2D[5];
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Fields & Properties
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// The collider.
		/// </summary>
		public GameObject Collider;

		/// <summary>
		/// The collider hit data.
		/// </summary>
		public Collision2D Collision;

		/// <summary>
		/// Gets the inverse collision direction.
		/// </summary>
		public CollisionDirection DirectionTo {
			get {
				switch (this.DirectionFrom) {
					case CollisionDirection.Up:
						return CollisionDirection.Down;
					case CollisionDirection.Down:
						return CollisionDirection.Up;
					case CollisionDirection.Left:
						return CollisionDirection.Right;
					case CollisionDirection.Right:
						return CollisionDirection.Left;
				}

				return CollisionDirection.Up;
			}
		}

		/// <summary>
		/// The collision direction
		/// </summary>
		public CollisionDirection DirectionFrom;
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Initialization
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="CollisionInformation"/> struct.
		/// </summary>
		public DirectionalCollisionInformation(Collision2D collision, CollisionDirection directionFrom) {
			this.Collider = collision.gameObject;
			this.Collision = collision;
			this.DirectionFrom = directionFrom;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion

		#region Methods
		// ----------------------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the direction from vector.
		/// </summary>
		/// <param name="collisionInfo">The directional vector.</param>
		public static CollisionDirection GetCollisionDirection(Collision2D collisionInfo) {
			var velocity = collisionInfo.relativeVelocity;
			var direction = velocity.normalized;
			var magnitude = direction.magnitude;
			if (velocity.magnitude <= 4f) {
				return CollisionDirection.None;
			}

			collisionInfo.GetContacts(ContactPoints);
			var contactPoint = ContactPoints[0];

			var relativePont = contactPoint.point - (Vector2) collisionInfo.gameObject.transform.position;

			if (contactPoint.normal.x < 0) {
				if (direction.x < 0 && math.abs(relativePont.y) < 0.2f) {
					return CollisionDirection.Left;
				}
			}
			else if (contactPoint.normal.x > 0) {
				if (direction.x > 0 && math.abs(relativePont.y) < 0.2f) {
					return CollisionDirection.Right;
				}
			}
			else if (contactPoint.normal.y > 0) {
				if (direction.y > 0 && math.abs(relativePont.x) < 0.2f) {
					return CollisionDirection.Up;
				}
			}
			else if (contactPoint.normal.y < 0) {
				if (direction.y < 0 && math.abs(relativePont.x) < 0.2f) {
					return CollisionDirection.Down;
				}
			}

			return CollisionDirection.None;
		}
		// ----------------------------------------------------------------------------------------------------
		#endregion
	}
}
