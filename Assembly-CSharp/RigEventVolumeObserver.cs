using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020004DC RID: 1244
public class RigEventVolumeObserver : MonoBehaviour
{
	// Token: 0x06001E4B RID: 7755 RVA: 0x000A2420 File Offset: 0x000A0620
	private void Awake()
	{
		for (int i = 0; i < this.tMP_Texts.Length; i++)
		{
			this.formats.Add(this.tMP_Texts[i].text);
		}
	}

	// Token: 0x06001E4C RID: 7756 RVA: 0x000A2458 File Offset: 0x000A0658
	private void OnEnable()
	{
		this.Observed_OnCountChanged();
		this.observed.OnCountChanged += this.Observed_OnCountChanged;
	}

	// Token: 0x06001E4D RID: 7757 RVA: 0x000A2477 File Offset: 0x000A0677
	private void OnDisable()
	{
		this.observed.OnCountChanged -= this.Observed_OnCountChanged;
	}

	// Token: 0x06001E4E RID: 7758 RVA: 0x000A2490 File Offset: 0x000A0690
	private void Observed_OnCountChanged()
	{
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			this.gameObjects[i].ApplyActiveState(this.observed);
		}
		for (int j = 0; j < this.tMP_Texts.Length; j++)
		{
			this.tMP_Texts[j].text = this.Format(this.formats[j]);
		}
	}

	// Token: 0x06001E4F RID: 7759 RVA: 0x000A24F8 File Offset: 0x000A06F8
	private string Format(string s)
	{
		return s.Replace("\\c", this.observed.RigCount.ToString());
	}

	// Token: 0x04002871 RID: 10353
	[SerializeField]
	private RigEventVolume observed;

	// Token: 0x04002872 RID: 10354
	[SerializeField]
	private RigEventVolumeObserver.RigEventVolumeObserverGameObject[] gameObjects;

	// Token: 0x04002873 RID: 10355
	[SerializeField]
	private TMP_Text[] tMP_Texts;

	// Token: 0x04002874 RID: 10356
	private List<string> formats = new List<string>();

	// Token: 0x020004DD RID: 1245
	[Serializable]
	private class RigEventVolumeObserverGameObject
	{
		// Token: 0x06001E51 RID: 7761 RVA: 0x000A2538 File Offset: 0x000A0738
		public bool Check(RigEventVolume rev)
		{
			switch (this.comparison)
			{
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.EQ:
				return rev.RigCount == this.value;
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.LT:
				return rev.RigCount < this.value;
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.GT:
				return rev.RigCount > this.value;
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.LT_EQ:
				return rev.RigCount <= this.value;
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.GT_EQ:
				return rev.RigCount >= this.value;
			case RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison.NEQ:
				return rev.RigCount != this.value;
			default:
				return false;
			}
		}

		// Token: 0x06001E52 RID: 7762 RVA: 0x000A25D0 File Offset: 0x000A07D0
		public void ApplyActiveState(RigEventVolume rev)
		{
			this.gameObject.SetActive(this.Check(rev));
		}

		// Token: 0x04002875 RID: 10357
		[SerializeField]
		private GameObject gameObject;

		// Token: 0x04002876 RID: 10358
		[SerializeField]
		public RigEventVolumeObserver.RigEventVolumeObserverGameObject.Comparison comparison;

		// Token: 0x04002877 RID: 10359
		[SerializeField]
		public int value;

		// Token: 0x020004DE RID: 1246
		public enum Comparison
		{
			// Token: 0x04002879 RID: 10361
			EQ,
			// Token: 0x0400287A RID: 10362
			LT,
			// Token: 0x0400287B RID: 10363
			GT,
			// Token: 0x0400287C RID: 10364
			LT_EQ,
			// Token: 0x0400287D RID: 10365
			GT_EQ,
			// Token: 0x0400287E RID: 10366
			NEQ
		}
	}
}
