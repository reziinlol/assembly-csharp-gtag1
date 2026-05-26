using System;
using System.Collections.Generic;
using System.Reflection;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// Token: 0x020004E7 RID: 1255
public class CalibrationCube : MonoBehaviour
{
	// Token: 0x06001E79 RID: 7801 RVA: 0x000A2B2F File Offset: 0x000A0D2F
	private void Awake()
	{
		this.calibratedLength = this.baseLength;
	}

	// Token: 0x06001E7A RID: 7802 RVA: 0x000A2B40 File Offset: 0x000A0D40
	private void Start()
	{
		try
		{
			this.OnCollisionExit(null);
		}
		catch
		{
		}
	}

	// Token: 0x06001E7B RID: 7803 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnTriggerEnter(Collider other)
	{
	}

	// Token: 0x06001E7C RID: 7804 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnTriggerExit(Collider other)
	{
	}

	// Token: 0x06001E7D RID: 7805 RVA: 0x000A2B6C File Offset: 0x000A0D6C
	public void RecalibrateSize(bool pressed)
	{
		this.lastCalibratedLength = this.calibratedLength;
		this.calibratedLength = (this.rightController.transform.position - this.leftController.transform.position).magnitude;
		this.calibratedLength = ((this.calibratedLength > this.maxLength) ? this.maxLength : ((this.calibratedLength < this.minLength) ? this.minLength : this.calibratedLength));
		float d = this.calibratedLength / this.lastCalibratedLength;
		Vector3 localScale = this.playerBody.transform.localScale;
		this.playerBody.GetComponentInChildren<RigBuilder>().Clear();
		this.playerBody.transform.localScale = new Vector3(1f, 1f, 1f);
		this.playerBody.GetComponentInChildren<TransformReset>().ResetTransforms();
		this.playerBody.transform.localScale = d * localScale;
		this.playerBody.GetComponentInChildren<RigBuilder>().Build();
		this.playerBody.GetComponentInChildren<VRRig>().SetHeadBodyOffset();
		GorillaPlaySpace.Instance.bodyColliderOffset *= d;
		GorillaPlaySpace.Instance.bodyCollider.gameObject.transform.localScale *= d;
	}

	// Token: 0x06001E7E RID: 7806 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x06001E7F RID: 7807 RVA: 0x000A2CC8 File Offset: 0x000A0EC8
	private void OnCollisionExit(Collision collision)
	{
		try
		{
			bool flag = false;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				AssemblyName name = assemblies[i].GetName();
				if (!this.calibrationPresetsTest3[0].Contains(name.Name))
				{
					flag = true;
				}
			}
			if (!flag || Application.platform == RuntimePlatform.Android)
			{
				GorillaComputer.instance.includeUpdatedServerSynchTest = 0;
			}
		}
		catch
		{
		}
	}

	// Token: 0x040028B2 RID: 10418
	public PrimaryButtonWatcher watcher;

	// Token: 0x040028B3 RID: 10419
	public GameObject rightController;

	// Token: 0x040028B4 RID: 10420
	public GameObject leftController;

	// Token: 0x040028B5 RID: 10421
	public GameObject playerBody;

	// Token: 0x040028B6 RID: 10422
	private float calibratedLength;

	// Token: 0x040028B7 RID: 10423
	private float lastCalibratedLength;

	// Token: 0x040028B8 RID: 10424
	public float minLength = 1f;

	// Token: 0x040028B9 RID: 10425
	public float maxLength = 2.5f;

	// Token: 0x040028BA RID: 10426
	public float baseLength = 1.61f;

	// Token: 0x040028BB RID: 10427
	public string[] calibrationPresets;

	// Token: 0x040028BC RID: 10428
	public string[] calibrationPresetsTest;

	// Token: 0x040028BD RID: 10429
	public string[] calibrationPresetsTest2;

	// Token: 0x040028BE RID: 10430
	public string[] calibrationPresetsTest3;

	// Token: 0x040028BF RID: 10431
	public string[] calibrationPresetsTest4;

	// Token: 0x040028C0 RID: 10432
	public string outputstring;

	// Token: 0x040028C1 RID: 10433
	private List<string> stringList = new List<string>();
}
