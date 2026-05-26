using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x0200040C RID: 1036
public class LightArray : MonoBehaviour
{
	// Token: 0x060018B6 RID: 6326 RVA: 0x0008BBEA File Offset: 0x00089DEA
	private void ToggleDynamicLighting()
	{
		GameLightingManager.instance.ToggleCustomDynamicLightingEnabled();
	}

	// Token: 0x060018B7 RID: 6327 RVA: 0x0008BBF8 File Offset: 0x00089DF8
	public void SetCascadeTime(int ct)
	{
		this.cascadeTime = ct;
	}

	// Token: 0x060018B8 RID: 6328 RVA: 0x0008BC04 File Offset: 0x00089E04
	public void SetSubArraysCascadeTime(int ct)
	{
		for (int i = 0; i < this.subArrays.Length; i++)
		{
			this.subArrays[i].cascadeTime = ct;
		}
	}

	// Token: 0x060018B9 RID: 6329 RVA: 0x0008BC34 File Offset: 0x00089E34
	public void SetPreset(int i)
	{
		if (this.presets == null)
		{
			return;
		}
		LightArrayPresets.LightArrayPreset preset = this.presets.GetPreset(i);
		if (preset == null)
		{
			return;
		}
		this.SetColorAndIntensity(preset.color, preset.intensity);
	}

	// Token: 0x060018BA RID: 6330 RVA: 0x0008BC74 File Offset: 0x00089E74
	public void SetPreset(string n)
	{
		if (this.presets == null)
		{
			return;
		}
		LightArrayPresets.LightArrayPreset preset = this.presets.GetPreset(n);
		if (preset == null)
		{
			return;
		}
		this.SetColorAndIntensity(preset.color, preset.intensity);
	}

	// Token: 0x060018BB RID: 6331 RVA: 0x0008BCB3 File Offset: 0x00089EB3
	public void SetColorAndIntensity(string RRGGBBF)
	{
		this.SetColorAndIntensity(this.GetColor(RRGGBBF), float.Parse(RRGGBBF.Substring(6).ToString()));
	}

	// Token: 0x060018BC RID: 6332 RVA: 0x0008BCD4 File Offset: 0x00089ED4
	private void SetColorAndIntensity(Color c, float intensity)
	{
		LightArray.<SetColorAndIntensity>d__10 <SetColorAndIntensity>d__;
		<SetColorAndIntensity>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetColorAndIntensity>d__.<>4__this = this;
		<SetColorAndIntensity>d__.c = c;
		<SetColorAndIntensity>d__.intensity = intensity;
		<SetColorAndIntensity>d__.<>1__state = -1;
		<SetColorAndIntensity>d__.<>t__builder.Start<LightArray.<SetColorAndIntensity>d__10>(ref <SetColorAndIntensity>d__);
	}

	// Token: 0x060018BD RID: 6333 RVA: 0x0008BD1B File Offset: 0x00089F1B
	public void SetColor(string RRGGBB)
	{
		this.SetColor(this.GetColor(RRGGBB));
	}

	// Token: 0x060018BE RID: 6334 RVA: 0x0008BD2C File Offset: 0x00089F2C
	private void SetColor(Color c)
	{
		LightArray.<SetColor>d__12 <SetColor>d__;
		<SetColor>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetColor>d__.<>4__this = this;
		<SetColor>d__.c = c;
		<SetColor>d__.<>1__state = -1;
		<SetColor>d__.<>t__builder.Start<LightArray.<SetColor>d__12>(ref <SetColor>d__);
	}

	// Token: 0x060018BF RID: 6335 RVA: 0x0008BD6C File Offset: 0x00089F6C
	public void SetIntensity(float intensity)
	{
		LightArray.<SetIntensity>d__13 <SetIntensity>d__;
		<SetIntensity>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetIntensity>d__.<>4__this = this;
		<SetIntensity>d__.intensity = intensity;
		<SetIntensity>d__.<>1__state = -1;
		<SetIntensity>d__.<>t__builder.Start<LightArray.<SetIntensity>d__13>(ref <SetIntensity>d__);
	}

	// Token: 0x060018C0 RID: 6336 RVA: 0x0008BDAC File Offset: 0x00089FAC
	private Task SetLightColorAndIntensity(Color c, float intensity, int i)
	{
		LightArray.<SetLightColorAndIntensity>d__14 <SetLightColorAndIntensity>d__;
		<SetLightColorAndIntensity>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SetLightColorAndIntensity>d__.<>4__this = this;
		<SetLightColorAndIntensity>d__.c = c;
		<SetLightColorAndIntensity>d__.intensity = intensity;
		<SetLightColorAndIntensity>d__.i = i;
		<SetLightColorAndIntensity>d__.<>1__state = -1;
		<SetLightColorAndIntensity>d__.<>t__builder.Start<LightArray.<SetLightColorAndIntensity>d__14>(ref <SetLightColorAndIntensity>d__);
		return <SetLightColorAndIntensity>d__.<>t__builder.Task;
	}

	// Token: 0x060018C1 RID: 6337 RVA: 0x0008BE08 File Offset: 0x0008A008
	private Task SetLightColor(Color c, int i)
	{
		LightArray.<SetLightColor>d__15 <SetLightColor>d__;
		<SetLightColor>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SetLightColor>d__.<>4__this = this;
		<SetLightColor>d__.c = c;
		<SetLightColor>d__.i = i;
		<SetLightColor>d__.<>1__state = -1;
		<SetLightColor>d__.<>t__builder.Start<LightArray.<SetLightColor>d__15>(ref <SetLightColor>d__);
		return <SetLightColor>d__.<>t__builder.Task;
	}

	// Token: 0x060018C2 RID: 6338 RVA: 0x0008BE5C File Offset: 0x0008A05C
	private Task SetLightIntensity(float intensity, int i)
	{
		LightArray.<SetLightIntensity>d__16 <SetLightIntensity>d__;
		<SetLightIntensity>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SetLightIntensity>d__.<>4__this = this;
		<SetLightIntensity>d__.intensity = intensity;
		<SetLightIntensity>d__.i = i;
		<SetLightIntensity>d__.<>1__state = -1;
		<SetLightIntensity>d__.<>t__builder.Start<LightArray.<SetLightIntensity>d__16>(ref <SetLightIntensity>d__);
		return <SetLightIntensity>d__.<>t__builder.Task;
	}

	// Token: 0x060018C3 RID: 6339 RVA: 0x0008BEB0 File Offset: 0x0008A0B0
	private Color GetColor(string RRGGBB)
	{
		return new Color((float)int.Parse(RRGGBB.Substring(0, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBB.Substring(2, 2), NumberStyles.HexNumber) / 255f, (float)int.Parse(RRGGBB.Substring(4, 2), NumberStyles.HexNumber) / 255f);
	}

	// Token: 0x060018C4 RID: 6340 RVA: 0x0008BF10 File Offset: 0x0008A110
	private void LateUpdate()
	{
		bool flag = false;
		bool flag2 = false;
		if (this.preLightHue != this.setLightHue)
		{
			flag = true;
			this.preLightHue = this.setLightHue;
		}
		if (this.preLightSat != this.setLightSat)
		{
			flag = true;
			this.preLightSat = this.setLightSat;
		}
		if (this.preLightVal != this.setLightVal)
		{
			flag = true;
			this.preLightVal = this.setLightVal;
		}
		if (this.preLightIntensity != this.setLightIntensity)
		{
			flag2 = true;
			this.preLightIntensity = this.setLightIntensity;
		}
		if (flag && flag2)
		{
			this.SetColorAndIntensity(Color.HSVToRGB(this.setLightHue, this.setLightSat, this.setLightVal), this.setLightIntensity);
			return;
		}
		if (flag)
		{
			this.SetColor(Color.HSVToRGB(this.setLightHue, this.setLightSat, this.setLightVal));
			return;
		}
		if (flag2)
		{
			this.SetIntensity(this.setLightIntensity);
		}
	}

	// Token: 0x040023C9 RID: 9161
	[SerializeField]
	private LightArrayPresets presets;

	// Token: 0x040023CA RID: 9162
	[SerializeField]
	private GameLight[] lights;

	// Token: 0x040023CB RID: 9163
	[SerializeField]
	private LightArray[] subArrays;

	// Token: 0x040023CC RID: 9164
	[SerializeField]
	private int cascadeTime;

	// Token: 0x040023CD RID: 9165
	[SerializeField]
	private float setLightHue = -1f;

	// Token: 0x040023CE RID: 9166
	[NonSerialized]
	private float preLightHue = -1f;

	// Token: 0x040023CF RID: 9167
	[SerializeField]
	private float setLightSat = -1f;

	// Token: 0x040023D0 RID: 9168
	[NonSerialized]
	private float preLightSat = -1f;

	// Token: 0x040023D1 RID: 9169
	[SerializeField]
	private float setLightVal = -1f;

	// Token: 0x040023D2 RID: 9170
	[NonSerialized]
	private float preLightVal = -1f;

	// Token: 0x040023D3 RID: 9171
	[SerializeField]
	private float setLightIntensity = -1f;

	// Token: 0x040023D4 RID: 9172
	[NonSerialized]
	private float preLightIntensity = -1f;
}
