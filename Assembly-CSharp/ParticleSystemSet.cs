using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200048A RID: 1162
public class ParticleSystemSet : MonoBehaviour
{
	// Token: 0x06001C3A RID: 7226 RVA: 0x00098CE4 File Offset: 0x00096EE4
	private void Awake()
	{
		this.localScale = base.transform.localScale;
		this.ps = base.GetComponentsInChildren<ParticleSystem>();
		List<ParticleSystem.MainModule> list = new List<ParticleSystem.MainModule>();
		List<ParticleSystem.EmissionModule> list2 = new List<ParticleSystem.EmissionModule>();
		this.skipSet = new HashSet<ParticleSystem.MainModule>();
		for (int i = 0; i < this.ps.Length; i++)
		{
			list.Add(this.ps[i].main);
			list2.Add(this.ps[i].emission);
		}
		for (int j = 0; j < this.skipForSimulationSpeed.Length; j++)
		{
			this.skipSet.Add(this.skipForSimulationSpeed[j].GetComponent<ParticleSystem>().main);
		}
		this.psMains = list.ToArray();
		this.psEmits = list2.ToArray();
		this.SetPlayBackSpeed(0f);
	}

	// Token: 0x06001C3B RID: 7227 RVA: 0x00098DB2 File Offset: 0x00096FB2
	public void SetFadeRate(float rate)
	{
		if (rate > 0f)
		{
			this.fadeRate = rate;
		}
	}

	// Token: 0x06001C3C RID: 7228 RVA: 0x00098DC4 File Offset: 0x00096FC4
	public void SetPlayBackSpeed(float target)
	{
		for (int i = 0; i < this.psMains.Length; i++)
		{
			if (!this.skipSet.Contains(this.psMains[i]))
			{
				this.psMains[i].simulationSpeed = target;
			}
		}
	}

	// Token: 0x06001C3D RID: 7229 RVA: 0x00098E10 File Offset: 0x00097010
	public void FadePlayBackSpeed(float target)
	{
		ParticleSystemSet.<FadePlayBackSpeed>d__12 <FadePlayBackSpeed>d__;
		<FadePlayBackSpeed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<FadePlayBackSpeed>d__.<>4__this = this;
		<FadePlayBackSpeed>d__.target = target;
		<FadePlayBackSpeed>d__.<>1__state = -1;
		<FadePlayBackSpeed>d__.<>t__builder.Start<ParticleSystemSet.<FadePlayBackSpeed>d__12>(ref <FadePlayBackSpeed>d__);
	}

	// Token: 0x06001C3E RID: 7230 RVA: 0x00098E50 File Offset: 0x00097050
	public void SetColor(string RRGGBB)
	{
		Color color = new Color((float)int.Parse(RRGGBB.Substring(0, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBB.Substring(2, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBB.Substring(4, 2), NumberStyles.HexNumber) / 255f);
		float h;
		float s;
		float num;
		Color.RGBToHSV(color, out h, out s, out num);
		Color max = Color.HSVToRGB(h, s, num / 4f);
		for (int i = 0; i < this.psMains.Length; i++)
		{
			this.psMains[i].startColor = new ParticleSystem.MinMaxGradient(color, max);
		}
	}

	// Token: 0x06001C3F RID: 7231 RVA: 0x00098EFC File Offset: 0x000970FC
	public void SetColors(string RRGGBBRRGGBB)
	{
		Color min = new Color((float)int.Parse(RRGGBBRRGGBB.Substring(0, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBBRRGGBB.Substring(2, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBBRRGGBB.Substring(4, 2), NumberStyles.HexNumber) / 255f);
		Color max = new Color((float)int.Parse(RRGGBBRRGGBB.Substring(6, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBBRRGGBB.Substring(8, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBBRRGGBB.Substring(10, 2), NumberStyles.HexNumber) / 255f);
		for (int i = 0; i < this.psMains.Length; i++)
		{
			this.psMains[i].startColor = new ParticleSystem.MinMaxGradient(min, max);
		}
	}

	// Token: 0x06001C40 RID: 7232 RVA: 0x00098FDC File Offset: 0x000971DC
	public void Pause()
	{
		for (int i = 0; i < this.ps.Length; i++)
		{
			this.ps[i].Pause();
		}
	}

	// Token: 0x06001C41 RID: 7233 RVA: 0x0009900C File Offset: 0x0009720C
	public void StartEmission()
	{
		for (int i = 0; i < this.ps.Length; i++)
		{
			this.psMains[i].prewarm = false;
			this.ps[i].Play();
		}
		for (int j = 0; j < this.ActiveDuringEmission.Length; j++)
		{
			this.ActiveDuringEmission[j].SetActive(true);
		}
	}

	// Token: 0x06001C42 RID: 7234 RVA: 0x0009906C File Offset: 0x0009726C
	public void StopEmission()
	{
		for (int i = 0; i < this.ps.Length; i++)
		{
			this.ps[i].Stop();
		}
		for (int j = 0; j < this.ActiveDuringEmission.Length; j++)
		{
			this.ActiveDuringEmission[j].SetActive(false);
		}
	}

	// Token: 0x06001C43 RID: 7235 RVA: 0x000990BC File Offset: 0x000972BC
	public void Clear()
	{
		for (int i = 0; i < this.ps.Length; i++)
		{
			this.ps[i].Clear();
		}
	}

	// Token: 0x06001C44 RID: 7236 RVA: 0x000990E9 File Offset: 0x000972E9
	public void SetScaleXZ(float scaler)
	{
		base.transform.localScale = new Vector3(this.localScale.x * scaler, this.localScale.y, this.localScale.z * scaler);
	}

	// Token: 0x06001C45 RID: 7237 RVA: 0x00099120 File Offset: 0x00097320
	public void FadeScaleXZ(float scaler)
	{
		ParticleSystemSet.<FadeScaleXZ>d__20 <FadeScaleXZ>d__;
		<FadeScaleXZ>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<FadeScaleXZ>d__.<>4__this = this;
		<FadeScaleXZ>d__.scaler = scaler;
		<FadeScaleXZ>d__.<>1__state = -1;
		<FadeScaleXZ>d__.<>t__builder.Start<ParticleSystemSet.<FadeScaleXZ>d__20>(ref <FadeScaleXZ>d__);
	}

	// Token: 0x0400264D RID: 9805
	[SerializeField]
	private GameObject[] ActiveDuringEmission;

	// Token: 0x0400264E RID: 9806
	private Vector3 localScale = Vector3.one;

	// Token: 0x0400264F RID: 9807
	private ParticleSystem[] ps;

	// Token: 0x04002650 RID: 9808
	private ParticleSystem.MainModule[] psMains;

	// Token: 0x04002651 RID: 9809
	public GameObject[] skipForSimulationSpeed;

	// Token: 0x04002652 RID: 9810
	private HashSet<ParticleSystem.MainModule> skipSet;

	// Token: 0x04002653 RID: 9811
	private ParticleSystem.EmissionModule[] psEmits;

	// Token: 0x04002654 RID: 9812
	private bool loop;

	// Token: 0x04002655 RID: 9813
	private float fadeRate = 1f;
}
