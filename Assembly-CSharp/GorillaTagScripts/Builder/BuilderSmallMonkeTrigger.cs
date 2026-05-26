using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FBD RID: 4029
	public class BuilderSmallMonkeTrigger : MonoBehaviour
	{
		// Token: 0x17000973 RID: 2419
		// (get) Token: 0x060064BB RID: 25787 RVA: 0x00207820 File Offset: 0x00205A20
		public int overlapCount
		{
			get
			{
				return this.overlappingColliders.Count;
			}
		}

		// Token: 0x17000974 RID: 2420
		// (get) Token: 0x060064BC RID: 25788 RVA: 0x0020782D File Offset: 0x00205A2D
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x140000A9 RID: 169
		// (add) Token: 0x060064BD RID: 25789 RVA: 0x0020783C File Offset: 0x00205A3C
		// (remove) Token: 0x060064BE RID: 25790 RVA: 0x00207874 File Offset: 0x00205A74
		public event Action<int> onPlayerEnteredTrigger;

		// Token: 0x140000AA RID: 170
		// (add) Token: 0x060064BF RID: 25791 RVA: 0x002078AC File Offset: 0x00205AAC
		// (remove) Token: 0x060064C0 RID: 25792 RVA: 0x002078E4 File Offset: 0x00205AE4
		public event Action onTriggerFirstEntered;

		// Token: 0x140000AB RID: 171
		// (add) Token: 0x060064C1 RID: 25793 RVA: 0x0020791C File Offset: 0x00205B1C
		// (remove) Token: 0x060064C2 RID: 25794 RVA: 0x00207954 File Offset: 0x00205B54
		public event Action onTriggerLastExited;

		// Token: 0x060064C3 RID: 25795 RVA: 0x0020798C File Offset: 0x00205B8C
		public void ValidateOverlappingColliders()
		{
			for (int i = this.overlappingColliders.Count - 1; i >= 0; i--)
			{
				if (this.overlappingColliders[i] == null || !this.overlappingColliders[i].gameObject.activeInHierarchy || !this.overlappingColliders[i].enabled)
				{
					this.overlappingColliders.RemoveAt(i);
				}
				else
				{
					VRRig vrrig = this.overlappingColliders[i].attachedRigidbody.gameObject.GetComponent<VRRig>();
					if (vrrig == null)
					{
						if (GTPlayer.Instance.bodyCollider == this.overlappingColliders[i] || GTPlayer.Instance.headCollider == this.overlappingColliders[i])
						{
							vrrig = GorillaTagger.Instance.offlineVRRig;
						}
						else
						{
							this.overlappingColliders.RemoveAt(i);
						}
					}
					if (!this.ignoreScale && vrrig != null && (double)vrrig.scaleFactor > 0.99)
					{
						this.overlappingColliders.RemoveAt(i);
					}
				}
			}
		}

		// Token: 0x060064C4 RID: 25796 RVA: 0x00207AB0 File Offset: 0x00205CB0
		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody == null)
			{
				return;
			}
			VRRig vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (vrrig == null)
			{
				if (!(GTPlayer.Instance.bodyCollider == other) && !(GTPlayer.Instance.headCollider == other))
				{
					return;
				}
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (!this.hasCheckedZone)
			{
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(vrrig.zoneEntity.currentZone, out builderTable))
				{
					this.ignoreScale = !builderTable.isTableMutable;
				}
				this.hasCheckedZone = true;
			}
			if (!this.ignoreScale && (double)vrrig.scaleFactor > 0.99)
			{
				return;
			}
			if (vrrig != null)
			{
				Action<int> action = this.onPlayerEnteredTrigger;
				if (action != null)
				{
					action(vrrig.OwningNetPlayer.ActorNumber);
				}
			}
			bool flag = this.overlappingColliders.Count == 0;
			if (!this.overlappingColliders.Contains(other))
			{
				this.overlappingColliders.Add(other);
			}
			this.lastTriggeredFrame = Time.frameCount;
			if (flag)
			{
				Action action2 = this.onTriggerFirstEntered;
				if (action2 == null)
				{
					return;
				}
				action2();
			}
		}

		// Token: 0x060064C5 RID: 25797 RVA: 0x00207BCF File Offset: 0x00205DCF
		private void OnTriggerExit(Collider other)
		{
			if (this.overlappingColliders.Remove(other) && this.overlappingColliders.Count == 0)
			{
				Action action = this.onTriggerLastExited;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x040073B0 RID: 29616
		private int lastTriggeredFrame = -1;

		// Token: 0x040073B1 RID: 29617
		private List<Collider> overlappingColliders = new List<Collider>(20);

		// Token: 0x040073B5 RID: 29621
		private bool hasCheckedZone;

		// Token: 0x040073B6 RID: 29622
		private bool ignoreScale;
	}
}
