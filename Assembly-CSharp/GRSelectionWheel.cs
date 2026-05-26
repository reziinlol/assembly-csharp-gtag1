using System;
using System.Collections.Generic;
using GorillaExtensions;
using TMPro;
using UnityEngine;

// Token: 0x020007D6 RID: 2006
public class GRSelectionWheel : MonoBehaviour, ITickSystemTick
{
	// Token: 0x1700049F RID: 1183
	// (get) Token: 0x06003330 RID: 13104 RVA: 0x00119C12 File Offset: 0x00117E12
	// (set) Token: 0x06003331 RID: 13105 RVA: 0x00119C1A File Offset: 0x00117E1A
	public bool TickRunning { get; set; }

	// Token: 0x06003332 RID: 13106 RVA: 0x00119C23 File Offset: 0x00117E23
	public void Start()
	{
		this.targetPage = 0;
	}

	// Token: 0x06003333 RID: 13107 RVA: 0x00019E3F File Offset: 0x0001803F
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06003334 RID: 13108 RVA: 0x00019E47 File Offset: 0x00018047
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06003335 RID: 13109 RVA: 0x00119C2C File Offset: 0x00117E2C
	public void ShowText(bool showText)
	{
		foreach (TMP_Text tmp_Text in this.shelfNames)
		{
			tmp_Text.enabled = showText;
		}
	}

	// Token: 0x06003336 RID: 13110 RVA: 0x00119C80 File Offset: 0x00117E80
	public void InitFromNameList(List<string> shelves)
	{
		this.shelfNames.Clear();
		for (int i = 0; i < shelves.Count; i++)
		{
			TMP_Text tmp_Text = Object.Instantiate<TMP_Text>(this.templateText);
			tmp_Text.text = shelves[i];
			this.shelfNames.Add(tmp_Text);
			tmp_Text.transform.SetParent(base.transform, false);
		}
		this.UpdateVisuals();
	}

	// Token: 0x06003337 RID: 13111 RVA: 0x00119CE8 File Offset: 0x00117EE8
	public void Tick()
	{
		if (!this.isBeingDrivenRemotely)
		{
			float num = this.deltaAngle * (float)this.shelfNames.Count;
			float num2 = this.currentAngle / this.deltaAngle;
			int num3 = (int)(num2 + 0.5f);
			if (this.rotSpeedMult == 0f)
			{
				float num4 = ((float)num3 - num2) * this.deltaAngle;
				this.currentAngle += num4 * (1f - Mathf.Exp(-20f * Time.deltaTime));
				this.targetPage = num3;
			}
			else
			{
				this.currentAngle += this.rotSpeedMult * Time.deltaTime * this.rotSpeed;
				this.currentAngle = Mathf.Clamp(this.currentAngle, -this.deltaAngle * 0.4f, num - this.deltaAngle + this.deltaAngle * 0.4f);
			}
		}
		int num5 = (int)(this.currentAngle / this.deltaAngle + 0.5f);
		if (this.lastPlayedAudioTickPage != num5)
		{
			this.lastPlayedAudioTickPage = num5;
			this.audioSource.GTPlay();
		}
		float num6 = 0.005f;
		if (Math.Abs(this.lastAngle - this.currentAngle) > num6)
		{
			this.UpdateVisuals();
		}
		this.lastAngle = this.currentAngle;
	}

	// Token: 0x06003338 RID: 13112 RVA: 0x00119E27 File Offset: 0x00118027
	public void SetRotationSpeed(float speed)
	{
		this.rotSpeedMult = Mathf.Sign(speed) * Mathf.Pow(Mathf.Abs(speed), 2f);
	}

	// Token: 0x06003339 RID: 13113 RVA: 0x00119E46 File Offset: 0x00118046
	public void SetTargetShelf(int shelf)
	{
		this.currentAngle += (float)(shelf - this.targetPage) * this.deltaAngle;
		this.targetPage = shelf;
	}

	// Token: 0x0600333A RID: 13114 RVA: 0x00119E6C File Offset: 0x0011806C
	public void SetTargetAngle(float angle)
	{
		this.currentAngle = angle;
	}

	// Token: 0x0600333B RID: 13115 RVA: 0x00119E78 File Offset: 0x00118078
	public void UpdateVisuals()
	{
		this.rotationWheel.localRotation = Quaternion.Euler(-this.currentAngle + 7.5f, 0f, 0f);
		float num = this.deltaAngle;
		int count = this.shelfNames.Count;
		float num2 = this.currentAngle / this.deltaAngle;
		for (int i = 0; i < this.shelfNames.Count; i++)
		{
			float num3 = ((float)i - num2) * this.deltaAngle + this.pointerOffsetAngle;
			float f = num3 * 3.1415927f / 180f;
			float num4 = Mathf.Cos(f);
			float num5 = Mathf.Sin(f);
			Quaternion localRotation = Quaternion.Euler(90f - num3, 180f, 0f);
			Vector3 position = new Vector3(this.textHorizOffset, num4 * this.wheelTextRadius, num5 * this.wheelTextRadius);
			this.shelfNames[i].transform.rotation = base.transform.TransformRotation(localRotation);
			this.shelfNames[i].transform.position = base.transform.TransformPoint(position);
			this.shelfNames[i].color = ((Math.Abs(num2 - (float)i) < 0.5f) ? Color.green : Color.white);
		}
	}

	// Token: 0x040042C9 RID: 17097
	private List<TMP_Text> shelfNames = new List<TMP_Text>();

	// Token: 0x040042CA RID: 17098
	public TMP_Text templateText;

	// Token: 0x040042CB RID: 17099
	public float deltaAngle;

	// Token: 0x040042CC RID: 17100
	public float pointerOffsetAngle;

	// Token: 0x040042CD RID: 17101
	public float wheelTextRadius;

	// Token: 0x040042CE RID: 17102
	public float textHorizOffset = -0.0375f;

	// Token: 0x040042CF RID: 17103
	public float rotSpeed = 60f;

	// Token: 0x040042D0 RID: 17104
	public bool isBeingDrivenRemotely;

	// Token: 0x040042D1 RID: 17105
	public AudioSource audioSource;

	// Token: 0x040042D2 RID: 17106
	public int lastPlayedAudioTickPage = -1;

	// Token: 0x040042D3 RID: 17107
	public float wheelTextPairOffset = 0.0025f;

	// Token: 0x040042D4 RID: 17108
	public Transform rotationWheel;

	// Token: 0x040042D5 RID: 17109
	public float lastAngle = -1000f;

	// Token: 0x040042D7 RID: 17111
	[NonSerialized]
	public int targetPage;

	// Token: 0x040042D8 RID: 17112
	[NonSerialized]
	public float currentAngle;

	// Token: 0x040042D9 RID: 17113
	private float rotSpeedMult;
}
