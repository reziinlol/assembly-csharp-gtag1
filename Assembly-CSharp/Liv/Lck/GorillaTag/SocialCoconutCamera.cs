using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	// Token: 0x020010C5 RID: 4293
	public class SocialCoconutCamera : MonoBehaviour
	{
		// Token: 0x06006B8A RID: 27530 RVA: 0x0022CD37 File Offset: 0x0022AF37
		private void Awake()
		{
			if (this._propertyBlock == null)
			{
				this._propertyBlock = new MaterialPropertyBlock();
			}
			this._propertyBlock.SetInt(this.IS_RECORDING, 0);
			this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
		}

		// Token: 0x06006B8B RID: 27531 RVA: 0x0022CD6F File Offset: 0x0022AF6F
		public void SetVisualsActive(bool active)
		{
			this._isActive = active;
			this._visuals.SetActive(active);
		}

		// Token: 0x06006B8C RID: 27532 RVA: 0x0022CD84 File Offset: 0x0022AF84
		public void SetRecordingState(bool isRecording)
		{
			if (!this._isActive)
			{
				return;
			}
			this._propertyBlock.SetInt(this.IS_RECORDING, isRecording ? 1 : 0);
			this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
		}

		// Token: 0x04007BA3 RID: 31651
		[SerializeField]
		private GameObject _visuals;

		// Token: 0x04007BA4 RID: 31652
		[SerializeField]
		private MeshRenderer _bodyRenderer;

		// Token: 0x04007BA5 RID: 31653
		private bool _isActive;

		// Token: 0x04007BA6 RID: 31654
		private MaterialPropertyBlock _propertyBlock;

		// Token: 0x04007BA7 RID: 31655
		private string IS_RECORDING = "_Is_Recording";
	}
}
