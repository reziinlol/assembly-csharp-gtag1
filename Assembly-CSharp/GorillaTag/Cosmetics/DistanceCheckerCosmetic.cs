using System;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200126C RID: 4716
	public class DistanceCheckerCosmetic : MonoBehaviour, ISpawnable, IGorillaSliceableSimple
	{
		// Token: 0x17000B69 RID: 2921
		// (get) Token: 0x0600763E RID: 30270 RVA: 0x0026BA50 File Offset: 0x00269C50
		// (set) Token: 0x0600763F RID: 30271 RVA: 0x0026BA58 File Offset: 0x00269C58
		public bool IsSpawned { get; set; }

		// Token: 0x17000B6A RID: 2922
		// (get) Token: 0x06007640 RID: 30272 RVA: 0x0026BA61 File Offset: 0x00269C61
		// (set) Token: 0x06007641 RID: 30273 RVA: 0x0026BA69 File Offset: 0x00269C69
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06007642 RID: 30274 RVA: 0x0026BA72 File Offset: 0x00269C72
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x06007643 RID: 30275 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnDespawn()
		{
		}

		// Token: 0x06007644 RID: 30276 RVA: 0x0026BA7C File Offset: 0x00269C7C
		private void OnEnable()
		{
			this.currentState = DistanceCheckerCosmetic.State.None;
			this.transferableObject = base.GetComponentInParent<TransferrableObject>();
			if (this.transferableObject != null)
			{
				this.ownerRig = this.transferableObject.ownerRig;
			}
			else
			{
				this.ownerRig = base.GetComponentInParent<VRRig>();
			}
			if (this.ownerRig == null)
			{
				this.ownerRig = GorillaTagger.Instance.offlineVRRig;
			}
			this.ResetClosestPlayer();
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06007645 RID: 30277 RVA: 0x00018E11 File Offset: 0x00017011
		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06007646 RID: 30278 RVA: 0x0026BAF4 File Offset: 0x00269CF4
		public void SliceUpdate()
		{
			this.UpdateDistance();
		}

		// Token: 0x06007647 RID: 30279 RVA: 0x0026BAFC File Offset: 0x00269CFC
		private bool IsBelowThreshold(Vector3 distance)
		{
			return distance.IsShorterThan(this.distanceThreshold);
		}

		// Token: 0x06007648 RID: 30280 RVA: 0x0026BB0F File Offset: 0x00269D0F
		private bool IsAboveThreshold(Vector3 distance)
		{
			return distance.IsLongerThan(this.distanceThreshold);
		}

		// Token: 0x06007649 RID: 30281 RVA: 0x0026BB24 File Offset: 0x00269D24
		private void UpdateClosestPlayer(bool others = false)
		{
			if (!PhotonNetwork.InRoom)
			{
				this.ResetClosestPlayer();
				return;
			}
			VRRig y = this.currentClosestPlayer;
			this.closestDistance = Vector3.positiveInfinity;
			this.currentClosestPlayer = null;
			foreach (VRRig vrrig in VRRigCache.ActiveRigs)
			{
				if (!others || !(this.ownerRig != null) || !(vrrig == this.ownerRig))
				{
					Vector3 distance = vrrig.transform.position - this.distanceFrom.position;
					if (this.IsBelowThreshold(distance) && distance.sqrMagnitude < this.closestDistance.sqrMagnitude)
					{
						this.closestDistance = distance;
						this.currentClosestPlayer = vrrig;
					}
				}
			}
			if (this.currentClosestPlayer != null && this.currentClosestPlayer != y)
			{
				UnityEvent<VRRig, float> unityEvent = this.onClosestPlayerBelowThresholdChanged;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(this.currentClosestPlayer, this.closestDistance.magnitude);
			}
		}

		// Token: 0x0600764A RID: 30282 RVA: 0x0026BC34 File Offset: 0x00269E34
		private void ResetClosestPlayer()
		{
			this.closestDistance = Vector3.positiveInfinity;
			this.currentClosestPlayer = null;
		}

		// Token: 0x0600764B RID: 30283 RVA: 0x0026BC48 File Offset: 0x00269E48
		private void UpdateDistance()
		{
			bool flag = true;
			switch (this.distanceTo)
			{
			case DistanceCheckerCosmetic.DistanceCondition.Owner:
			{
				Vector3 distance = this.myRig.transform.position - this.distanceFrom.position;
				if (this.IsBelowThreshold(distance))
				{
					this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
					return;
				}
				if (this.IsAboveThreshold(distance))
				{
					this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
				}
				break;
			}
			case DistanceCheckerCosmetic.DistanceCondition.Others:
				this.UpdateClosestPlayer(true);
				if (!PhotonNetwork.InRoom)
				{
					return;
				}
				foreach (VRRig vrrig in VRRigCache.ActiveRigs)
				{
					if (!(this.ownerRig != null) || !(vrrig == this.ownerRig))
					{
						Vector3 distance2 = vrrig.transform.position - this.distanceFrom.position;
						if (this.IsBelowThreshold(distance2))
						{
							this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
							flag = false;
						}
					}
				}
				if (flag)
				{
					this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
					return;
				}
				break;
			case DistanceCheckerCosmetic.DistanceCondition.Everyone:
				this.UpdateClosestPlayer(false);
				if (!PhotonNetwork.InRoom)
				{
					return;
				}
				foreach (VRRig vrrig2 in VRRigCache.ActiveRigs)
				{
					Vector3 distance3 = vrrig2.transform.position - this.distanceFrom.position;
					if (this.IsBelowThreshold(distance3))
					{
						this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
						flag = false;
					}
				}
				if (flag)
				{
					this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x0600764C RID: 30284 RVA: 0x0026BDD8 File Offset: 0x00269FD8
		private void UpdateState(DistanceCheckerCosmetic.State newState)
		{
			if (this.currentState == newState)
			{
				return;
			}
			this.currentState = newState;
			if (this.currentState != DistanceCheckerCosmetic.State.AboveThreshold)
			{
				if (this.currentState == DistanceCheckerCosmetic.State.BelowThreshold)
				{
					UnityEvent unityEvent = this.onOneIsBelowThreshold;
					if (unityEvent == null)
					{
						return;
					}
					unityEvent.Invoke();
				}
				return;
			}
			UnityEvent unityEvent2 = this.onAllAreAboveThreshold;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke();
		}

		// Token: 0x04008802 RID: 34818
		[SerializeField]
		private Transform distanceFrom;

		// Token: 0x04008803 RID: 34819
		[SerializeField]
		private DistanceCheckerCosmetic.DistanceCondition distanceTo;

		// Token: 0x04008804 RID: 34820
		[Tooltip("Receive events when above or below this distance")]
		public float distanceThreshold;

		// Token: 0x04008805 RID: 34821
		public UnityEvent onOneIsBelowThreshold;

		// Token: 0x04008806 RID: 34822
		public UnityEvent onAllAreAboveThreshold;

		// Token: 0x04008807 RID: 34823
		public UnityEvent<VRRig, float> onClosestPlayerBelowThresholdChanged;

		// Token: 0x04008808 RID: 34824
		private VRRig myRig;

		// Token: 0x04008809 RID: 34825
		private DistanceCheckerCosmetic.State currentState;

		// Token: 0x0400880A RID: 34826
		private Vector3 closestDistance;

		// Token: 0x0400880B RID: 34827
		private VRRig currentClosestPlayer;

		// Token: 0x0400880C RID: 34828
		private VRRig ownerRig;

		// Token: 0x0400880D RID: 34829
		private TransferrableObject transferableObject;

		// Token: 0x0200126D RID: 4717
		private enum State
		{
			// Token: 0x04008811 RID: 34833
			AboveThreshold,
			// Token: 0x04008812 RID: 34834
			BelowThreshold,
			// Token: 0x04008813 RID: 34835
			None
		}

		// Token: 0x0200126E RID: 4718
		private enum DistanceCondition
		{
			// Token: 0x04008815 RID: 34837
			None,
			// Token: 0x04008816 RID: 34838
			Owner,
			// Token: 0x04008817 RID: 34839
			Others,
			// Token: 0x04008818 RID: 34840
			Everyone
		}
	}
}
