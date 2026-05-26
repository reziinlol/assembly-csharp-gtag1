using System;
using UnityEngine;

// Token: 0x0200065D RID: 1629
public class Campfire : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600288E RID: 10382 RVA: 0x000DC540 File Offset: 0x000DA740
	private void Start()
	{
		this.lastAngleBottom = 0f;
		this.lastAngleMiddle = 0f;
		this.lastAngleTop = 0f;
		this.perlinBottom = (float)Random.Range(0, 100);
		this.perlinMiddle = (float)Random.Range(200, 300);
		this.perlinTop = (float)Random.Range(400, 500);
		this.startingRotationBottom = this.baseFire.localEulerAngles.x;
		this.startingRotationMiddle = this.middleFire.localEulerAngles.x;
		this.startingRotationTop = this.topFire.localEulerAngles.x;
		this.tempVec = new Vector3(0f, 0f, 0f);
		this.mergedBottom = false;
		this.mergedMiddle = false;
		this.mergedTop = false;
		this.wasActive = false;
		this.lastTime = Time.time;
	}

	// Token: 0x0600288F RID: 10383 RVA: 0x00011DD7 File Offset: 0x0000FFD7
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002890 RID: 10384 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002891 RID: 10385 RVA: 0x000DC62C File Offset: 0x000DA82C
	public void SliceUpdate()
	{
		if (BetterDayNightManager.instance == null)
		{
			return;
		}
		if ((this.isActive[BetterDayNightManager.instance.currentTimeIndex] && (this.playDuringRain || BetterDayNightManager.instance.CurrentWeather() != BetterDayNightManager.WeatherType.Raining)) || this.overrideDayNight == 1)
		{
			if (!this.wasActive)
			{
				this.wasActive = true;
				this.mergedBottom = false;
				this.mergedMiddle = false;
				this.mergedTop = false;
				Color.RGBToHSV(this.mat.color, out this.h, out this.s, out this.v);
				this.mat.color = Color.HSVToRGB(this.h, this.s, 1f);
			}
			this.Flap(ref this.perlinBottom, this.perlinStepBottom, ref this.lastAngleBottom, ref this.baseFire, this.bottomRange, this.baseMultiplier, ref this.mergedBottom);
			this.Flap(ref this.perlinMiddle, this.perlinStepMiddle, ref this.lastAngleMiddle, ref this.middleFire, this.middleRange, this.middleMultiplier, ref this.mergedMiddle);
			this.Flap(ref this.perlinTop, this.perlinStepTop, ref this.lastAngleTop, ref this.topFire, this.topRange, this.topMultiplier, ref this.mergedTop);
		}
		else
		{
			if (this.wasActive)
			{
				this.wasActive = false;
				this.mergedBottom = false;
				this.mergedMiddle = false;
				this.mergedTop = false;
				Color.RGBToHSV(this.mat.color, out this.h, out this.s, out this.v);
				this.mat.color = Color.HSVToRGB(this.h, this.s, 0.25f);
			}
			this.ReturnToOff(ref this.baseFire, this.startingRotationBottom, ref this.mergedBottom);
			this.ReturnToOff(ref this.middleFire, this.startingRotationMiddle, ref this.mergedMiddle);
			this.ReturnToOff(ref this.topFire, this.startingRotationTop, ref this.mergedTop);
		}
		this.lastTime = Time.time;
	}

	// Token: 0x06002892 RID: 10386 RVA: 0x000DC838 File Offset: 0x000DAA38
	private void Flap(ref float perlinValue, float perlinStep, ref float lastAngle, ref Transform flameTransform, float range, float multiplier, ref bool isMerged)
	{
		perlinValue += perlinStep;
		lastAngle += (Time.time - this.lastTime) * Mathf.PerlinNoise(perlinValue, 0f);
		this.tempVec.x = range * Mathf.Sin(lastAngle * multiplier);
		if (Mathf.Abs(this.tempVec.x - flameTransform.localEulerAngles.x) > 180f)
		{
			if (this.tempVec.x > flameTransform.localEulerAngles.x)
			{
				this.tempVec.x = this.tempVec.x - 360f;
			}
			else
			{
				this.tempVec.x = this.tempVec.x + 360f;
			}
		}
		if (isMerged)
		{
			flameTransform.localEulerAngles = this.tempVec;
			return;
		}
		if (Mathf.Abs(flameTransform.localEulerAngles.x - this.tempVec.x) < 1f)
		{
			isMerged = true;
			flameTransform.localEulerAngles = this.tempVec;
			return;
		}
		this.tempVec.x = (this.tempVec.x - flameTransform.localEulerAngles.x) * this.slerp + flameTransform.localEulerAngles.x;
		flameTransform.localEulerAngles = this.tempVec;
	}

	// Token: 0x06002893 RID: 10387 RVA: 0x000DC980 File Offset: 0x000DAB80
	private void ReturnToOff(ref Transform startTransform, float targetAngle, ref bool isMerged)
	{
		this.tempVec.x = targetAngle;
		if (Mathf.Abs(this.tempVec.x - startTransform.localEulerAngles.x) > 180f)
		{
			if (this.tempVec.x > startTransform.localEulerAngles.x)
			{
				this.tempVec.x = this.tempVec.x - 360f;
			}
			else
			{
				this.tempVec.x = this.tempVec.x + 360f;
			}
		}
		if (!isMerged)
		{
			if (Mathf.Abs(startTransform.localEulerAngles.x - targetAngle) < 1f)
			{
				isMerged = true;
				return;
			}
			this.tempVec.x = (this.tempVec.x - startTransform.localEulerAngles.x) * this.slerp + startTransform.localEulerAngles.x;
			startTransform.localEulerAngles = this.tempVec;
		}
	}

	// Token: 0x040034E2 RID: 13538
	public Transform baseFire;

	// Token: 0x040034E3 RID: 13539
	public Transform middleFire;

	// Token: 0x040034E4 RID: 13540
	public Transform topFire;

	// Token: 0x040034E5 RID: 13541
	public float baseMultiplier;

	// Token: 0x040034E6 RID: 13542
	public float middleMultiplier;

	// Token: 0x040034E7 RID: 13543
	public float topMultiplier;

	// Token: 0x040034E8 RID: 13544
	public float bottomRange;

	// Token: 0x040034E9 RID: 13545
	public float middleRange;

	// Token: 0x040034EA RID: 13546
	public float topRange;

	// Token: 0x040034EB RID: 13547
	private float lastAngleBottom;

	// Token: 0x040034EC RID: 13548
	private float lastAngleMiddle;

	// Token: 0x040034ED RID: 13549
	private float lastAngleTop;

	// Token: 0x040034EE RID: 13550
	public float perlinStepBottom;

	// Token: 0x040034EF RID: 13551
	public float perlinStepMiddle;

	// Token: 0x040034F0 RID: 13552
	public float perlinStepTop;

	// Token: 0x040034F1 RID: 13553
	private float perlinBottom;

	// Token: 0x040034F2 RID: 13554
	private float perlinMiddle;

	// Token: 0x040034F3 RID: 13555
	private float perlinTop;

	// Token: 0x040034F4 RID: 13556
	public float startingRotationBottom;

	// Token: 0x040034F5 RID: 13557
	public float startingRotationMiddle;

	// Token: 0x040034F6 RID: 13558
	public float startingRotationTop;

	// Token: 0x040034F7 RID: 13559
	public float slerp = 0.01f;

	// Token: 0x040034F8 RID: 13560
	private bool mergedBottom;

	// Token: 0x040034F9 RID: 13561
	private bool mergedMiddle;

	// Token: 0x040034FA RID: 13562
	private bool mergedTop;

	// Token: 0x040034FB RID: 13563
	public string lastTimeOfDay;

	// Token: 0x040034FC RID: 13564
	public Material mat;

	// Token: 0x040034FD RID: 13565
	private float h;

	// Token: 0x040034FE RID: 13566
	private float s;

	// Token: 0x040034FF RID: 13567
	private float v;

	// Token: 0x04003500 RID: 13568
	public int overrideDayNight;

	// Token: 0x04003501 RID: 13569
	private Vector3 tempVec;

	// Token: 0x04003502 RID: 13570
	public bool[] isActive;

	// Token: 0x04003503 RID: 13571
	public bool wasActive;

	// Token: 0x04003504 RID: 13572
	private float lastTime;

	// Token: 0x04003505 RID: 13573
	public bool playDuringRain;
}
