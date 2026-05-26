using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200033D RID: 829
public class GorillaStatusToThermalTemperatureMono : MonoBehaviour, ISpawnable
{
	// Token: 0x17000205 RID: 517
	// (get) Token: 0x06001453 RID: 5203 RVA: 0x0006CF0E File Offset: 0x0006B10E
	// (set) Token: 0x06001454 RID: 5204 RVA: 0x0006CF16 File Offset: 0x0006B116
	public bool hasRig { get; private set; }

	// Token: 0x17000206 RID: 518
	// (get) Token: 0x06001455 RID: 5205 RVA: 0x0006CF1F File Offset: 0x0006B11F
	public VRRig rig
	{
		get
		{
			return this.m_rig;
		}
	}

	// Token: 0x06001456 RID: 5206 RVA: 0x0006CF28 File Offset: 0x0006B128
	public void SetRig(VRRig newRig)
	{
		if (newRig == this.m_rig)
		{
			return;
		}
		if (this.hasRig)
		{
			VRRig rig = this.m_rig;
			rig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(rig.OnMaterialIndexChanged, new Action<int, int>(this._OnMatChanged));
		}
		this.m_rig = newRig;
		this.hasRig = (newRig != null);
		if (!this.hasRig || !base.isActiveAndEnabled)
		{
			return;
		}
		VRRig rig2 = this.m_rig;
		rig2.OnMaterialIndexChanged = (Action<int, int>)Delegate.Combine(rig2.OnMaterialIndexChanged, new Action<int, int>(this._OnMatChanged));
		this._InitRuntimeArray();
		this._OnMatChanged(-1, this.m_rig.setMatIndex);
	}

	// Token: 0x06001457 RID: 5207 RVA: 0x0006CFD7 File Offset: 0x0006B1D7
	protected void Awake()
	{
		this.hasRig = (this.m_rig != null);
		this._InitRuntimeArray();
	}

	// Token: 0x06001458 RID: 5208 RVA: 0x0006CFF4 File Offset: 0x0006B1F4
	private void _InitRuntimeArray()
	{
		if (!this.hasRig || this._runtimeMatIndexes_to_temperatures != null)
		{
			return;
		}
		int num = VRRig.LocalRig.materialsToChangeTo.Length;
		this._runtimeMatIndexes_to_temperatures = new float[num];
		for (int i = 0; i < this._runtimeMatIndexes_to_temperatures.Length; i++)
		{
			this._runtimeMatIndexes_to_temperatures[i] = -32768f;
		}
		foreach (GorillaStatusToThermalTemperatureMono._MaterialIndexToTemperature materialIndexToTemperature in this.m_materialIndexesToTemperatures)
		{
			foreach (int num2 in materialIndexToTemperature.matIndexes)
			{
				if (num2 >= 0 && num2 < num)
				{
					this._runtimeMatIndexes_to_temperatures[num2] = materialIndexToTemperature.temperature;
				}
			}
		}
		if (!Application.isEditor)
		{
			this.m_materialIndexesToTemperatures = null;
		}
	}

	// Token: 0x06001459 RID: 5209 RVA: 0x0006D0B4 File Offset: 0x0006B2B4
	protected void OnEnable()
	{
		if (!this.hasRig || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.m_thermalSourceVolume == null)
		{
			GTDev.LogError<string>("[GorillaStatusToThermalTemperatureMono]  ERROR!!!  Disabling because thermal source is not assigned. Path=" + base.transform.GetPathQ(), this, null);
			base.enabled = false;
			return;
		}
		VRRig rig = this.m_rig;
		rig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Combine(rig.OnMaterialIndexChanged, new Action<int, int>(this._OnMatChanged));
		this._OnMatChanged(-1, this.m_rig.setMatIndex);
	}

	// Token: 0x0600145A RID: 5210 RVA: 0x0006D13C File Offset: 0x0006B33C
	protected void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting || !this.hasRig)
		{
			return;
		}
		VRRig rig = this.m_rig;
		rig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(rig.OnMaterialIndexChanged, new Action<int, int>(this._OnMatChanged));
	}

	// Token: 0x0600145B RID: 5211 RVA: 0x0006D178 File Offset: 0x0006B378
	private void _OnMatChanged(int oldIndex, int newIndex)
	{
		float num = this._runtimeMatIndexes_to_temperatures[newIndex];
		this.m_thermalSourceVolume.celsius = num;
		this.m_thermalSourceVolume.enabled = (num > -32767.99f);
	}

	// Token: 0x17000207 RID: 519
	// (get) Token: 0x0600145C RID: 5212 RVA: 0x0006D1AD File Offset: 0x0006B3AD
	// (set) Token: 0x0600145D RID: 5213 RVA: 0x0006D1B5 File Offset: 0x0006B3B5
	public bool IsSpawned { get; set; }

	// Token: 0x17000208 RID: 520
	// (get) Token: 0x0600145E RID: 5214 RVA: 0x0006D1BE File Offset: 0x0006B3BE
	// (set) Token: 0x0600145F RID: 5215 RVA: 0x0006D1C6 File Offset: 0x0006B3C6
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001460 RID: 5216 RVA: 0x0006D1CF File Offset: 0x0006B3CF
	public void OnSpawn(VRRig newRig)
	{
		this.SetRig(newRig);
	}

	// Token: 0x06001461 RID: 5217 RVA: 0x0006D1D8 File Offset: 0x0006B3D8
	public void OnDespawn()
	{
		this.SetRig(null);
	}

	// Token: 0x04001924 RID: 6436
	private const string preLog = "[GorillaStatusToThermalTemperatureMono]  ";

	// Token: 0x04001925 RID: 6437
	private const string preErr = "[GorillaStatusToThermalTemperatureMono]  ERROR!!!  ";

	// Token: 0x04001926 RID: 6438
	[Tooltip("Should either be assigned here or via another script.")]
	[SerializeField]
	private VRRig m_rig;

	// Token: 0x04001928 RID: 6440
	[SerializeField]
	private ThermalSourceVolume m_thermalSourceVolume;

	// Token: 0x04001929 RID: 6441
	[SerializeField]
	private GorillaStatusToThermalTemperatureMono._MaterialIndexToTemperature[] m_materialIndexesToTemperatures;

	// Token: 0x0400192A RID: 6442
	[DebugReadout]
	private float[] _runtimeMatIndexes_to_temperatures;

	// Token: 0x0400192B RID: 6443
	private const float _k_invalidTemperature = -32768f;

	// Token: 0x0200033E RID: 830
	[Serializable]
	private struct _MaterialIndexToTemperature
	{
		// Token: 0x0400192E RID: 6446
		public int[] matIndexes;

		// Token: 0x0400192F RID: 6447
		public float temperature;
	}
}
