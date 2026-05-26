using System;
using System.Collections;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020011F3 RID: 4595
	public class GTRecorder : Recorder, ITickSystemPost
	{
		// Token: 0x17000B0E RID: 2830
		// (get) Token: 0x06007348 RID: 29512 RVA: 0x00257AA6 File Offset: 0x00255CA6
		// (set) Token: 0x06007349 RID: 29513 RVA: 0x00257AAE File Offset: 0x00255CAE
		public bool PostTickRunning { get; set; }

		// Token: 0x0600734A RID: 29514 RVA: 0x001A578D File Offset: 0x001A398D
		private void OnEnable()
		{
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x0600734B RID: 29515 RVA: 0x00156E8B File Offset: 0x0015508B
		private void OnDisable()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x0600734C RID: 29516 RVA: 0x00257AB7 File Offset: 0x00255CB7
		protected override MicWrapper CreateMicWrapper(string micDev, int samplingRateInt, VoiceLogger logger)
		{
			this._micWrapper = new GTMicWrapper(micDev, samplingRateInt, this.AllowPitchAdjustment, this.PitchAdjustment, this.AllowVolumeAdjustment, this.VolumeAdjustment, logger);
			return this._micWrapper;
		}

		// Token: 0x0600734D RID: 29517 RVA: 0x00257AE5 File Offset: 0x00255CE5
		private IEnumerator DoTestEcho()
		{
			base.DebugEchoMode = true;
			yield return new WaitForSeconds(this.DebugEchoLength);
			base.DebugEchoMode = false;
			yield return null;
			this._testEchoCoroutine = null;
			yield break;
		}

		// Token: 0x0600734E RID: 29518 RVA: 0x00257AF4 File Offset: 0x00255CF4
		public void PostTick()
		{
			if (this._micWrapper != null)
			{
				this._micWrapper.UpdateWrapper(this.AllowPitchAdjustment, this.PitchAdjustment, this.AllowVolumeAdjustment, this.VolumeAdjustment);
			}
		}

		// Token: 0x040083B2 RID: 33714
		public bool AllowPitchAdjustment;

		// Token: 0x040083B3 RID: 33715
		public float PitchAdjustment = 1f;

		// Token: 0x040083B4 RID: 33716
		public bool AllowVolumeAdjustment;

		// Token: 0x040083B5 RID: 33717
		public float VolumeAdjustment = 1f;

		// Token: 0x040083B6 RID: 33718
		public float DebugEchoLength = 5f;

		// Token: 0x040083B7 RID: 33719
		private GTMicWrapper _micWrapper;

		// Token: 0x040083B8 RID: 33720
		private Coroutine _testEchoCoroutine;
	}
}
