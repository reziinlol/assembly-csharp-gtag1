using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000CA RID: 202
public class ShadeRevealer : TransferrableObject
{
	// Token: 0x060004E5 RID: 1253 RVA: 0x0001B398 File Offset: 0x00019598
	protected override void Awake()
	{
		base.Awake();
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		for (int i = 0; i < this.enableWhenScanning.Length; i++)
		{
			hashSet.Add(this.enableWhenScanning[i]);
		}
		for (int j = 0; j < this.enableWhenTracking.Length; j++)
		{
			hashSet.Add(this.enableWhenTracking[j]);
		}
		for (int k = 0; k < this.enableWhenLocked.Length; k++)
		{
			hashSet.Add(this.enableWhenLocked[k]);
		}
		for (int l = 0; l < this.enableWhenPrimed.Length; l++)
		{
			hashSet.Add(this.enableWhenPrimed[l]);
		}
		this.objectsToDisableWhenOff = new GameObject[hashSet.Count];
		hashSet.CopyTo(this.objectsToDisableWhenOff);
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x0001B45C File Offset: 0x0001965C
	private float GetDistanceToBeamRay(Vector3 toPosition)
	{
		return Vector3.Cross(this.beamForward.forward, toPosition).magnitude;
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x0001B484 File Offset: 0x00019684
	public ShadeRevealer.State GetBeamStateForPosition(Vector3 toPosition, float tolerance)
	{
		if (toPosition.magnitude <= this.beamLength + tolerance && Vector3.Dot(toPosition.normalized, this.beamForward.forward) > 0f)
		{
			float num = this.GetDistanceToBeamRay(toPosition) - tolerance;
			if (num <= this.lockThreshold)
			{
				return ShadeRevealer.State.LOCKED;
			}
			if (num <= this.trackThreshold)
			{
				return ShadeRevealer.State.TRACKING;
			}
		}
		return ShadeRevealer.State.SCANNING;
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x0001B4E1 File Offset: 0x000196E1
	public ShadeRevealer.State GetBeamStateForCritter(CosmeticCritter critter, float tolerance)
	{
		return this.GetBeamStateForPosition(critter.transform.position - this.beamForward.position, tolerance);
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x0001B505 File Offset: 0x00019705
	public bool CritterWithinBeamThreshold(CosmeticCritter critter, ShadeRevealer.State criteria, float tolerance)
	{
		return this.GetBeamStateForCritter(critter, tolerance) >= criteria;
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x0001B515 File Offset: 0x00019715
	public void SetBestBeamState(ShadeRevealer.State state)
	{
		if (state > this.pendingBeamState)
		{
			this.pendingBeamState = state;
		}
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x0001B528 File Offset: 0x00019728
	private void SetObjectsEnabledFromState(ShadeRevealer.State state)
	{
		for (int i = 0; i < this.objectsToDisableWhenOff.Length; i++)
		{
			this.objectsToDisableWhenOff[i].SetActive(false);
		}
		GameObject[] array;
		switch (state)
		{
		case ShadeRevealer.State.SCANNING:
			array = this.enableWhenScanning;
			break;
		case ShadeRevealer.State.TRACKING:
			array = this.enableWhenTracking;
			break;
		case ShadeRevealer.State.LOCKED:
			array = this.enableWhenLocked;
			break;
		case ShadeRevealer.State.PRIMED:
			array = this.enableWhenPrimed;
			break;
		default:
			return;
		}
		for (int j = 0; j < array.Length; j++)
		{
			array[j].SetActive(true);
		}
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x0001B5A8 File Offset: 0x000197A8
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.currentBeamState != this.pendingBeamState)
		{
			this.currentBeamState = this.pendingBeamState;
			this.SetObjectsEnabledFromState(this.currentBeamState);
		}
		this.beamSFX.pitch = 1f + this.shadeCatcher.GetActionTimeFrac() * 2f;
		if (this.isScanning)
		{
			this.pendingBeamState = ShadeRevealer.State.SCANNING;
		}
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x0001B612 File Offset: 0x00019812
	public void StartScanning()
	{
		this.shadeCatcher.enabled = true;
		this.initialActivationSFX.GTPlay();
		this.beamSFX.GTPlay();
		this.isScanning = true;
		this.currentBeamState = ShadeRevealer.State.OFF;
		this.pendingBeamState = ShadeRevealer.State.SCANNING;
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x0001B64C File Offset: 0x0001984C
	public void StopScanning()
	{
		if (this.currentBeamState == ShadeRevealer.State.PRIMED)
		{
			UnityEvent unityEvent = this.onShadeLaunched;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}
		this.shadeCatcher.enabled = false;
		this.initialActivationSFX.GTStop();
		this.beamSFX.GTStop();
		this.isScanning = false;
		this.currentBeamState = ShadeRevealer.State.OFF;
		this.pendingBeamState = ShadeRevealer.State.OFF;
		this.SetObjectsEnabledFromState(ShadeRevealer.State.OFF);
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x0001B6B4 File Offset: 0x000198B4
	public void ShadeCaught()
	{
		this.shadeCatcher.enabled = false;
		this.beamSFX.GTStop();
		this.catchSFX.GTPlay();
		this.catchFX.Play();
		this.isScanning = false;
		this.currentBeamState = ShadeRevealer.State.OFF;
		this.pendingBeamState = ShadeRevealer.State.PRIMED;
	}

	// Token: 0x0400057F RID: 1407
	[SerializeField]
	private AudioSource initialActivationSFX;

	// Token: 0x04000580 RID: 1408
	[SerializeField]
	private AudioSource beamSFX;

	// Token: 0x04000581 RID: 1409
	[SerializeField]
	private AudioSource catchSFX;

	// Token: 0x04000582 RID: 1410
	[SerializeField]
	private ParticleSystem catchFX;

	// Token: 0x04000583 RID: 1411
	[Space]
	[SerializeField]
	private CosmeticCritterCatcherShade shadeCatcher;

	// Token: 0x04000584 RID: 1412
	[Space]
	[Tooltip("The transform that represents the origin of the revealer beam.")]
	[SerializeField]
	private Transform beamForward;

	// Token: 0x04000585 RID: 1413
	[Tooltip("The maximum length of the beam.")]
	[SerializeField]
	private float beamLength;

	// Token: 0x04000586 RID: 1414
	[Tooltip("If the Shade is this close to the beam, set it to flee and have all Revealers enter Tracking mode.")]
	[SerializeField]
	private float trackThreshold;

	// Token: 0x04000587 RID: 1415
	[Tooltip("If the Shade is this close to the beam, slow it down.")]
	[SerializeField]
	private float lockThreshold;

	// Token: 0x04000588 RID: 1416
	[Tooltip("Editor-only object to help test the thresholds.")]
	[SerializeField]
	private Transform thresholdTester;

	// Token: 0x04000589 RID: 1417
	[Tooltip("Whether to draw the tester or not.")]
	[SerializeField]
	private bool drawThresholdTesterInEditor = true;

	// Token: 0x0400058A RID: 1418
	[Space]
	[Tooltip("Enable these objects while the beam is in Scanning mode.")]
	[SerializeField]
	private GameObject[] enableWhenScanning;

	// Token: 0x0400058B RID: 1419
	[Tooltip("Enable these objects while the beam is in Tracking mode.")]
	[SerializeField]
	private GameObject[] enableWhenTracking;

	// Token: 0x0400058C RID: 1420
	[Tooltip("Enable these objects while the beam is in Locked mode.")]
	[SerializeField]
	private GameObject[] enableWhenLocked;

	// Token: 0x0400058D RID: 1421
	[Tooltip("Enable these objects while ready to fire.")]
	[SerializeField]
	private GameObject[] enableWhenPrimed;

	// Token: 0x0400058E RID: 1422
	[Space]
	[SerializeField]
	private UnityEvent onShadeLaunched;

	// Token: 0x0400058F RID: 1423
	private bool isScanning;

	// Token: 0x04000590 RID: 1424
	private ShadeRevealer.State currentBeamState;

	// Token: 0x04000591 RID: 1425
	private ShadeRevealer.State pendingBeamState;

	// Token: 0x04000592 RID: 1426
	private GameObject[] objectsToDisableWhenOff;

	// Token: 0x020000CB RID: 203
	public enum State
	{
		// Token: 0x04000594 RID: 1428
		OFF,
		// Token: 0x04000595 RID: 1429
		SCANNING,
		// Token: 0x04000596 RID: 1430
		TRACKING,
		// Token: 0x04000597 RID: 1431
		LOCKED,
		// Token: 0x04000598 RID: 1432
		PRIMED
	}
}
