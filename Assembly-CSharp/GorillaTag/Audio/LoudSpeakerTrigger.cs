using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020011F9 RID: 4601
	public class LoudSpeakerTrigger : MonoBehaviour
	{
		// Token: 0x06007377 RID: 29559 RVA: 0x0025860E File Offset: 0x0025680E
		public void SetRecorder(GTRecorder recorder)
		{
			this._recorder = recorder;
		}

		// Token: 0x06007378 RID: 29560 RVA: 0x00258618 File Offset: 0x00256818
		public void OnPlayerEnter(VRRig player)
		{
			if (this._recorder != null && this._network != null)
			{
				this._recorder.AllowPitchAdjustment = true;
				this._recorder.PitchAdjustment = this.PitchAdjustment;
				this._network.StartBroadcastSpeakerOutput(player);
			}
		}

		// Token: 0x06007379 RID: 29561 RVA: 0x0025866C File Offset: 0x0025686C
		public void OnPlayerExit(VRRig player)
		{
			if (this._recorder != null && this._network != null)
			{
				this._recorder.AllowPitchAdjustment = false;
				this._recorder.PitchAdjustment = 1f;
				this._network.StopBroadcastSpeakerOutput(player);
			}
		}

		// Token: 0x040083D5 RID: 33749
		public float PitchAdjustment = 1f;

		// Token: 0x040083D6 RID: 33750
		[SerializeField]
		private LoudSpeakerNetwork _network;

		// Token: 0x040083D7 RID: 33751
		[SerializeField]
		private GTRecorder _recorder;
	}
}
