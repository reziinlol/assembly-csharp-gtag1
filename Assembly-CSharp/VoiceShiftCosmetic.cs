using System;
using UnityEngine;

// Token: 0x020002FD RID: 765
public class VoiceShiftCosmetic : MonoBehaviour
{
	// Token: 0x170001ED RID: 493
	// (get) Token: 0x0600137F RID: 4991 RVA: 0x00066C90 File Offset: 0x00064E90
	public bool ModifyPitch
	{
		get
		{
			return this.modifyPitch;
		}
	}

	// Token: 0x170001EE RID: 494
	// (get) Token: 0x06001380 RID: 4992 RVA: 0x00066C98 File Offset: 0x00064E98
	public bool ModifyVolume
	{
		get
		{
			return this.modifyVolume;
		}
	}

	// Token: 0x170001EF RID: 495
	// (get) Token: 0x06001381 RID: 4993 RVA: 0x00066CA0 File Offset: 0x00064EA0
	public bool IsShifted
	{
		get
		{
			return this.isShifted;
		}
	}

	// Token: 0x170001F0 RID: 496
	// (get) Token: 0x06001382 RID: 4994 RVA: 0x00066CA8 File Offset: 0x00064EA8
	// (set) Token: 0x06001383 RID: 4995 RVA: 0x00066CB0 File Offset: 0x00064EB0
	public float Pitch
	{
		get
		{
			return this.pitch;
		}
		set
		{
			if (!this.modifyPitch)
			{
				return;
			}
			float num = Mathf.Clamp(value, 0.6666667f, 1.5f);
			this.pitch = num;
			VRRig vrrig = this.myRig;
			if (vrrig == null)
			{
				return;
			}
			vrrig.SetVoiceShiftCosmeticsDirty();
		}
	}

	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x06001384 RID: 4996 RVA: 0x00066CEE File Offset: 0x00064EEE
	// (set) Token: 0x06001385 RID: 4997 RVA: 0x00066CF8 File Offset: 0x00064EF8
	public float Volume
	{
		get
		{
			return this.volume;
		}
		set
		{
			if (!this.modifyVolume)
			{
				return;
			}
			float num = Mathf.Clamp(value, 0f, 1f);
			this.volume = num;
			VRRig vrrig = this.myRig;
			if (vrrig == null)
			{
				return;
			}
			vrrig.SetVoiceShiftCosmeticsDirty();
		}
	}

	// Token: 0x06001386 RID: 4998 RVA: 0x00066D38 File Offset: 0x00064F38
	private void OnEnable()
	{
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
		if (this.myRig == null)
		{
			return;
		}
		this.myRig.VoiceShiftCosmetics.Add(this);
		this.myRig.SetVoiceShiftCosmeticsDirty();
	}

	// Token: 0x06001387 RID: 4999 RVA: 0x00066D84 File Offset: 0x00064F84
	private void OnDisable()
	{
		if (this.myRig == null)
		{
			return;
		}
		this.myRig.VoiceShiftCosmetics.Remove(this);
		this.myRig.SetVoiceShiftCosmeticsDirty();
	}

	// Token: 0x06001388 RID: 5000 RVA: 0x00066DB2 File Offset: 0x00064FB2
	public void StartVoiceShift()
	{
		if (this.isShifted)
		{
			return;
		}
		this.isShifted = true;
		if (this.modifyPitch)
		{
			this.Pitch = this.shiftedPitch;
		}
		if (this.modifyVolume)
		{
			this.Volume = this.shiftedVolume;
		}
	}

	// Token: 0x06001389 RID: 5001 RVA: 0x00066DEC File Offset: 0x00064FEC
	public void StopVoiceShift()
	{
		if (!this.isShifted)
		{
			return;
		}
		this.isShifted = false;
		VRRig vrrig = this.myRig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.SetVoiceShiftCosmeticsDirty();
	}

	// Token: 0x0600138A RID: 5002 RVA: 0x00066E0E File Offset: 0x0006500E
	public void ToggleVoiceShift()
	{
		if (this.isShifted)
		{
			this.StopVoiceShift();
			return;
		}
		this.StartVoiceShift();
	}

	// Token: 0x040017EB RID: 6123
	private const float PITCH_MIN = 0.6666667f;

	// Token: 0x040017EC RID: 6124
	private const float PITCH_MAX = 1.5f;

	// Token: 0x040017ED RID: 6125
	private const float VOLUME_MIN = 0f;

	// Token: 0x040017EE RID: 6126
	private const float VOLUME_MAX = 1f;

	// Token: 0x040017EF RID: 6127
	[SerializeField]
	private bool modifyPitch = true;

	// Token: 0x040017F0 RID: 6128
	[SerializeField]
	private bool modifyVolume = true;

	// Token: 0x040017F1 RID: 6129
	[Range(0.6666667f, 1.5f)]
	[SerializeField]
	private float shiftedPitch = 1.5f;

	// Token: 0x040017F2 RID: 6130
	[Range(0f, 1f)]
	[SerializeField]
	private float shiftedVolume = 1f;

	// Token: 0x040017F3 RID: 6131
	private float pitch = 1f;

	// Token: 0x040017F4 RID: 6132
	private float volume = 1f;

	// Token: 0x040017F5 RID: 6133
	private bool isShifted;

	// Token: 0x040017F6 RID: 6134
	private VRRig myRig;
}
