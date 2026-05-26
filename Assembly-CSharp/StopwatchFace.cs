using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020009D8 RID: 2520
public class StopwatchFace : MonoBehaviour
{
	// Token: 0x170005F9 RID: 1529
	// (get) Token: 0x0600407C RID: 16508 RVA: 0x00158685 File Offset: 0x00156885
	public bool watchActive
	{
		get
		{
			return this._watchActive;
		}
	}

	// Token: 0x170005FA RID: 1530
	// (get) Token: 0x0600407D RID: 16509 RVA: 0x0015868D File Offset: 0x0015688D
	public int millisElapsed
	{
		get
		{
			return this._millisElapsed;
		}
	}

	// Token: 0x170005FB RID: 1531
	// (get) Token: 0x0600407E RID: 16510 RVA: 0x00158695 File Offset: 0x00156895
	public Vector3Int digitsMmSsMs
	{
		get
		{
			return StopwatchFace.ParseDigits(TimeSpan.FromMilliseconds((double)this._millisElapsed));
		}
	}

	// Token: 0x0600407F RID: 16511 RVA: 0x001586A8 File Offset: 0x001568A8
	public void SetMillisElapsed(int millis, bool updateFace = true)
	{
		this._millisElapsed = millis;
		if (!updateFace)
		{
			return;
		}
		this.UpdateText();
		this.UpdateHand();
	}

	// Token: 0x06004080 RID: 16512 RVA: 0x001586C1 File Offset: 0x001568C1
	private void Awake()
	{
		this._lerpToZero = new LerpTask<int>();
		this._lerpToZero.onLerp = new Action<int, int, float>(this.OnLerpToZero);
		this._lerpToZero.onLerpEnd = new Action(this.OnLerpEnd);
	}

	// Token: 0x06004081 RID: 16513 RVA: 0x001586FC File Offset: 0x001568FC
	private void OnLerpToZero(int a, int b, float t)
	{
		this._millisElapsed = Mathf.FloorToInt(Mathf.Lerp((float)a, (float)b, t * t));
		this.UpdateText();
		this.UpdateHand();
	}

	// Token: 0x06004082 RID: 16514 RVA: 0x00158721 File Offset: 0x00156921
	private void OnLerpEnd()
	{
		this.WatchReset(false);
	}

	// Token: 0x06004083 RID: 16515 RVA: 0x00158721 File Offset: 0x00156921
	private void OnEnable()
	{
		this.WatchReset(false);
	}

	// Token: 0x06004084 RID: 16516 RVA: 0x00158721 File Offset: 0x00156921
	private void OnDisable()
	{
		this.WatchReset(false);
	}

	// Token: 0x06004085 RID: 16517 RVA: 0x0015872C File Offset: 0x0015692C
	private void Update()
	{
		if (this._lerpToZero.active)
		{
			this._lerpToZero.Update();
			return;
		}
		if (this._watchActive)
		{
			this._millisElapsed += Mathf.FloorToInt(Time.deltaTime * 1000f);
			this.UpdateText();
			this.UpdateHand();
		}
	}

	// Token: 0x06004086 RID: 16518 RVA: 0x00158784 File Offset: 0x00156984
	private static Vector3Int ParseDigits(TimeSpan time)
	{
		int num = (int)time.TotalMinutes % 100;
		double num2 = 60.0 * (time.TotalMinutes - (double)num);
		int num3 = (int)num2;
		int num4 = (int)(100.0 * (num2 - (double)num3));
		num = Math.Clamp(num, 0, 99);
		num3 = Math.Clamp(num3, 0, 59);
		num4 = Math.Clamp(num4, 0, 99);
		return new Vector3Int(num, num3, num4);
	}

	// Token: 0x06004087 RID: 16519 RVA: 0x001587EC File Offset: 0x001569EC
	private void UpdateText()
	{
		Vector3Int vector3Int = StopwatchFace.ParseDigits(TimeSpan.FromMilliseconds((double)this._millisElapsed));
		string text = vector3Int.x.ToString("D2");
		string text2 = vector3Int.y.ToString("D2");
		string text3 = vector3Int.z.ToString("D2");
		this._text.text = string.Concat(new string[]
		{
			text,
			":",
			text2,
			":",
			text3
		});
	}

	// Token: 0x06004088 RID: 16520 RVA: 0x00158880 File Offset: 0x00156A80
	private void UpdateHand()
	{
		float z = (float)(this._millisElapsed % 60000) / 60000f * 360f;
		this._hand.localEulerAngles = new Vector3(0f, 0f, z);
	}

	// Token: 0x06004089 RID: 16521 RVA: 0x001588C2 File Offset: 0x00156AC2
	public void WatchToggle()
	{
		if (!this._watchActive)
		{
			this.WatchStart();
			return;
		}
		this.WatchStop();
	}

	// Token: 0x0600408A RID: 16522 RVA: 0x001588D9 File Offset: 0x00156AD9
	public void WatchStart()
	{
		if (this._lerpToZero.active)
		{
			return;
		}
		this._watchActive = true;
	}

	// Token: 0x0600408B RID: 16523 RVA: 0x001588F0 File Offset: 0x00156AF0
	public void WatchStop()
	{
		if (this._lerpToZero.active)
		{
			return;
		}
		this._watchActive = false;
	}

	// Token: 0x0600408C RID: 16524 RVA: 0x00158907 File Offset: 0x00156B07
	public void WatchReset()
	{
		this.WatchReset(true);
	}

	// Token: 0x0600408D RID: 16525 RVA: 0x00158910 File Offset: 0x00156B10
	public void WatchReset(bool doLerp)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (doLerp)
		{
			if (!this._lerpToZero.active)
			{
				this._lerpToZero.Start(this._millisElapsed % 60000, 0, 0.36f);
				return;
			}
		}
		else
		{
			this._watchActive = false;
			this._millisElapsed = 0;
			this.UpdateText();
			this.UpdateHand();
		}
	}

	// Token: 0x04005113 RID: 20755
	[SerializeField]
	private Transform _hand;

	// Token: 0x04005114 RID: 20756
	[SerializeField]
	private Text _text;

	// Token: 0x04005115 RID: 20757
	[Space]
	[SerializeField]
	private StopwatchCosmetic _cosmetic;

	// Token: 0x04005116 RID: 20758
	[Space]
	[SerializeField]
	private AudioClip _audioClick;

	// Token: 0x04005117 RID: 20759
	[SerializeField]
	private AudioClip _audioReset;

	// Token: 0x04005118 RID: 20760
	[SerializeField]
	private AudioClip _audioTick;

	// Token: 0x04005119 RID: 20761
	[Space]
	[NonSerialized]
	private int _millisElapsed;

	// Token: 0x0400511A RID: 20762
	[NonSerialized]
	private bool _watchActive;

	// Token: 0x0400511B RID: 20763
	[NonSerialized]
	private LerpTask<int> _lerpToZero;
}
