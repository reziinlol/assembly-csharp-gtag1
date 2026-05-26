using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag.Reactions
{
	// Token: 0x020011A4 RID: 4516
	public class SpawnWorldEffects : MonoBehaviour
	{
		// Token: 0x06007237 RID: 29239 RVA: 0x00253260 File Offset: 0x00251460
		protected void OnEnable()
		{
			if (GorillaComputer.instance == null)
			{
				Debug.LogError("SpawnWorldEffects: Disabling because GorillaComputer not found! Hierarchy path: " + base.transform.GetPath(), this);
				base.enabled = false;
				return;
			}
			if (this._prefabToSpawn != null && !this._isPrefabInPool)
			{
				if (this._prefabToSpawn.CompareTag("Untagged"))
				{
					Debug.LogError("SpawnWorldEffects: Disabling because Spawn Prefab has no tag! Hierarchy path: " + base.transform.GetPath(), this);
					base.enabled = false;
					return;
				}
				this._isPrefabInPool = ObjectPools.instance.DoesPoolExist(this._prefabToSpawn);
				if (!this._isPrefabInPool)
				{
					Debug.LogError("SpawnWorldEffects: Disabling because Spawn Prefab not in pool! Hierarchy path: " + base.transform.GetPath(), this);
					base.enabled = false;
					return;
				}
				this._pool = ObjectPools.instance.GetPoolByObjectType(this._prefabToSpawn);
			}
			this._hasPrefabToSpawn = (this._prefabToSpawn != null && this._isPrefabInPool);
		}

		// Token: 0x06007238 RID: 29240 RVA: 0x00253364 File Offset: 0x00251564
		public void RequestSpawn(Vector3 worldPosition)
		{
			this.RequestSpawn(worldPosition, Vector3.up);
		}

		// Token: 0x06007239 RID: 29241 RVA: 0x00253374 File Offset: 0x00251574
		public void RequestSpawn(Vector3 worldPosition, Vector3 normal)
		{
			if (this._maxParticleHitReactionRate < 1E-05f || !FireManager.hasInstance)
			{
				return;
			}
			double num = GTTime.TimeAsDouble();
			if ((float)(num - this._lastCollisionTime) < 1f / this._maxParticleHitReactionRate)
			{
				return;
			}
			if (this._hasPrefabToSpawn && this._isPrefabInPool && this._pool.GetInactiveCount() > 0)
			{
				Vector3 vector = normal;
				if (this._useNormalOrientation)
				{
					vector = this.GetSurfaceNormal(worldPosition, normal);
				}
				Quaternion? rotationOverride = null;
				if (this._forwardOrientationSource != null)
				{
					Vector3 vector2 = Vector3.ProjectOnPlane(SpawnWorldEffects.GetAxisVector(this._forwardOrientationSource, this._forwardSourceAxis), vector);
					if (vector2.sqrMagnitude < 0.0001f)
					{
						vector2 = Vector3.ProjectOnPlane((Mathf.Abs(vector.y) < 0.99f) ? Vector3.up : Vector3.right, vector);
					}
					rotationOverride = new Quaternion?(Quaternion.LookRotation(vector2.normalized, vector));
				}
				FireManager.SpawnFire(this._pool, worldPosition, vector, base.transform.lossyScale.x, rotationOverride);
			}
			this._lastCollisionTime = num;
		}

		// Token: 0x0600723A RID: 29242 RVA: 0x00253488 File Offset: 0x00251688
		private static Vector3 GetAxisVector(Transform source, SpawnWorldEffects.TransformAxis axis)
		{
			Vector3 result;
			switch (axis)
			{
			case SpawnWorldEffects.TransformAxis.Forward:
				result = source.forward;
				break;
			case SpawnWorldEffects.TransformAxis.Back:
				result = -source.forward;
				break;
			case SpawnWorldEffects.TransformAxis.Right:
				result = source.right;
				break;
			case SpawnWorldEffects.TransformAxis.Left:
				result = -source.right;
				break;
			case SpawnWorldEffects.TransformAxis.Up:
				result = source.up;
				break;
			case SpawnWorldEffects.TransformAxis.Down:
				result = -source.up;
				break;
			default:
				result = source.forward;
				break;
			}
			return result;
		}

		// Token: 0x0600723B RID: 29243 RVA: 0x00253504 File Offset: 0x00251704
		private Vector3 GetSurfaceNormal(Vector3 worldPosition, Vector3 hitNormal)
		{
			Vector3 vector;
			if (this._raycastDirectionSource != null)
			{
				Vector3 a = this._raycastDirectionSource.forward;
				if (this._raycastDirectionUseNegativeForward)
				{
					a = -a;
				}
				vector = ((a.sqrMagnitude > 1E-06f) ? a.normalized : Vector3.down);
			}
			else
			{
				vector = -((hitNormal.sqrMagnitude > 1E-06f) ? hitNormal.normalized : Vector3.up);
			}
			Vector3 origin = worldPosition + -vector * 0.05f;
			Vector3 direction = vector;
			RaycastHit raycastHit;
			if (Physics.Raycast(origin, direction, out raycastHit, this._normalRaycastDistance + 0.05f, this._normalRaycastLayers, QueryTriggerInteraction.Ignore))
			{
				return raycastHit.normal;
			}
			return hitNormal;
		}

		// Token: 0x0400820A RID: 33290
		[Tooltip("The defaults are numbers for the flamethrower hair dryer.")]
		private readonly float _maxParticleHitReactionRate = 2f;

		// Token: 0x0400820B RID: 33291
		[Tooltip("Must be in the global object pool and have a tag.")]
		[SerializeField]
		private GameObject _prefabToSpawn;

		// Token: 0x0400820C RID: 33292
		[Tooltip("When enabled, a short raycast is fired from the spawn position to find the exact surface normal. The spawned object's Up vector will be aligned to that normal instead of world Up.")]
		[SerializeField]
		private bool _useNormalOrientation;

		// Token: 0x0400820D RID: 33293
		[SerializeField]
		private float _normalRaycastDistance = 0.3f;

		// Token: 0x0400820E RID: 33294
		[SerializeField]
		private LayerMask _normalRaycastLayers = 134218241;

		// Token: 0x0400820F RID: 33295
		[Header("Raycast Direction Override")]
		[Tooltip("Optional. When assigned, the raycast used for normal-orientation will shoot along this transform's forward axis instead of along the incoming hit normal.")]
		[SerializeField]
		private Transform _raycastDirectionSource;

		// Token: 0x04008210 RID: 33296
		[Tooltip("If true, uses -forward instead of forward from Raycast Direction Source.")]
		[SerializeField]
		private bool _raycastDirectionUseNegativeForward;

		// Token: 0x04008211 RID: 33297
		[Header("Forward Orientation")]
		[Tooltip("Optional. When assigned, the spawned object's forward vector will be aligned to the chosen axis of this transform, projected onto the spawn surface.")]
		[SerializeField]
		private Transform _forwardOrientationSource;

		// Token: 0x04008212 RID: 33298
		[Tooltip("Which local axis of the Forward Orientation Source to use as the spawned object's forward.")]
		[SerializeField]
		private SpawnWorldEffects.TransformAxis _forwardSourceAxis;

		// Token: 0x04008213 RID: 33299
		private bool _hasPrefabToSpawn;

		// Token: 0x04008214 RID: 33300
		private bool _isPrefabInPool;

		// Token: 0x04008215 RID: 33301
		private double _lastCollisionTime;

		// Token: 0x04008216 RID: 33302
		private SinglePool _pool;

		// Token: 0x020011A5 RID: 4517
		private enum TransformAxis
		{
			// Token: 0x04008218 RID: 33304
			Forward,
			// Token: 0x04008219 RID: 33305
			Back,
			// Token: 0x0400821A RID: 33306
			Right,
			// Token: 0x0400821B RID: 33307
			Left,
			// Token: 0x0400821C RID: 33308
			Up,
			// Token: 0x0400821D RID: 33309
			Down
		}
	}
}
