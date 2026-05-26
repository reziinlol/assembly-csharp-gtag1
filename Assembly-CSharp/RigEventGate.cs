using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004D8 RID: 1240
public class RigEventGate : MonoBehaviour, IBuildValidation
{
	// Token: 0x06001E31 RID: 7729 RVA: 0x000A1948 File Offset: 0x0009FB48
	private void OnEnable()
	{
		if (this.rigCollection == null)
		{
			return;
		}
		VRRigCollection vrrigCollection = this.rigCollection;
		vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.OnJoined));
		VRRigCollection vrrigCollection2 = this.rigCollection;
		vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.OnLeft));
	}

	// Token: 0x06001E32 RID: 7730 RVA: 0x000A19B4 File Offset: 0x0009FBB4
	private void OnDisable()
	{
		if (this.rigCollection == null)
		{
			return;
		}
		VRRigCollection vrrigCollection = this.rigCollection;
		vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.OnJoined));
		VRRigCollection vrrigCollection2 = this.rigCollection;
		vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.OnLeft));
	}

	// Token: 0x06001E33 RID: 7731 RVA: 0x000A1A20 File Offset: 0x0009FC20
	private void OnDestroy()
	{
		if (this.rigCollection == null)
		{
			return;
		}
		VRRigCollection vrrigCollection = this.rigCollection;
		vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.OnJoined));
		VRRigCollection vrrigCollection2 = this.rigCollection;
		vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Remove(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.OnLeft));
	}

	// Token: 0x06001E34 RID: 7732 RVA: 0x000A1A8C File Offset: 0x0009FC8C
	private void OnJoined(RigContainer rc)
	{
		int num = (this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count;
		this.countChanged(this.gameObjects.Count, this.gameObjects.Count, num - 1, num, null);
	}

	// Token: 0x06001E35 RID: 7733 RVA: 0x000A1ADC File Offset: 0x0009FCDC
	private void OnLeft(RigContainer rc)
	{
		RigEventVolumeTrigger rigEventVolumeTrigger = null;
		for (int i = 0; i < this.gameObjects.Count; i++)
		{
			if (this.gameObjects[i].Rig == rc.Rig)
			{
				rigEventVolumeTrigger = this.gameObjects[i];
			}
		}
		int num = (this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count;
		if (rigEventVolumeTrigger != null)
		{
			this.gameObjects.Remove(rigEventVolumeTrigger);
			this.countChanged(this.gameObjects.Count + 1, this.gameObjects.Count, num + 1, num, null);
			return;
		}
		this.countChanged(this.gameObjects.Count, this.gameObjects.Count, num + 1, num, null);
	}

	// Token: 0x06001E36 RID: 7734 RVA: 0x000A1BAC File Offset: 0x0009FDAC
	private void OnTriggerEnter(Collider other)
	{
		RigEventVolumeTrigger rigEventVolumeTrigger;
		if (!other.gameObject.TryGetComponent<RigEventVolumeTrigger>(out rigEventVolumeTrigger) || base.transform.InverseTransformPoint(rigEventVolumeTrigger.transform.position).z < 0f)
		{
			return;
		}
		if (this.gameObjects.Contains(rigEventVolumeTrigger))
		{
			int num = (this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count;
			int count = this.gameObjects.Count;
			this.gameObjects.Remove(rigEventVolumeTrigger);
			this.countChanged(count, this.gameObjects.Count, num, num, rigEventVolumeTrigger);
		}
	}

	// Token: 0x06001E37 RID: 7735 RVA: 0x000A1C4C File Offset: 0x0009FE4C
	private void OnTriggerExit(Collider other)
	{
		RigEventVolumeTrigger rigEventVolumeTrigger;
		if (!other.gameObject.TryGetComponent<RigEventVolumeTrigger>(out rigEventVolumeTrigger) || base.transform.InverseTransformPoint(rigEventVolumeTrigger.transform.position).z < 0f)
		{
			return;
		}
		if (!this.gameObjects.Contains(rigEventVolumeTrigger))
		{
			int num = (this.rigCollection == null) ? 1 : this.rigCollection.Rigs.Count;
			int count = this.gameObjects.Count;
			this.gameObjects.Add(rigEventVolumeTrigger);
			this.countChanged(count, this.gameObjects.Count, num, num, rigEventVolumeTrigger);
		}
	}

	// Token: 0x06001E38 RID: 7736 RVA: 0x000A1CE8 File Offset: 0x0009FEE8
	private void countChanged(int oldValue, int newValue, int oldPlayerCount, int newPlayerCount, RigEventVolumeTrigger rig)
	{
		if (newValue > oldValue)
		{
			if (rig != null)
			{
				UnityEvent<VRRig> rigExits = this.RigExits;
				if (rigExits != null)
				{
					rigExits.Invoke(rig.Rig);
				}
			}
			if ((this.mode == RigEventGate.Mode.RELATIVE && (float)newValue / (float)newPlayerCount >= this.relThreshold && (float)oldValue / (float)oldPlayerCount < this.relThreshold) || (this.mode == RigEventGate.Mode.ABSOLUTE && newValue >= this.absThreshold && oldValue < this.absThreshold))
			{
				UnityEvent goesOverThreshold = this.GoesOverThreshold;
				if (goesOverThreshold == null)
				{
					return;
				}
				goesOverThreshold.Invoke();
			}
		}
	}

	// Token: 0x06001E39 RID: 7737 RVA: 0x000A1D69 File Offset: 0x0009FF69
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.mode == RigEventGate.Mode.RELATIVE && this.rigCollection == null)
		{
			Debug.Log("RigEventGate on " + base.name + " is set to RELATIVE mode but has no Player Count Source. This will crash!");
			return false;
		}
		return true;
	}

	// Token: 0x04002855 RID: 10325
	private List<RigEventVolumeTrigger> gameObjects = new List<RigEventVolumeTrigger>();

	// Token: 0x04002856 RID: 10326
	[SerializeField]
	private RigEventGate.Mode mode = RigEventGate.Mode.ABSOLUTE;

	// Token: 0x04002857 RID: 10327
	[Range(0.05f, 1f)]
	[SerializeField]
	private float relThreshold = 0.05f;

	// Token: 0x04002858 RID: 10328
	[SerializeField]
	private VRRigCollection rigCollection;

	// Token: 0x04002859 RID: 10329
	[Range(1f, 20f)]
	[SerializeField]
	private int absThreshold = 1;

	// Token: 0x0400285A RID: 10330
	[SerializeField]
	private UnityEvent<VRRig> RigExits;

	// Token: 0x0400285B RID: 10331
	[SerializeField]
	private UnityEvent GoesOverThreshold;

	// Token: 0x020004D9 RID: 1241
	private enum Mode
	{
		// Token: 0x0400285D RID: 10333
		RELATIVE,
		// Token: 0x0400285E RID: 10334
		ABSOLUTE
	}
}
