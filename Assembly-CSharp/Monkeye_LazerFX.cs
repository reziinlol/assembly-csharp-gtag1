using System;
using UnityEngine;

// Token: 0x020001A9 RID: 425
public class Monkeye_LazerFX : MonoBehaviour
{
	// Token: 0x06000B8B RID: 2955 RVA: 0x0003E100 File Offset: 0x0003C300
	private void Awake()
	{
		base.enabled = false;
		foreach (LineRenderer lineRenderer in this.lines)
		{
			lineRenderer.positionCount = 2;
			lineRenderer.enabled = false;
		}
		if (this.targetFx != null)
		{
			this.targetFx.SetActive(false);
		}
	}

	// Token: 0x06000B8C RID: 2956 RVA: 0x0003E154 File Offset: 0x0003C354
	public void EnableLazer(Transform[] eyes_, VRRig rig_, float maxDist = 10000f)
	{
		if (rig_ == this.targetRig)
		{
			return;
		}
		this.eyeBones = eyes_;
		this.targetRig = rig_;
		this.targetPos = this.targetRig.transform.position;
		base.enabled = true;
		LineRenderer[] array = this.lines;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		if (this.targetFx != null)
		{
			this.targetFx.transform.position = this.targetPos;
			this.targetFx.SetActive(true);
		}
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x0003E1EC File Offset: 0x0003C3EC
	public void EnableLazer(Transform[] eyes_, Vector3 targetPos_)
	{
		this.eyeBones = eyes_;
		this.targetRig = null;
		this.targetPos = targetPos_;
		base.enabled = true;
		LineRenderer[] array = this.lines;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		if (this.targetFx != null)
		{
			this.targetFx.transform.position = this.targetPos;
			this.targetFx.SetActive(true);
		}
	}

	// Token: 0x06000B8E RID: 2958 RVA: 0x0003E264 File Offset: 0x0003C464
	public void DisableLazer()
	{
		this.targetRig = null;
		if (base.enabled)
		{
			base.enabled = false;
			LineRenderer[] array = this.lines;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			if (this.targetFx != null)
			{
				this.targetFx.SetActive(false);
			}
		}
	}

	// Token: 0x06000B8F RID: 2959 RVA: 0x0003E2C0 File Offset: 0x0003C4C0
	private void Update()
	{
		if (this.targetRig != null)
		{
			this.targetPos = this.targetRig.transform.position;
		}
		for (int i = 0; i < this.lines.Length; i++)
		{
			this.lines[i].SetPosition(0, this.eyeBones[i].transform.position);
			this.lines[i].SetPosition(1, this.targetPos);
		}
		if (this.targetFx != null)
		{
			this.targetFx.transform.position = this.targetPos;
		}
	}

	// Token: 0x04000DF2 RID: 3570
	private Transform[] eyeBones;

	// Token: 0x04000DF3 RID: 3571
	private VRRig targetRig;

	// Token: 0x04000DF4 RID: 3572
	private Vector3 targetPos;

	// Token: 0x04000DF5 RID: 3573
	public LineRenderer[] lines;

	// Token: 0x04000DF6 RID: 3574
	public GameObject targetFx;
}
