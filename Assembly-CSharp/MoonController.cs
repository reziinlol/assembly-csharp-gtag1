using System;
using System.Collections.Generic;
using CjLib;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200022C RID: 556
public class MoonController : MonoBehaviour
{
	// Token: 0x17000169 RID: 361
	// (get) Token: 0x06000EB5 RID: 3765 RVA: 0x000504F3 File Offset: 0x0004E6F3
	public float Distance
	{
		get
		{
			return this.distance;
		}
	}

	// Token: 0x1700016A RID: 362
	// (get) Token: 0x06000EB6 RID: 3766 RVA: 0x000504FB File Offset: 0x0004E6FB
	private float TimeOfDay
	{
		get
		{
			if (this.debugOverrideTimeOfDay)
			{
				return Mathf.Repeat(this.timeOfDayOverride, 1f);
			}
			if (!(BetterDayNightManager.instance != null))
			{
				return 1f;
			}
			return BetterDayNightManager.instance.NormalizedTimeOfDay;
		}
	}

	// Token: 0x06000EB7 RID: 3767 RVA: 0x00050537 File Offset: 0x0004E737
	public void SetEyeOpenAnimation()
	{
		this.openMoonAnimator.SetBool(this.eyeOpenHash, true);
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x0005054B File Offset: 0x0004E74B
	public void StartEyeCloseAnimation()
	{
		this.openMoonAnimator.SetBool(this.eyeOpenHash, false);
	}

	// Token: 0x06000EB9 RID: 3769 RVA: 0x00050560 File Offset: 0x0004E760
	private void Start()
	{
		this.eyeOpenHash = Animator.StringToHash("EyeOpen");
		this.zoneToSceneMapping.Add(GTZone.forest, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.city, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.basement, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.canyon, MoonController.Scenes.Canyon);
		this.zoneToSceneMapping.Add(GTZone.beach, MoonController.Scenes.Beach);
		this.zoneToSceneMapping.Add(GTZone.mountain, MoonController.Scenes.Mountain);
		this.zoneToSceneMapping.Add(GTZone.skyJungle, MoonController.Scenes.Clouds);
		this.zoneToSceneMapping.Add(GTZone.cave, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.cityWithSkyJungle, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.tutorial, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.rotating, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.none, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.Metropolis, MoonController.Scenes.Metropolis);
		this.zoneToSceneMapping.Add(GTZone.cityNoBuildings, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.attic, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.arcade, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.bayou, MoonController.Scenes.Bayou);
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager.Instance.RegisterMoon(this);
		}
		this.crackStartDayOfYear = new DateTime(2024, 10, 4).DayOfYear;
		this.crackEndDayOfYear = new DateTime(2024, 10, 25).DayOfYear;
		if (this.crackRenderer != null)
		{
			this.currentlySetCrackProgress = 1f;
			this.crackMaterialPropertyBlock = new MaterialPropertyBlock();
			this.crackRenderer.GetPropertyBlock(this.crackMaterialPropertyBlock);
			this.crackMaterialPropertyBlock.SetFloat(ShaderProps._Progress, this.currentlySetCrackProgress);
			this.crackRenderer.SetPropertyBlock(this.crackMaterialPropertyBlock);
		}
		this.orbitAngle = 0f;
		this.UpdateCrack();
		this.UpdatePlacement();
	}

	// Token: 0x06000EBA RID: 3770 RVA: 0x0005075B File Offset: 0x0004E95B
	private void OnDestroy()
	{
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager.Instance.UnregisterMoon(this);
		}
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x0005077C File Offset: 0x0004E97C
	private void OnZoneChanged()
	{
		ZoneManagement instance = ZoneManagement.instance;
		MoonController.Scenes scenes = MoonController.Scenes.Forest;
		for (int i = 0; i < instance.activeZones.Count; i++)
		{
			MoonController.Scenes scenes2;
			if (this.zoneToSceneMapping.TryGetValue(instance.activeZones[i], out scenes2) && scenes2 > scenes)
			{
				scenes = scenes2;
			}
		}
		this.UpdateActiveScene(scenes);
	}

	// Token: 0x06000EBC RID: 3772 RVA: 0x000507CF File Offset: 0x0004E9CF
	private void UpdateActiveScene(MoonController.Scenes nextScene)
	{
		this.activeScene = nextScene;
		this.UpdateCrack();
		this.UpdatePlacement();
	}

	// Token: 0x06000EBD RID: 3773 RVA: 0x000507E4 File Offset: 0x0004E9E4
	private void Update()
	{
		this.UpdateCrack();
		if (!this.alwaysInTheSky)
		{
			float timeOfDay = this.TimeOfDay;
			bool flag = timeOfDay > 0.53999996f && timeOfDay < 0.6733333f;
			bool flag2 = timeOfDay > 0.086666666f && timeOfDay < 0.22f;
			bool flag3 = timeOfDay <= 0.086666666f || timeOfDay >= 0.6733333f;
			if (timeOfDay >= 0.22f)
			{
				bool flag4 = timeOfDay <= 0.53999996f;
			}
			float num = this.orbitAngle;
			if (flag)
			{
				num = Mathf.Lerp(3.1415927f, 0f, (timeOfDay - 0.53999996f) / 0.13333333f);
			}
			else if (flag2)
			{
				num = Mathf.Lerp(0f, -3.1415927f, (timeOfDay - 0.086666666f) / 0.13333333f);
			}
			else if (flag3)
			{
				num = 0f;
			}
			else
			{
				num = 3.1415927f;
			}
			if (this.orbitAngle != num)
			{
				this.orbitAngle = num;
				this.UpdateCrack();
				this.UpdatePlacement();
			}
		}
	}

	// Token: 0x06000EBE RID: 3774 RVA: 0x000508D5 File Offset: 0x0004EAD5
	public void UpdateDistance(float nextDistance)
	{
		this.distance = nextDistance;
		this.UpdateVisualState();
		this.UpdatePlacement();
	}

	// Token: 0x06000EBF RID: 3775 RVA: 0x000508EC File Offset: 0x0004EAEC
	public void UpdateVisualState()
	{
		bool flag = false;
		if (GreyZoneManager.Instance != null)
		{
			flag = GreyZoneManager.Instance.GreyZoneActive;
		}
		if (flag && this.openEyeModelEnabled && this.distance < this.eyeOpenDistThreshold && !this.openMoonAnimator.GetBool(this.eyeOpenHash))
		{
			this.openMoonAnimator.SetBool(this.eyeOpenHash, true);
			return;
		}
		if (!flag && this.distance > this.eyeCloseDistThreshold && this.openMoonAnimator.GetBool(this.eyeOpenHash))
		{
			this.openMoonAnimator.SetBool(this.eyeOpenHash, false);
		}
	}

	// Token: 0x06000EC0 RID: 3776 RVA: 0x0005098C File Offset: 0x0004EB8C
	public void UpdatePlacement()
	{
		if (this.alwaysInTheSky)
		{
			this.UpdatePlacementSimple();
			return;
		}
		this.UpdatePlacementOrbit();
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x000509A4 File Offset: 0x0004EBA4
	private void UpdatePlacementSimple()
	{
		MoonController.SceneData sceneData = this.scenes[(int)this.activeScene];
		Transform referencePoint = sceneData.referencePoint;
		MoonController.Placement placement = sceneData.overridePlacement ? sceneData.PlacementOverride : this.defaultPlacement;
		float num = Mathf.Lerp(placement.heightRange.x, placement.heightRange.y, this.distance);
		float num2 = Mathf.Lerp(placement.radiusRange.x, placement.radiusRange.y, this.distance);
		float d = Mathf.Lerp(placement.scaleRange.x, placement.scaleRange.y, this.distance);
		float restAngle = placement.restAngle;
		Vector3 position = referencePoint.position;
		position.y += num;
		position.x += num2 * Mathf.Cos(restAngle * 0.017453292f);
		position.z += num2 * Mathf.Sin(restAngle * 0.017453292f);
		base.transform.position = position;
		base.transform.rotation = Quaternion.LookRotation(referencePoint.position - base.transform.position);
		base.transform.localScale = Vector3.one * d;
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x00050AE8 File Offset: 0x0004ECE8
	public void UpdatePlacementOrbit()
	{
		MoonController.SceneData sceneData = this.scenes[(int)this.activeScene];
		Transform referencePoint = sceneData.referencePoint;
		MoonController.Placement placement = sceneData.overridePlacement ? sceneData.PlacementOverride : this.defaultPlacement;
		float y = placement.heightRange.y;
		float y2 = placement.radiusRange.y;
		Vector3 position = referencePoint.position;
		position.y += y;
		position.x += y2 * Mathf.Cos(placement.restAngle * 0.017453292f);
		position.z += y2 * Mathf.Sin(placement.restAngle * 0.017453292f);
		float d = Mathf.Sqrt(y * y + y2 * y2);
		float num = Mathf.Atan2(y, y2);
		Quaternion rotation = Quaternion.AngleAxis(57.29578f * num, Vector3.Cross(position - referencePoint.position, Vector3.up));
		float f = placement.restAngle * 0.017453292f + this.orbitAngle;
		Vector3 vector = referencePoint.position + rotation * new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f)) * d;
		if (this.distance < 1f)
		{
			Vector3 position2 = referencePoint.position;
			position2.y += placement.heightRange.x;
			position2.x += placement.radiusRange.x * Mathf.Cos(placement.restAngle * 0.017453292f);
			position2.z += placement.radiusRange.x * Mathf.Sin(placement.restAngle * 0.017453292f);
			if (Mathf.Abs(this.orbitAngle) < 0.9424779f)
			{
				vector = Vector3.Lerp(position2, vector, this.distance);
			}
			else
			{
				vector = Vector3.Lerp(position2, position, this.distance);
			}
		}
		base.transform.position = vector;
		base.transform.rotation = Quaternion.LookRotation(referencePoint.position - base.transform.position);
		base.transform.localScale = Vector3.one * Mathf.Lerp(placement.scaleRange.x, placement.scaleRange.y, this.distance);
		if (this.debugDrawOrbit)
		{
			int num2 = 32;
			float timeOfDay = this.TimeOfDay;
			float num3 = 0.086666666f;
			float num4 = 0.24666667f;
			float num5 = 0.6333333f;
			float num6 = 0.76f;
			bool flag = timeOfDay > num3 && timeOfDay < num4;
			bool flag2 = timeOfDay > num5 && timeOfDay < num6;
			bool flag3 = timeOfDay <= num3 || timeOfDay >= num6;
			if (timeOfDay >= num4)
			{
				bool flag4 = timeOfDay <= num5;
			}
			Color color = flag2 ? Color.red : (flag3 ? Color.green : (flag ? Color.yellow : Color.blue));
			Vector3 v = referencePoint.position + rotation * new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)) * d;
			for (int i = 1; i <= num2; i++)
			{
				float num7 = (float)i / (float)num2;
				Vector3 vector2 = referencePoint.position + rotation * new Vector3(Mathf.Cos(6.2831855f * num7), 0f, Mathf.Sin(6.2831855f * num7)) * d;
				DebugUtil.DrawLine(v, vector2, color, false);
				v = vector2;
			}
		}
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x00050E78 File Offset: 0x0004F078
	private void UpdateCrack()
	{
		bool flag = GreyZoneManager.Instance != null && GreyZoneManager.Instance.GreyZoneAvailable;
		if (flag && !this.openEyeModelEnabled)
		{
			this.openEyeModelEnabled = true;
			this.defaultMoon.gameObject.SetActive(false);
			this.openMoon.gameObject.SetActive(true);
		}
		else if (!flag && this.openEyeModelEnabled)
		{
			this.openEyeModelEnabled = false;
			this.defaultMoon.gameObject.SetActive(true);
			this.openMoon.gameObject.SetActive(false);
		}
		if (!flag && GorillaComputer.instance != null)
		{
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			if (this.debugOverrideCrackDayInOctober)
			{
				serverTime = new DateTime(2024, 10, Mathf.Clamp(this.crackDayInOctoberOverride, 1, 31));
			}
			float value = Mathf.InverseLerp((float)this.crackStartDayOfYear, (float)this.crackEndDayOfYear, (float)serverTime.DayOfYear);
			if (this.debugOverrideCrackProgress)
			{
				value = this.crackProgress;
			}
			float num = 1f - Mathf.Clamp01(value);
			if (this.crackRenderer != null && Mathf.Abs(num - this.currentlySetCrackProgress) > Mathf.Epsilon)
			{
				this.currentlySetCrackProgress = num;
				this.crackMaterialPropertyBlock.SetFloat(ShaderProps._Progress, this.currentlySetCrackProgress);
				this.crackRenderer.SetPropertyBlock(this.crackMaterialPropertyBlock);
			}
		}
	}

	// Token: 0x040011BC RID: 4540
	[SerializeField]
	private List<MoonController.SceneData> scenes = new List<MoonController.SceneData>();

	// Token: 0x040011BD RID: 4541
	[SerializeField]
	private MoonController.Scenes activeScene;

	// Token: 0x040011BE RID: 4542
	[SerializeField]
	private MoonController.Placement defaultPlacement;

	// Token: 0x040011BF RID: 4543
	[SerializeField]
	[Range(0f, 1f)]
	private float distance;

	// Token: 0x040011C0 RID: 4544
	[SerializeField]
	private bool alwaysInTheSky;

	// Token: 0x040011C1 RID: 4545
	[Header("Model Swap")]
	[SerializeField]
	private Transform defaultMoon;

	// Token: 0x040011C2 RID: 4546
	[SerializeField]
	private Transform openMoon;

	// Token: 0x040011C3 RID: 4547
	[Header("Animation")]
	[SerializeField]
	private Animator openMoonAnimator;

	// Token: 0x040011C4 RID: 4548
	[SerializeField]
	private float eyeOpenDistThreshold = 0.9f;

	// Token: 0x040011C5 RID: 4549
	[SerializeField]
	private float eyeCloseDistThreshold = 0.05f;

	// Token: 0x040011C6 RID: 4550
	[Header("Debug")]
	[SerializeField]
	private bool debugOverrideTimeOfDay;

	// Token: 0x040011C7 RID: 4551
	[SerializeField]
	[Range(0f, 4f)]
	private float timeOfDayOverride;

	// Token: 0x040011C8 RID: 4552
	[SerializeField]
	private bool debugOverrideCrackProgress;

	// Token: 0x040011C9 RID: 4553
	[SerializeField]
	[Range(0f, 1f)]
	private float crackProgress;

	// Token: 0x040011CA RID: 4554
	[SerializeField]
	private bool debugOverrideCrackDayInOctober;

	// Token: 0x040011CB RID: 4555
	[SerializeField]
	[Range(1f, 31f)]
	private int crackDayInOctoberOverride = 4;

	// Token: 0x040011CC RID: 4556
	[SerializeField]
	private MeshRenderer crackRenderer;

	// Token: 0x040011CD RID: 4557
	private int crackStartDayOfYear;

	// Token: 0x040011CE RID: 4558
	private int crackEndDayOfYear;

	// Token: 0x040011CF RID: 4559
	private float orbitAngle;

	// Token: 0x040011D0 RID: 4560
	private int eyeOpenHash;

	// Token: 0x040011D1 RID: 4561
	private bool openEyeModelEnabled;

	// Token: 0x040011D2 RID: 4562
	private float currentlySetCrackProgress;

	// Token: 0x040011D3 RID: 4563
	private MaterialPropertyBlock crackMaterialPropertyBlock;

	// Token: 0x040011D4 RID: 4564
	private bool debugDrawOrbit;

	// Token: 0x040011D5 RID: 4565
	private Dictionary<GTZone, MoonController.Scenes> zoneToSceneMapping = new Dictionary<GTZone, MoonController.Scenes>();

	// Token: 0x040011D6 RID: 4566
	private const float moonFallStart = 0.086666666f;

	// Token: 0x040011D7 RID: 4567
	private const float moonFallEnd = 0.22f;

	// Token: 0x040011D8 RID: 4568
	private const float moonRiseStart = 0.53999996f;

	// Token: 0x040011D9 RID: 4569
	private const float moonRiseEnd = 0.6733333f;

	// Token: 0x0200022D RID: 557
	public enum Scenes
	{
		// Token: 0x040011DB RID: 4571
		Forest,
		// Token: 0x040011DC RID: 4572
		Bayou,
		// Token: 0x040011DD RID: 4573
		Beach,
		// Token: 0x040011DE RID: 4574
		Canyon,
		// Token: 0x040011DF RID: 4575
		Clouds,
		// Token: 0x040011E0 RID: 4576
		City,
		// Token: 0x040011E1 RID: 4577
		Metropolis,
		// Token: 0x040011E2 RID: 4578
		Mountain
	}

	// Token: 0x0200022E RID: 558
	[Serializable]
	public struct SceneData
	{
		// Token: 0x040011E3 RID: 4579
		public MoonController.Scenes scene;

		// Token: 0x040011E4 RID: 4580
		public Transform referencePoint;

		// Token: 0x040011E5 RID: 4581
		public bool overridePlacement;

		// Token: 0x040011E6 RID: 4582
		public MoonController.Placement PlacementOverride;
	}

	// Token: 0x0200022F RID: 559
	[Serializable]
	public struct Placement
	{
		// Token: 0x040011E7 RID: 4583
		public Vector2 radiusRange;

		// Token: 0x040011E8 RID: 4584
		public Vector2 heightRange;

		// Token: 0x040011E9 RID: 4585
		public Vector2 scaleRange;

		// Token: 0x040011EA RID: 4586
		public float restAngle;
	}
}
