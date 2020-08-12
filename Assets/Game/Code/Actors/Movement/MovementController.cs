using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ProjectBlocky.Actors
{
    /// <summary>
    /// Raycast physics is for sending physics events when the rays hit something. (faster than unity physics).
    /// </summary>
	public class MovementController : MonoBehaviour
	{
		// Serialize

		[InfoBox("Missing a Rigidbody2D!", "SetColliderAndRB", InfoMessageType = InfoMessageType.Error)]
		[SerializeField]
		private float defaultSpeed = 200f;

		[Space]
		[InfoBox("Using Rigidbody Physics. Set Rigidbody body type to Kinematic to use Raycast Physics.", "RigidbodyIsNotKinematic")]
		[EnableIf("RigidbodyIsKinematic")]
		[SerializeField]
		private bool useRaycastPhysics = true;
		[Space]
		[SerializeField]
		private bool useRaycastEvents = true;

        [Space]
        [ShowIf("UsingRays")]
        [EnumToggleButtons]
        [SerializeField]
        private RayTypes rayTypes = 0;
		[InfoBox("Attach a Collider2D to automatically use its extents.", "ShowCustomExtents")] // If no collider
		[ShowIf("ShowCustomExtents")] // If no collider
		[SerializeField]
		private Vector2 objectExtents;

		// Set in Awake

		private Transform thisTransform;

		private Rigidbody2D thisRigidbody;
		private bool useRigidbody;
		private bool hasDynamicRigidbody = false;

		private Collider2D thisCollider;
		private bool useCollider;
		private LayerMask layermask;

		// Cached

		private Vector2 cachedVelocity;
		public Vector2 CachedVelocity => cachedVelocity;
		
		/// <summary>
		/// Velocity that is used over normal velocity if set.
		/// </summary>
		private Vector2 staticVelocity;
		private bool useStaticVelocity = false;
		private Vector2 thisVelocity;
		private Vector2 currentPosition;
		
		private bool wasColliding = false;
		private Collider2D triggerCollider;

		private List<RaycastHit2D> raycastHits = new List<RaycastHit2D>();
		private RaycastHit2D[] firstHits = new RaycastHit2D[1];
		private RaycastHit2D[] secondHits = new RaycastHit2D[1];
		private RaycastHit2D[] thirdHits = new RaycastHit2D[1];
		private RaycastHit2D closestHit;

		#region Events


		public event ColliderHitEvent OnColliderEnter;
		public event ColliderHitEvent OnColliderStay;
		public event ColliderHitEvent OnColliderLeave;

		public delegate void ColliderHitEvent(Collider2D collider);

		/// <summary>
		/// Sends Trigger and Collision Enter/Stay/Leave events.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ProcessEvents(bool colliding)
		{
			if (colliding)
			{
				triggerCollider = closestHit.collider;
			}

			if (colliding && wasColliding) // Stay
			{
				OnColliderStay?.Invoke(triggerCollider);
			}
			else if (colliding && !wasColliding) // Enter
			{
				OnColliderEnter?.Invoke(triggerCollider);
			}
			else if (!colliding && wasColliding) // Leave
			{
				OnColliderLeave?.Invoke(triggerCollider);
			}

			wasColliding = colliding;
		}


		#endregion


		#region Speed

		private List<ISpeedMultiplier> speedMultipliers = new List<ISpeedMultiplier>();

		private byte byteIndex = 0;

		public void AddSpeedMultiplier(ISpeedMultiplier speedMultiplier)
		{
			byteIndex++;
			speedMultiplier.InitializeSpeedMultiplier(this, byteIndex);
			speedMultipliers.Add(speedMultiplier);
		}

		public void RemoveSpeedMultiplier(int ID)
		{
			for (int i = speedMultipliers.Count - 1; i >= 0; i--)
			{
				if (speedMultipliers[i].ID == ID)
				{
					speedMultipliers.RemoveAt(i);
					break;
				}
			}
		}

		public float CurrentSpeed
		{
			get
			{
				float speed = defaultSpeed;

				for (int i = 0; i < speedMultipliers.Count; i++)
				{
					speed *= speedMultipliers[i].SpeedMultiplier;
				}

				return speed;
			}
		}

		#endregion


		private void Awake()
		{
			thisTransform = this.transform;

			thisRigidbody = GetComponent<Rigidbody2D>();
			useRigidbody = thisRigidbody != null;
			if (useRigidbody && thisRigidbody.bodyType == RigidbodyType2D.Dynamic)
			{
				hasDynamicRigidbody = true;
			}

			var thisCollider = GetComponent<Collider2D>();
			useCollider = thisCollider != null;
			if (useCollider)
			{
				objectExtents = thisCollider.bounds.extents;
			}
			var thisLayer = gameObject.layer;
			layermask = Physics2D.GetLayerCollisionMask(thisLayer);

			CustomUpdate.OnEndOfFixedUpdate += EndOfFixedUpdate;

			currentPosition = thisTransform.position;
		}

		/// <summary>
		/// Runs after all other Fixed Update's.
		/// </summary>
		private void FixedUpdate() //Physics update
		{
			var fixedTime = Time.fixedDeltaTime;
			thisVelocity *= fixedTime;
			cachedVelocity = thisVelocity * fixedTime;
			Vector2 nextPosition = currentPosition + cachedVelocity;

			if (useRaycastPhysics || useRaycastEvents)
			{
				var hitCollider = PerformRays(cachedVelocity, rayTypes, ref raycastHits);

				if (useRaycastEvents)
				{
					ProcessEvents(hitCollider.HasPoint);
				}

				if (hitCollider.HasPoint && useRaycastPhysics)
				{
					nextPosition = currentPosition + hitCollider.Point;
				}
			}

			// Set position
			if (hasDynamicRigidbody)
			{
				thisRigidbody.velocity = thisVelocity;
			}
			else if (useRigidbody)
			{
				thisRigidbody.MovePosition(nextPosition);
			}
			else
			{
				thisTransform.position = nextPosition;
			}
		}
		
		private void EndOfFixedUpdate()
		{
			// Reset values per frame
			useStaticVelocity = false;
			currentPosition = thisTransform.position;
			thisVelocity = Vector2.zero;
		}

		private void AddVelocity(Vector2 velocity)
		{
			if (!useStaticVelocity)
			{
				thisVelocity += velocity;
			}
		}


		#region SetVelocity

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Vector2 GetDirectionalSpeed(Vector2 direction, float speed)
		{
			return direction.normalized * speed;
		}

		public void AddUnchangingForce(Vector2 direction, float speed)
		{
			staticVelocity = GetDirectionalSpeed(direction, speed);
			useStaticVelocity = true;
			thisVelocity = staticVelocity;
		}

		/// <summary>
		/// Adds a force in direction of speed.
		/// </summary>
		public void AddForceWithSpeed(Vector2 direction, float speed)
		{
			AddVelocity(GetDirectionalSpeed(direction, speed));
		}

		/// <summary>
		/// Adds a force in direction of CurrentSpeed * speed.
		/// </summary>
		public void AddForceWithSpeedMultipler(Vector2 direction, float speed)
		{
			AddVelocity(GetDirectionalSpeed(direction, CurrentSpeed * speed));
		}

		/// <summary>
		/// Adds a force in direction of CurrentSpeed.
		/// </summary>
		public void AddForce(Vector2 direction)
		{
			AddVelocity(GetDirectionalSpeed(direction, CurrentSpeed));
		}

		/// <summary>
		/// Adds a force in direction of CurrentSpeed.
		/// Callback and lock position to point when reached.
		/// </summary>
		public void AddForceTowardsPoint(Vector2 point, IPointMovement pointMovement = null)
		{
			MoveTowardsPoint(CurrentSpeed * Time.fixedDeltaTime * Time.fixedDeltaTime, point, pointMovement);
		}

		/// <summary>
		/// Adds a force in direction of speed.
		/// Callback and lock position to point when reached.
		/// </summary>
		public void AddForceTowardsPointWithSpeed(Vector2 point, float speed, IPointMovement pointMovement = null)
		{
			MoveTowardsPoint(speed * Time.fixedDeltaTime * Time.fixedDeltaTime, point, pointMovement);
		}

		/// <summary>
		/// Adds a force in direction of CurrentSpeed * speed.
		/// Callback and lock position to point when reached.
		/// </summary>
		public void AddForceTowardsPointWithSpeedMultiplier(Vector2 point, float speed, IPointMovement pointMovement = null)
		{
			MoveTowardsPoint(CurrentSpeed * speed * Time.fixedDeltaTime * Time.fixedDeltaTime, point, pointMovement);
		}
		
		private void MoveTowardsPoint(float magnitude, Vector2 point, IPointMovement tileMovement)
		{
			var direction = point - currentPosition;
			var nodeDistance = direction.magnitude;

			//If we overshot the point
			if (magnitude > nodeDistance)
			{
				currentPosition = point;

				if (tileMovement != null)
				{
					var newPoint = tileMovement.ReachedPoint();
					if (newPoint.HasPoint)
					{
						MoveTowardsPoint(magnitude - nodeDistance, newPoint.Point, tileMovement);
					}
				}
			}
			else
			{
				MoveWithMagnitude(direction, magnitude);
			}
		}

		private void MoveWithMagnitude(Vector2 direction, float magnitude)
		{
			var backwardsDelta = 1f / Time.fixedDeltaTime;
			AddVelocity(magnitude * direction.normalized * backwardsDelta * backwardsDelta);
		}


		#endregion


		#region Raycasting


		/// <summary>
		/// Performs ray with layermask.
		/// </summary>
		/// <returns>If hit object and distance to objects</returns>
		public NewPoint PerformRays(Vector2 velocity, RayTypes rays, LayerMask layerMask, ref List<RaycastHit2D> raycastHits)
		{
			raycastHits.Clear();
			const float CompareZero = 0.00001f;
			var hitCollider = false;
			Vector2 distanceToEdge = Vector2.zero;

			if (Mathf.Abs(velocity.x) > CompareZero)
			{
				var checkAxis = CheckCollisionAxis(velocity, AxisDirection.Horizontal, rays, layerMask, ref raycastHits);
				if (checkAxis.HasPoint)
				{
					hitCollider = true;
					distanceToEdge = checkAxis.Point;
				}
			}
			if (Mathf.Abs(velocity.y) > CompareZero)
			{
				var checkAxis = CheckCollisionAxis(velocity, AxisDirection.Vertical, rays, layerMask, ref raycastHits);
				if (checkAxis.HasPoint)
				{
					hitCollider = true;
					distanceToEdge += checkAxis.Point;
				}
			}

			if (hitCollider) return new NewPoint(distanceToEdge);
			else return new NewPoint(false);
		}

		/// <summary>
		/// Performs ray with gameobject layer.
		/// </summary>
		/// <returns>If hit object and distance to objects</returns>
		public NewPoint PerformRays(Vector2 velocity, RayTypes rays, ref List<RaycastHit2D> raycastHits)
		{
			raycastHits.Clear();
			const float CompareZero = 0.00001f;
			var hitCollider = false;
			Vector2 distanceToEdge = Vector2.zero;

			if (Mathf.Abs(velocity.x) > CompareZero)
			{
				var checkAxis = CheckCollisionAxis(velocity, AxisDirection.Horizontal, rays, layermask, ref raycastHits);
				if (checkAxis.HasPoint)
				{
					hitCollider = true;
					distanceToEdge = checkAxis.Point;
				}
			}
			if (Mathf.Abs(velocity.y) > CompareZero)
			{
				var checkAxis = CheckCollisionAxis(velocity, AxisDirection.Vertical, rays, layermask, ref raycastHits);
				if (checkAxis.HasPoint)
				{
					hitCollider = true;
					distanceToEdge += checkAxis.Point;
				}
			}

			if (hitCollider) return new NewPoint(distanceToEdge);
			else return new NewPoint(false);
		}

		[Flags]
		public enum RayTypes : byte
		{
			MiddleRay = 1,
			EdgeRays = 2
		}

		private enum AxisDirection : byte
		{
			Vertical = 0,
			Horizontal = 1
		}
		
		/// <returns>If hit ray and distance to hit</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private NewPoint CheckCollisionAxis(Vector2 velocity, AxisDirection axisDirection, RayTypes rayTypes, LayerMask layerMask, ref List<RaycastHit2D> raycastHits)
		{
			// percentage of the way to the edge where the ray should shoot
			const float DistanceToEdgeMultiplier = 0.75f;

			// If Vertical
			float axisVelocity = velocity.y;
			Vector2 direction = new Vector2(0, axisVelocity);
			float forwardDistance = objectExtents.y;
			float sideDistance = objectExtents.x * DistanceToEdgeMultiplier;
			Vector2 directionToEdge = Vector2.right;

			// If Horizontal
			if (axisDirection == AxisDirection.Horizontal)
			{
				directionToEdge = Vector2.up;
				axisVelocity = velocity.x;
				direction = new Vector2(axisVelocity, 0);
				forwardDistance = objectExtents.x;
				sideDistance = objectExtents.y * DistanceToEdgeMultiplier;
			}

			var rayDistance = Mathf.Abs(axisVelocity) + forwardDistance;
			direction.Normalize();
			bool hitFirstPoint = false;
			bool hitSecondPoint = false;
			bool hitThirdPoint = false;

			if (rayTypes.HasFlag(RayTypes.EdgeRays))
			{
				directionToEdge *= sideDistance;
				var firstPoint = currentPosition + directionToEdge;
				var secondPoint = currentPosition - directionToEdge;

#if UNITY_EDITOR
				var directionAndLength = direction * rayDistance;
				debugRays[0] = new Ray(firstPoint, directionAndLength);
				debugRays[1] = new Ray(secondPoint, directionAndLength);
				debugRays[2] = new Ray(currentPosition, directionAndLength);
#endif
				hitFirstPoint = (Physics2D.RaycastNonAlloc(firstPoint, direction, firstHits, rayDistance, layerMask) > 0);
				hitSecondPoint = (Physics2D.RaycastNonAlloc(secondPoint, direction, secondHits, rayDistance, layerMask) > 0);
			}
			if (rayTypes.HasFlag(RayTypes.MiddleRay))
			{
				hitThirdPoint = (Physics2D.RaycastNonAlloc(currentPosition, direction, thirdHits, rayDistance, layerMask) > 0);
			}
			
			// If hit collider
			if (hitFirstPoint || hitSecondPoint || hitThirdPoint)
			{
				// Find closest point
				var firstHit = firstHits[0];
				var secondHit = secondHits[0];
				var thirdHit = thirdHits[0];

				float firstHitDistance = (hitFirstPoint) ? firstHit.distance : 255;
				float secondHitDistance = (hitSecondPoint) ? secondHit.distance : 255;
				float thirdHitDistance = (hitThirdPoint) ? thirdHit.distance : 255;

				closestHit.distance = 254;
				if (firstHitDistance < secondHitDistance)
				{
					closestHit = firstHit;
				}
				if (closestHit.distance >= secondHitDistance)
				{
					closestHit = secondHit;
				}
				if (closestHit.distance >= thirdHitDistance)
				{
					closestHit = thirdHit;
				}

				if (hitFirstPoint) raycastHits.Add(firstHit);
				if (hitSecondPoint) raycastHits.Add(secondHit);
				if (hitThirdPoint) raycastHits.Add(thirdHit);
				
				return new NewPoint(direction * (closestHit.distance - forwardDistance));
			}

			return new NewPoint(false);
		}


		#endregion
		

#if UNITY_EDITOR

		// Inspector Checks

		private void FetchCollider()
		{
			useCollider = GetComponent<Collider2D>() != null;
		}

		private bool ShowCustomExtents => !useCollider && UsingRays;

		private bool UsingRays => useRaycastEvents || useRaycastPhysics;

		/// <returns>Has collider but no RB for error</returns>
		private bool SetColliderAndRB()
		{
			FetchCollider();
			useRigidbody = (thisRigidbody = GetComponent<Rigidbody2D>()) != null;

			return (useCollider && !useRigidbody);
		}

		/// <summary>
		/// Is Kinematic or doesn't exist.
		/// Also sets useRaycastPhysics false if RB is dynamic.
		/// </summary>
		/// <returns></returns>
		private bool RigidbodyIsKinematic()
		{
			if (useRigidbody)
			{
				var bodyType = thisRigidbody.bodyType;

				if (bodyType == RigidbodyType2D.Kinematic)
				{
					return true;
				}
				else if (bodyType == RigidbodyType2D.Dynamic)
				{
					useRaycastPhysics = false;
					return false;
				}
				else
				{
					throw new System.Exception("Rigidbody can not be static with a MovementController.");
				}
			}
			else
			{
				return true;
			}
		}

		private bool RigidbodyIsNotKinematic => useRigidbody && !thisRigidbody.isKinematic;

		private Ray[] debugRays = new Ray[3];

		private void OnDrawGizmos()
		{
			var debugRaysCount = debugRays.Length;
			for (int i = 0; i < debugRaysCount; i++)
			{
				Gizmos.DrawRay(debugRays[i]);
			}
		}
#endif
	}
}