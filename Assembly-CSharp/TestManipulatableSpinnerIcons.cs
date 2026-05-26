using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200052E RID: 1326
public class TestManipulatableSpinnerIcons : MonoBehaviour
{
	// Token: 0x06002166 RID: 8550 RVA: 0x000B2156 File Offset: 0x000B0356
	private void Awake()
	{
		this.GenerateRollers();
	}

	// Token: 0x06002167 RID: 8551 RVA: 0x000B215E File Offset: 0x000B035E
	private void LateUpdate()
	{
		this.currentRotation = this.spinner.angle * this.rotationScale;
		this.UpdateSelectedIndex();
		this.UpdateRollers();
	}

	// Token: 0x06002168 RID: 8552 RVA: 0x000B2184 File Offset: 0x000B0384
	private void GenerateRollers()
	{
		for (int i = 0; i < this.rollerElementCount; i++)
		{
			float x = this.rollerElementAngle * (float)i + this.rollerElementAngle * 0.5f;
			Object.Instantiate<GameObject>(this.rollerElementTemplate, base.transform).transform.localRotation = Quaternion.Euler(x, 0f, 0f);
			GameObject gameObject = Object.Instantiate<GameObject>(this.iconElementTemplate, this.iconCanvas.transform);
			gameObject.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
			this.visibleIcons.Add(gameObject.GetComponentInChildren<Text>());
		}
		this.rollerElementTemplate.SetActive(false);
		this.iconElementTemplate.SetActive(false);
		this.UpdateRollers();
	}

	// Token: 0x06002169 RID: 8553 RVA: 0x000B224C File Offset: 0x000B044C
	private void UpdateSelectedIndex()
	{
		float num = this.currentRotation / this.rollerElementAngle;
		if (this.rollerElementCount % 2 == 1)
		{
			num += 0.5f;
		}
		this.selectedIndex = Mathf.FloorToInt(num);
		this.selectedIndex %= this.scrollableCount;
		if (this.selectedIndex < 0)
		{
			this.selectedIndex = this.scrollableCount + this.selectedIndex;
		}
	}

	// Token: 0x0600216A RID: 8554 RVA: 0x000B22B8 File Offset: 0x000B04B8
	private void UpdateRollers()
	{
		float num = this.currentRotation;
		if (Mathf.Abs(num) > this.rollerElementAngle / 2f)
		{
			if (num > 0f)
			{
				num += this.rollerElementAngle / 2f;
				num %= this.rollerElementAngle;
				num -= this.rollerElementAngle / 2f;
			}
			else
			{
				num -= this.rollerElementAngle / 2f;
				num %= this.rollerElementAngle;
				num += this.rollerElementAngle / 2f;
			}
		}
		num -= (float)this.rollerElementCount / 2f * this.rollerElementAngle;
		base.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
		this.iconCanvas.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
		int num2 = this.rollerElementCount / 2;
		for (int i = 0; i < this.visibleIcons.Count; i++)
		{
			int num3 = this.selectedIndex - i + num2;
			if (num3 < 0)
			{
				num3 += this.scrollableCount;
			}
			else
			{
				num3 %= this.scrollableCount;
			}
			this.visibleIcons[i].text = string.Format("{0}", num3 + 1);
		}
	}

	// Token: 0x04002C1A RID: 11290
	public ManipulatableSpinner spinner;

	// Token: 0x04002C1B RID: 11291
	public float rotationScale = 1f;

	// Token: 0x04002C1C RID: 11292
	public int rollerElementCount = 5;

	// Token: 0x04002C1D RID: 11293
	public GameObject rollerElementTemplate;

	// Token: 0x04002C1E RID: 11294
	public GameObject iconCanvas;

	// Token: 0x04002C1F RID: 11295
	public GameObject iconElementTemplate;

	// Token: 0x04002C20 RID: 11296
	public float iconOffset = 1f;

	// Token: 0x04002C21 RID: 11297
	public float rollerElementAngle = 15f;

	// Token: 0x04002C22 RID: 11298
	private List<Text> visibleIcons = new List<Text>();

	// Token: 0x04002C23 RID: 11299
	private float currentRotation;

	// Token: 0x04002C24 RID: 11300
	public int scrollableCount = 50;

	// Token: 0x04002C25 RID: 11301
	public int selectedIndex;
}
