using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020011F7 RID: 4599
	public class LoudSpeakerActivator : MonoBehaviour
	{
		// Token: 0x06007364 RID: 29540 RVA: 0x00257FB3 File Offset: 0x002561B3
		private void Awake()
		{
			this._isLocal = this.IsParentedToLocalRig();
			if (!this._isLocal)
			{
				this._nonlocalRig = base.transform.root.GetComponent<VRRig>();
			}
		}

		// Token: 0x06007365 RID: 29541 RVA: 0x00257FE0 File Offset: 0x002561E0
		private bool IsParentedToLocalRig()
		{
			if (VRRigCache.Instance.localRig == null)
			{
				return false;
			}
			Transform parent = base.transform.parent;
			while (parent != null)
			{
				if (parent == VRRigCache.Instance.localRig.transform)
				{
					return true;
				}
				parent = parent.parent;
			}
			return false;
		}

		// Token: 0x06007366 RID: 29542 RVA: 0x00258039 File Offset: 0x00256239
		public void SetRecorder(GTRecorder recorder)
		{
			this._recorder = recorder;
		}

		// Token: 0x06007367 RID: 29543 RVA: 0x00258044 File Offset: 0x00256244
		public void StartLocalBroadcast()
		{
			if (!this._isLocal)
			{
				if (this._network != null && this._nonlocalRig != null)
				{
					this._network.StartBroadcastSpeakerOutput(this._nonlocalRig);
				}
				return;
			}
			if (this.IsBroadcasting)
			{
				return;
			}
			if (this._recorder == null && NetworkSystem.Instance.LocalRecorder != null)
			{
				this.SetRecorder((GTRecorder)NetworkSystem.Instance.LocalRecorder);
			}
			if (this._recorder != null && this._network != null)
			{
				this.IsBroadcasting = true;
				this._recorder.AllowPitchAdjustment = true;
				this._recorder.PitchAdjustment = this.PitchAdjustment;
				this._recorder.AllowVolumeAdjustment = true;
				this._recorder.VolumeAdjustment = this.VolumeAdjustment;
				this._network.StartBroadcastSpeakerOutput(VRRigCache.Instance.localRig.Rig);
			}
		}

		// Token: 0x06007368 RID: 29544 RVA: 0x0025813C File Offset: 0x0025633C
		public void StopLocalBroadcast()
		{
			if (!this._isLocal)
			{
				if (this._network != null && this._nonlocalRig != null)
				{
					this._network.StopBroadcastSpeakerOutput(this._nonlocalRig);
				}
				return;
			}
			if (!this.IsBroadcasting)
			{
				return;
			}
			if (this._recorder == null && NetworkSystem.Instance.LocalRecorder != null)
			{
				this.SetRecorder((GTRecorder)NetworkSystem.Instance.LocalRecorder);
			}
			if (this._recorder != null && this._network != null)
			{
				this.IsBroadcasting = false;
				this._recorder.AllowPitchAdjustment = false;
				this._recorder.PitchAdjustment = 1f;
				this._recorder.AllowVolumeAdjustment = false;
				this._recorder.VolumeAdjustment = 1f;
				this._network.StopBroadcastSpeakerOutput(VRRigCache.Instance.localRig.Rig);
			}
		}

		// Token: 0x040083C8 RID: 33736
		public float PitchAdjustment = 1f;

		// Token: 0x040083C9 RID: 33737
		public float VolumeAdjustment = 2.5f;

		// Token: 0x040083CA RID: 33738
		public bool IsBroadcasting;

		// Token: 0x040083CB RID: 33739
		[SerializeField]
		private LoudSpeakerNetwork _network;

		// Token: 0x040083CC RID: 33740
		[SerializeField]
		private GTRecorder _recorder;

		// Token: 0x040083CD RID: 33741
		private bool _isLocal;

		// Token: 0x040083CE RID: 33742
		private VRRig _nonlocalRig;
	}
}
