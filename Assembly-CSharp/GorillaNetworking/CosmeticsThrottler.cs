using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02001042 RID: 4162
	public class CosmeticsThrottler : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06006818 RID: 26648 RVA: 0x00219674 File Offset: 0x00217874
		private void Awake()
		{
			this._cosmeticSlots = 16;
			VRRig[] allRigs = VRRigCache.Instance.GetAllRigs();
			this._rigHelpers = new GorillaRigHelper[allRigs.Length];
			for (int i = 0; i < allRigs.Length; i++)
			{
				this._rigHelpers[i] = new GorillaRigHelper
				{
					rig = allRigs[i],
					state = CosmeticsThrottler.RigDrawState.Startup,
					sqrDistance = 9999f,
					prevSqrDistance = 9999f
				};
			}
			RoomSystem.JoinedRoomEvent += new Action(this.UpdatePlayerCount);
			RoomSystem.LeftRoomEvent += new Action(this.UpdatePlayerCount);
		}

		// Token: 0x06006819 RID: 26649 RVA: 0x00219728 File Offset: 0x00217928
		private void UpdatePlayerCount()
		{
			int num = NetworkSystem.Instance.AllNetPlayers.Length;
			if (num < this.ThrottlePlayerCountThreshold && this.lastPlayerCount >= this.ThrottlePlayerCountThreshold)
			{
				this.EnableAllRenderers();
			}
			this.lastPlayerCount = num;
		}

		// Token: 0x0600681A RID: 26650 RVA: 0x000DCF37 File Offset: 0x000DB137
		private void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this);
		}

		// Token: 0x0600681B RID: 26651 RVA: 0x000DCF3F File Offset: 0x000DB13F
		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this);
		}

		// Token: 0x0600681C RID: 26652 RVA: 0x00219768 File Offset: 0x00217968
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(base.transform.position, this.DrawAllDistance);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position, this.MaxDrawDistance);
		}

		// Token: 0x0600681D RID: 26653 RVA: 0x002197B8 File Offset: 0x002179B8
		public void SliceUpdate()
		{
			if (this.lastPlayerCount < this.ThrottlePlayerCountThreshold)
			{
				return;
			}
			if (this.mainCamera == null)
			{
				this.mainCamera = Camera.main;
				return;
			}
			Vector3 position = base.transform.position;
			for (int i = 0; i < this._rigHelpers.Length; i++)
			{
				this._rigHelpers[i].prevSqrDistance = this._rigHelpers[i].sqrDistance;
				if (!this._rigHelpers[i].rig.isActiveAndEnabled || this._rigHelpers[i].rig.isLocal)
				{
					this._rigHelpers[i].sqrDistance = 9999f;
				}
				else
				{
					Vector3 position2 = this._rigHelpers[i].rig.transform.position;
					if (this.mainCamera.WorldToScreenPoint(position2).z <= 0f)
					{
						this._rigHelpers[i].sqrDistance = 9999f;
					}
					else
					{
						float sqrMagnitude = (position2 - position).sqrMagnitude;
						this._rigHelpers[i].sqrDistance = sqrMagnitude;
					}
				}
			}
			Array.Sort<GorillaRigHelper>(this._rigHelpers);
			float num = this.DrawAllDistance * this.DrawAllDistance;
			float num2 = this.MaxDrawDistance * this.MaxDrawDistance;
			for (int j = 0; j < this._rigHelpers.Length; j++)
			{
				if (this._rigHelpers[j].sqrDistance >= 9999f)
				{
					this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.Min);
				}
				else
				{
					if (this.DrawOnPlayerCount)
					{
						if (j < this.DrawAllCount)
						{
							this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.All);
							goto IL_293;
						}
						if (j >= this.DrawMaxCount)
						{
							this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.Min);
							goto IL_293;
						}
					}
					if (this._rigHelpers[j].sqrDistance <= num)
					{
						this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.All);
					}
					else if (this._rigHelpers[j].sqrDistance <= num2)
					{
						this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.Partial);
					}
					else
					{
						this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.Min);
					}
				}
				IL_293:;
			}
		}

		// Token: 0x0600681E RID: 26654 RVA: 0x00219A70 File Offset: 0x00217C70
		private GorillaRigHelper UpdateRigState(GorillaRigHelper helper, CosmeticsThrottler.RigDrawState newState)
		{
			CosmeticsThrottler.RigDrawState state = helper.state;
			if (newState == state)
			{
				return helper;
			}
			switch (newState)
			{
			case CosmeticsThrottler.RigDrawState.All:
				if (state != CosmeticsThrottler.RigDrawState.All)
				{
					this.ToggleRenderersOnRig(helper.rig, true);
					helper.rig.ToggleMatParticles(true);
				}
				break;
			case CosmeticsThrottler.RigDrawState.Partial:
				if (state <= CosmeticsThrottler.RigDrawState.All)
				{
					this.ToggleRenderersOnRigForSlots(helper.rig, false, true);
					helper.rig.ToggleMatParticles(false);
				}
				else if (state == CosmeticsThrottler.RigDrawState.Min)
				{
					this.ToggleRenderersOnRigForSlots(helper.rig, true, false);
				}
				break;
			case CosmeticsThrottler.RigDrawState.Min:
				if (state != CosmeticsThrottler.RigDrawState.Min)
				{
					this.ToggleRenderersOnRig(helper.rig, false);
					helper.rig.ToggleMatParticles(false);
				}
				break;
			}
			helper.state = newState;
			return helper;
		}

		// Token: 0x0600681F RID: 26655 RVA: 0x00219B18 File Offset: 0x00217D18
		private void ToggleRenderersOnRig(VRRig rig, bool toggle)
		{
			CosmeticsController.CosmeticSet cosmeticSet = rig.cosmeticSet;
			int num = cosmeticSet.items.Length;
			for (int i = 0; i < num; i++)
			{
				CosmeticItemInstance cosmeticItemInstance = rig.cosmeticsObjectRegistry.Cosmetic(cosmeticSet.items[i].displayName);
				if (cosmeticItemInstance != null)
				{
					cosmeticItemInstance.ToggleRenderers(toggle);
					cosmeticItemInstance.ToggleParticles(toggle);
				}
			}
		}

		// Token: 0x06006820 RID: 26656 RVA: 0x00219B70 File Offset: 0x00217D70
		private void ToggleRenderersOnRigForSlots(VRRig rig, bool toggle, bool includesSlots = true)
		{
			CosmeticsController.CosmeticSet cosmeticSet = rig.cosmeticSet;
			int num = cosmeticSet.items.Length;
			for (int i = 0; i < num; i++)
			{
				CosmeticItemInstance cosmeticItemInstance = rig.cosmeticsObjectRegistry.Cosmetic(cosmeticSet.items[i].displayName);
				if (cosmeticItemInstance != null)
				{
					cosmeticItemInstance.ToggleParticles(toggle);
					if (this.ContainsSlot(cosmeticItemInstance.ActiveSlot) == includesSlots)
					{
						cosmeticItemInstance.ToggleRenderers(toggle);
					}
				}
			}
		}

		// Token: 0x06006821 RID: 26657 RVA: 0x00219BD8 File Offset: 0x00217DD8
		private bool ContainsSlot(CosmeticsController.CosmeticSlots slot)
		{
			for (int i = 0; i < this.ToggleSlots.Length; i++)
			{
				if (this.ToggleSlots[i] == slot)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006822 RID: 26658 RVA: 0x00219C08 File Offset: 0x00217E08
		public void EnableAllRenderers()
		{
			for (int i = 0; i < this._rigHelpers.Length; i++)
			{
				this.ToggleRenderersOnRig(this._rigHelpers[i].rig, true);
			}
		}

		// Token: 0x0400776B RID: 30571
		public float DrawAllDistance = 5f;

		// Token: 0x0400776C RID: 30572
		public float MaxDrawDistance = 10f;

		// Token: 0x0400776D RID: 30573
		public bool DrawOnPlayerCount = true;

		// Token: 0x0400776E RID: 30574
		public int DrawAllCount = 6;

		// Token: 0x0400776F RID: 30575
		public int DrawMaxCount = 14;

		// Token: 0x04007770 RID: 30576
		public int ThrottlePlayerCountThreshold = 11;

		// Token: 0x04007771 RID: 30577
		private int lastPlayerCount;

		// Token: 0x04007772 RID: 30578
		public CosmeticsController.CosmeticSlots[] ToggleSlots;

		// Token: 0x04007773 RID: 30579
		[SerializeField]
		private GorillaRigHelper[] _rigHelpers;

		// Token: 0x04007774 RID: 30580
		private int _cosmeticSlots;

		// Token: 0x04007775 RID: 30581
		private float _update;

		// Token: 0x04007776 RID: 30582
		private Camera mainCamera;

		// Token: 0x02001043 RID: 4163
		public enum RigDrawState
		{
			// Token: 0x04007778 RID: 30584
			All,
			// Token: 0x04007779 RID: 30585
			Partial,
			// Token: 0x0400777A RID: 30586
			Min,
			// Token: 0x0400777B RID: 30587
			Startup = -1
		}
	}
}
