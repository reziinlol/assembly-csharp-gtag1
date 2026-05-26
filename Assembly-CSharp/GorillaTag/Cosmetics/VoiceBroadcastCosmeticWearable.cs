using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012B6 RID: 4790
	public class VoiceBroadcastCosmeticWearable : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x060077D4 RID: 30676 RVA: 0x00274D54 File Offset: 0x00272F54
		private void Start()
		{
			VoiceBroadcastCosmetic[] componentsInChildren = base.GetComponentInParent<VRRig>().GetComponentsInChildren<VoiceBroadcastCosmetic>(true);
			this.voiceBroadcasters = new List<VoiceBroadcastCosmetic>();
			foreach (VoiceBroadcastCosmetic voiceBroadcastCosmetic in componentsInChildren)
			{
				if (voiceBroadcastCosmetic.talkingCosmeticType == this.talkingCosmeticType)
				{
					this.voiceBroadcasters.Add(voiceBroadcastCosmetic);
					voiceBroadcastCosmetic.SetWearable(this);
				}
			}
		}

		// Token: 0x060077D5 RID: 30677 RVA: 0x00274DAC File Offset: 0x00272FAC
		public void OnEnable()
		{
			if (this.playerHeadCollider == null)
			{
				VRRig componentInParent = base.GetComponentInParent<VRRig>();
				this.playerHeadCollider = ((componentInParent != null) ? componentInParent.rigContainer.HeadCollider : null);
			}
			if (this.headDistanceActivation && this.playerHeadCollider != null)
			{
				GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			}
		}

		// Token: 0x060077D6 RID: 30678 RVA: 0x00011DE0 File Offset: 0x0000FFE0
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x060077D7 RID: 30679 RVA: 0x00274E04 File Offset: 0x00273004
		public void SliceUpdate()
		{
			if (Time.time - this.lastToggleTime >= this.toggleCooldown)
			{
				bool flag = (base.transform.position - this.playerHeadCollider.transform.position).sqrMagnitude <= this.headDistance * this.headDistance;
				if (flag != this.toggleState)
				{
					this.toggleState = flag;
					this.lastToggleTime = Time.time;
					if (flag)
					{
						UnityEvent unityEvent = this.onStartListening;
						if (unityEvent != null)
						{
							unityEvent.Invoke();
						}
					}
					else
					{
						UnityEvent unityEvent2 = this.onStopListening;
						if (unityEvent2 != null)
						{
							unityEvent2.Invoke();
						}
					}
					for (int i = 0; i < this.voiceBroadcasters.Count; i++)
					{
						this.voiceBroadcasters[i].SetListenState(flag);
					}
				}
			}
		}

		// Token: 0x060077D8 RID: 30680 RVA: 0x00274ECD File Offset: 0x002730CD
		public void OnCosmeticStartListening()
		{
			if (this.headDistanceActivation)
			{
				return;
			}
			UnityEvent unityEvent = this.onStartListening;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x060077D9 RID: 30681 RVA: 0x00274EE8 File Offset: 0x002730E8
		public void OnCosmeticStopListening()
		{
			if (this.headDistanceActivation)
			{
				return;
			}
			UnityEvent unityEvent = this.onStopListening;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x04008A99 RID: 35481
		public TalkingCosmeticType talkingCosmeticType;

		// Token: 0x04008A9A RID: 35482
		[SerializeField]
		private bool headDistanceActivation = true;

		// Token: 0x04008A9B RID: 35483
		[SerializeField]
		private float headDistance = 0.4f;

		// Token: 0x04008A9C RID: 35484
		[SerializeField]
		private float toggleCooldown = 0.5f;

		// Token: 0x04008A9D RID: 35485
		private bool toggleState;

		// Token: 0x04008A9E RID: 35486
		private float lastToggleTime;

		// Token: 0x04008A9F RID: 35487
		[SerializeField]
		private UnityEvent onStartListening;

		// Token: 0x04008AA0 RID: 35488
		[SerializeField]
		private UnityEvent onStopListening;

		// Token: 0x04008AA1 RID: 35489
		private List<VoiceBroadcastCosmetic> voiceBroadcasters;

		// Token: 0x04008AA2 RID: 35490
		private Collider playerHeadCollider;
	}
}
