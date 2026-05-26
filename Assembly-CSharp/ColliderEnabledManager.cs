using System;
using UnityEngine;

// Token: 0x02000039 RID: 57
public class ColliderEnabledManager : MonoBehaviour
{
	// Token: 0x060000E9 RID: 233 RVA: 0x00005D62 File Offset: 0x00003F62
	private void Start()
	{
		this.floorEnabled = true;
		this.floorCollidersEnabled = true;
		ColliderEnabledManager.instance = this;
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00005D78 File Offset: 0x00003F78
	private void OnDestroy()
	{
		ColliderEnabledManager.instance = null;
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00005D80 File Offset: 0x00003F80
	public void DisableFloorForFrame()
	{
		this.floorEnabled = false;
	}

	// Token: 0x060000EC RID: 236 RVA: 0x00005D8C File Offset: 0x00003F8C
	private void LateUpdate()
	{
		if (!this.floorEnabled && this.floorCollidersEnabled)
		{
			this.DisableFloor();
		}
		if (!this.floorCollidersEnabled && Time.time > this.timeDisabled + this.disableLength)
		{
			this.floorCollidersEnabled = true;
		}
		Collider[] array = this.floorCollider;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.floorCollidersEnabled;
		}
		if (this.floorCollidersEnabled)
		{
			GorillaSurfaceOverride[] array2 = this.walls;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].overrideIndex = this.wallsBeforeMaterial;
			}
		}
		else
		{
			GorillaSurfaceOverride[] array2 = this.walls;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].overrideIndex = this.wallsAfterMaterial;
			}
		}
		this.floorEnabled = true;
	}

	// Token: 0x060000ED RID: 237 RVA: 0x00005E4C File Offset: 0x0000404C
	private void DisableFloor()
	{
		this.floorCollidersEnabled = false;
		this.timeDisabled = Time.time;
	}

	// Token: 0x040000F5 RID: 245
	public static ColliderEnabledManager instance;

	// Token: 0x040000F6 RID: 246
	public Collider[] floorCollider;

	// Token: 0x040000F7 RID: 247
	public bool floorEnabled;

	// Token: 0x040000F8 RID: 248
	public bool wasFloorEnabled;

	// Token: 0x040000F9 RID: 249
	public bool floorCollidersEnabled;

	// Token: 0x040000FA RID: 250
	[GorillaSoundLookup]
	public int wallsBeforeMaterial;

	// Token: 0x040000FB RID: 251
	[GorillaSoundLookup]
	public int wallsAfterMaterial;

	// Token: 0x040000FC RID: 252
	public GorillaSurfaceOverride[] walls;

	// Token: 0x040000FD RID: 253
	public float timeDisabled;

	// Token: 0x040000FE RID: 254
	public float disableLength;
}
