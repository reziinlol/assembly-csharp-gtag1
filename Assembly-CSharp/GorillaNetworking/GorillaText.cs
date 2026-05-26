using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaNetworking
{
	// Token: 0x02001059 RID: 4185
	[Serializable]
	public class GorillaText
	{
		// Token: 0x0600691D RID: 26909 RVA: 0x00220650 File Offset: 0x0021E850
		public void Initialize(Material[] originalMaterials, Material failureMaterial, UnityEvent<string> callback = null, UnityEvent<Material[]> materialCallback = null)
		{
			this.failureMaterial = failureMaterial;
			this.originalMaterials = originalMaterials;
			this.currentMaterials = originalMaterials;
			Debug.Log("Original text = " + this.originalText);
			this.updateTextCallback = callback;
			this.updateMaterialCallback = materialCallback;
			GorillaTextManager.RegisterText(this);
		}

		// Token: 0x0600691E RID: 26910 RVA: 0x0022069C File Offset: 0x0021E89C
		public void InvokeIfUpdated()
		{
			if (!this.modified)
			{
				return;
			}
			this.modified = false;
			string b = this.stringBuilder.ToString();
			if (this.currentText != b)
			{
				this.currentText = b;
				UnityEvent<string> unityEvent = this.updateTextCallback;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(this.currentText);
			}
		}

		// Token: 0x0600691F RID: 26911 RVA: 0x002206F0 File Offset: 0x0021E8F0
		public void EnableFailedState(string failText)
		{
			this.failedState = true;
			this.failureText = failText;
			UnityEvent<string> unityEvent = this.updateTextCallback;
			if (unityEvent != null)
			{
				unityEvent.Invoke(failText);
			}
			this.originalText = this.currentText;
			this.currentText = failText;
			this.currentMaterials = (Material[])this.originalMaterials.Clone();
			this.currentMaterials[0] = this.failureMaterial;
			UnityEvent<Material[]> unityEvent2 = this.updateMaterialCallback;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(this.currentMaterials);
		}

		// Token: 0x06006920 RID: 26912 RVA: 0x0022076C File Offset: 0x0021E96C
		public void DisableFailedState()
		{
			this.failedState = false;
			UnityEvent<string> unityEvent = this.updateTextCallback;
			if (unityEvent != null)
			{
				unityEvent.Invoke(this.originalText);
			}
			this.failureText = "";
			this.currentText = this.originalText;
			this.currentMaterials = this.originalMaterials;
			UnityEvent<Material[]> unityEvent2 = this.updateMaterialCallback;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(this.currentMaterials);
		}

		// Token: 0x06006921 RID: 26913 RVA: 0x002207D0 File Offset: 0x0021E9D0
		public void Append(string str)
		{
			this.modified = true;
			this.stringBuilder.Append(str);
		}

		// Token: 0x06006922 RID: 26914 RVA: 0x002207E6 File Offset: 0x0021E9E6
		public void Set(string str)
		{
			this.modified = true;
			this.stringBuilder.Clear();
			this.stringBuilder.Append(str);
		}

		// Token: 0x04007952 RID: 31058
		private string failureText;

		// Token: 0x04007953 RID: 31059
		public string currentText;

		// Token: 0x04007954 RID: 31060
		private string originalText = string.Empty;

		// Token: 0x04007955 RID: 31061
		private StringBuilder stringBuilder = new StringBuilder();

		// Token: 0x04007956 RID: 31062
		private bool modified;

		// Token: 0x04007957 RID: 31063
		private bool failedState;

		// Token: 0x04007958 RID: 31064
		private Material[] originalMaterials;

		// Token: 0x04007959 RID: 31065
		private Material failureMaterial;

		// Token: 0x0400795A RID: 31066
		internal Material[] currentMaterials;

		// Token: 0x0400795B RID: 31067
		private UnityEvent<string> updateTextCallback;

		// Token: 0x0400795C RID: 31068
		private UnityEvent<Material[]> updateMaterialCallback;
	}
}
