using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription
{
	// Token: 0x02000F68 RID: 3944
	public class SubscriberExclusiveZone : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06006241 RID: 25153 RVA: 0x001FB39C File Offset: 0x001F959C
		private void Awake()
		{
			if (this.restrictedZone != null)
			{
				this.restrictedZoneCollider = this.restrictedZone.GetComponent<Collider>();
				if (this.restrictedZoneCollider != null && !this.restrictedZoneCollider.isTrigger)
				{
					Debug.LogError("restrictedZone must be a trigger collider!", this);
					base.enabled = false;
					return;
				}
				SubscriberZoneTrigger subscriberZoneTrigger = this.restrictedZone.GetComponent<SubscriberZoneTrigger>();
				if (subscriberZoneTrigger == null)
				{
					subscriberZoneTrigger = this.restrictedZone.AddComponent<SubscriberZoneTrigger>();
				}
				subscriberZoneTrigger.parentZone = this;
				subscriberZoneTrigger.isRestrictedZone = true;
			}
			if (this.warningZone != null)
			{
				this.influenceZoneCollider = this.warningZone.GetComponent<Collider>();
				if (this.influenceZoneCollider != null && !this.influenceZoneCollider.isTrigger)
				{
					Debug.LogError("influenceZone must be a trigger collider!", this);
					base.enabled = false;
					return;
				}
				SubscriberZoneTrigger subscriberZoneTrigger2 = this.warningZone.GetComponent<SubscriberZoneTrigger>();
				if (subscriberZoneTrigger2 == null)
				{
					subscriberZoneTrigger2 = this.warningZone.AddComponent<SubscriberZoneTrigger>();
				}
				subscriberZoneTrigger2.parentZone = this;
				subscriberZoneTrigger2.isRestrictedZone = false;
			}
			if (this.ejectionPoint == null)
			{
				Debug.LogError("Assign an ejectionPoint!", this);
				base.enabled = false;
				return;
			}
			this.UpdateDoor();
		}

		// Token: 0x06006242 RID: 25154 RVA: 0x001FB4C8 File Offset: 0x001F96C8
		private void OnEnable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			GorillaSlicerSimpleManager.RegisterSliceable(this);
		}

		// Token: 0x06006243 RID: 25155 RVA: 0x001FB4D8 File Offset: 0x001F96D8
		private void OnDisable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			GorillaSlicerSimpleManager.UnregisterSliceable(this);
			this.ClearAllRigOverrides();
		}

		// Token: 0x06006244 RID: 25156 RVA: 0x001FB4EF File Offset: 0x001F96EF
		private void Update()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			this.UpdateDoor();
			if (!SubscriptionManager.IsLocalSubscribed())
			{
				this.HandleZoneBehavior();
				return;
			}
			if (this.bodyColliderWasDisabled)
			{
				this.SetBodyCollider(GTPlayer.Instance, true);
				this.bodyColliderWasDisabled = false;
			}
		}

		// Token: 0x06006245 RID: 25157 RVA: 0x001FB528 File Offset: 0x001F9728
		public void OnZoneEnter(bool isRestricted)
		{
			if (isRestricted)
			{
				this.insideRestricted = true;
				if (this.showDebugInfo)
				{
					Debug.Log("[Zone] Entered restricted zone");
					return;
				}
			}
			else
			{
				this.insideInfluence = true;
				if (this.showDebugInfo)
				{
					Debug.Log("[Zone] Entered warning zone");
				}
			}
		}

		// Token: 0x06006246 RID: 25158 RVA: 0x001FB560 File Offset: 0x001F9760
		public void OnZoneExit(bool isRestricted)
		{
			if (isRestricted)
			{
				this.insideRestricted = false;
				if (this.showDebugInfo)
				{
					Debug.Log("[Zone] Exited restricted zone");
					return;
				}
			}
			else
			{
				this.insideInfluence = false;
				if (this.showDebugInfo)
				{
					Debug.Log("[Zone] Exited warning zone");
				}
			}
		}

		// Token: 0x06006247 RID: 25159 RVA: 0x001FB598 File Offset: 0x001F9798
		private void HandleZoneBehavior()
		{
			GTPlayer instance = GTPlayer.Instance;
			if (instance == null)
			{
				return;
			}
			if (this.insideRestricted)
			{
				if (Time.time - this.lastShoveTime >= this.shoveCooldown)
				{
					this.lastShoveTime = Time.time;
					instance.TeleportTo(this.ejectionPoint.position, instance.transform.rotation, true, false);
					UnityEvent onEnterRestrictedZone = this.OnEnterRestrictedZone;
					if (onEnterRestrictedZone == null)
					{
						return;
					}
					onEnterRestrictedZone.Invoke();
				}
				return;
			}
			if (this.insideInfluence)
			{
				this.DisplaceToward(instance, this.ejectionPoint, this.driftSpeed);
				UnityEvent onWarning = this.OnWarning;
				if (onWarning == null)
				{
					return;
				}
				onWarning.Invoke();
			}
		}

		// Token: 0x06006248 RID: 25160 RVA: 0x001FB638 File Offset: 0x001F9838
		private Vector3 FindSafeEjectionPosition(Vector3 playerPos)
		{
			if (this.restrictedZoneCollider == null)
			{
				return this.ejectionPoint.position;
			}
			Bounds bounds = this.restrictedZoneCollider.bounds;
			Vector3 a = bounds.ClosestPoint(playerPos);
			Vector3 normalized = (a - bounds.center).normalized;
			Vector3 vector = a + normalized * (this.safetyCheckRadius + 0.5f);
			float maxDistance = Vector3.Distance(playerPos, vector);
			RaycastHit raycastHit;
			if (Physics.SphereCast(playerPos + Vector3.up * this.safetyCheckRadius, this.safetyCheckRadius, normalized, out raycastHit, maxDistance, this.obstacleLayers))
			{
				vector = playerPos + normalized * Mathf.Max(0.1f, raycastHit.distance - this.safetyCheckRadius - 0.2f);
			}
			return vector;
		}

		// Token: 0x06006249 RID: 25161 RVA: 0x001FB708 File Offset: 0x001F9908
		private void DisplaceToward(GTPlayer player, Transform target, float speed)
		{
			Vector3 normalized = (target.position - player.transform.position).normalized;
			player.transform.position += normalized * speed * Time.deltaTime;
		}

		// Token: 0x0600624A RID: 25162 RVA: 0x001FB75C File Offset: 0x001F995C
		private void SetBodyCollider(GTPlayer player, bool enabled)
		{
			if (player != null && player.bodyCollider != null)
			{
				player.bodyCollider.enabled = enabled;
				this.bodyColliderWasDisabled = !enabled;
				if (this.showDebugInfo && player.bodyCollider.enabled != enabled)
				{
					Debug.Log(string.Format("[Zone] Body collider: {0}", enabled));
				}
			}
		}

		// Token: 0x0600624B RID: 25163 RVA: 0x001FB7C4 File Offset: 0x001F99C4
		private void UpdateDoor()
		{
			bool flag = SubscriptionManager.IsLocalSubscribed();
			if (this.nonSubscribeDoorObject.activeSelf == flag)
			{
				this.nonSubscribeDoorObject.SetActive(!flag);
			}
			if (this.subscriberDoorObject.activeSelf != flag)
			{
				this.subscriberDoorObject.SetActive(SubscriptionManager.IsLocalSubscribed());
			}
		}

		// Token: 0x0600624C RID: 25164 RVA: 0x001FB814 File Offset: 0x001F9A14
		public void SliceUpdate()
		{
			VRRigCache.Instance.GetActiveRigs(this.rigs);
			if (this.restrictedZoneCollider == null)
			{
				return;
			}
			for (int i = 0; i < this.rigs.Count; i++)
			{
				if (!this.rigs[i].isOfflineVRRig && !SubscriptionManager.GetSubscriptionDetails(this.rigs[i]).active)
				{
					Vector3 vector = this.restrictedZoneCollider.transform.InverseTransformPoint(this.rigs[i].syncPos);
					Vector3 vector2 = ((BoxCollider)this.restrictedZoneCollider).size / 2f;
					Vector3 center = ((BoxCollider)this.restrictedZoneCollider).center;
					if (vector.x < vector2.x + center.x && vector.x > -vector2.x + center.x && vector.y < vector2.y + center.y && vector.y > -vector2.y + center.y && vector.z < vector2.z + center.z && vector.z > -vector2.z + center.z)
					{
						this.rigs[i].InOverrideSubscriptionZone = true;
						this.rigs[i].OverrideSubscriptionZoneLocation = this.ejectionPoint.position;
					}
					else
					{
						this.rigs[i].InOverrideSubscriptionZone = false;
						this.rigs[i].OverrideSubscriptionZoneLocation = Vector3.zero;
					}
				}
			}
		}

		// Token: 0x0600624D RID: 25165 RVA: 0x001FB9BC File Offset: 0x001F9BBC
		public void ClearAllRigOverrides()
		{
			VRRigCache.Instance.GetActiveRigs(this.rigs);
			for (int i = 0; i < this.rigs.Count; i++)
			{
				this.rigs[i].InOverrideSubscriptionZone = false;
				this.rigs[i].OverrideSubscriptionZoneLocation = Vector3.zero;
			}
		}

		// Token: 0x0600624E RID: 25166 RVA: 0x001FBA17 File Offset: 0x001F9C17
		private void OnDestroy()
		{
			if (this.tempEjectionObject != null)
			{
				Object.Destroy(this.tempEjectionObject);
			}
		}

		// Token: 0x0600624F RID: 25167 RVA: 0x001FBA34 File Offset: 0x001F9C34
		private void OnDrawGizmos()
		{
			if (this.restrictedZoneCollider != null)
			{
				Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
				Gizmos.DrawCube(this.restrictedZoneCollider.bounds.center, this.restrictedZoneCollider.bounds.size);
			}
			if (this.influenceZoneCollider != null)
			{
				Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
				Gizmos.DrawCube(this.influenceZoneCollider.bounds.center, this.influenceZoneCollider.bounds.size);
			}
			if (this.ejectionPoint != null)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(this.ejectionPoint.position, 0.5f);
			}
		}

		// Token: 0x04007110 RID: 28944
		[Header("Zones")]
		[Tooltip("Inner restricted zone - hard pushback")]
		[SerializeField]
		private GameObject restrictedZone;

		// Token: 0x04007111 RID: 28945
		[Tooltip("Outer influence zone - gentle drift")]
		[SerializeField]
		private GameObject warningZone;

		// Token: 0x04007112 RID: 28946
		[Header("Safe Exit Point")]
		[SerializeField]
		private Transform ejectionPoint;

		// Token: 0x04007113 RID: 28947
		[Header("Tuning")]
		[SerializeField]
		private float driftSpeed = 3f;

		// Token: 0x04007114 RID: 28948
		[SerializeField]
		private float shoveCooldown = 0.5f;

		// Token: 0x04007115 RID: 28949
		[Header("Safety")]
		[SerializeField]
		private float safetyCheckRadius = 0.5f;

		// Token: 0x04007116 RID: 28950
		[SerializeField]
		private LayerMask obstacleLayers;

		// Token: 0x04007117 RID: 28951
		[Header("Door Visuals")]
		[SerializeField]
		private GameObject nonSubscribeDoorObject;

		// Token: 0x04007118 RID: 28952
		[SerializeField]
		private GameObject subscriberDoorObject;

		// Token: 0x04007119 RID: 28953
		public UnityEvent OnWarning;

		// Token: 0x0400711A RID: 28954
		public UnityEvent OnEnterRestrictedZone;

		// Token: 0x0400711B RID: 28955
		[Header("Debug")]
		[SerializeField]
		private bool showDebugInfo;

		// Token: 0x0400711C RID: 28956
		private bool insideRestricted;

		// Token: 0x0400711D RID: 28957
		private bool insideInfluence;

		// Token: 0x0400711E RID: 28958
		private float lastShoveTime;

		// Token: 0x0400711F RID: 28959
		private GameObject tempEjectionObject;

		// Token: 0x04007120 RID: 28960
		private Collider restrictedZoneCollider;

		// Token: 0x04007121 RID: 28961
		private Collider influenceZoneCollider;

		// Token: 0x04007122 RID: 28962
		private bool bodyColliderWasDisabled;

		// Token: 0x04007123 RID: 28963
		private List<VRRig> rigs = new List<VRRig>();
	}
}
