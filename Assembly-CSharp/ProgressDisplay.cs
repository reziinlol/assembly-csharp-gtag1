using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000250 RID: 592
public class ProgressDisplay : MonoBehaviour
{
	// Token: 0x06000FEC RID: 4076 RVA: 0x00055C3D File Offset: 0x00053E3D
	private void Reset()
	{
		this.root = base.gameObject;
	}

	// Token: 0x06000FED RID: 4077 RVA: 0x00055C4B File Offset: 0x00053E4B
	public void SetVisible(bool visible)
	{
		this.root.SetActive(visible);
	}

	// Token: 0x06000FEE RID: 4078 RVA: 0x00055C5C File Offset: 0x00053E5C
	public void SetProgress(int progress, int total)
	{
		if (this.text)
		{
			if (total < this.largestNumberToShow)
			{
				this.text.text = ((progress >= total) ? string.Format("{0}", total) : string.Format("{0}/{1}", progress, total));
				this.SetTextVisible(true);
			}
			else
			{
				this.SetTextVisible(false);
			}
		}
		this.progressImage.fillAmount = (float)progress / (float)total;
	}

	// Token: 0x06000FEF RID: 4079 RVA: 0x00055CD6 File Offset: 0x00053ED6
	public void SetProgress(float progress)
	{
		this.progressImage.fillAmount = progress;
	}

	// Token: 0x06000FF0 RID: 4080 RVA: 0x00055CE4 File Offset: 0x00053EE4
	private void SetTextVisible(bool visible)
	{
		if (this.text.gameObject.activeSelf == visible)
		{
			return;
		}
		this.text.gameObject.SetActive(visible);
	}

	// Token: 0x04001316 RID: 4886
	[SerializeField]
	private GameObject root;

	// Token: 0x04001317 RID: 4887
	[SerializeField]
	private TMP_Text text;

	// Token: 0x04001318 RID: 4888
	[SerializeField]
	private Image progressImage;

	// Token: 0x04001319 RID: 4889
	[SerializeField]
	private int largestNumberToShow = 99;
}
