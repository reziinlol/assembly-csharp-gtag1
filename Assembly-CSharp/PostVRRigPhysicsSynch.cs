using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000C91 RID: 3217
[DefaultExecutionOrder(9999)]
public class PostVRRigPhysicsSynch : MonoBehaviour
{
	// Token: 0x06004FC2 RID: 20418 RVA: 0x001A62F0 File Offset: 0x001A44F0
	private void LateUpdate()
	{
		for (int i = 0; i < PostVRRigPhysicsSynch.k_syncList.Count; i++)
		{
			AutoSyncTransforms autoSyncTransforms = PostVRRigPhysicsSynch.k_syncList[i];
			Transform targetTransform = autoSyncTransforms.TargetTransform;
			Rigidbody targetRigidbody = autoSyncTransforms.TargetRigidbody;
			Vector3 position = targetTransform.position;
			Quaternion rotation = targetTransform.rotation;
			targetRigidbody.position = position;
			targetRigidbody.rotation = rotation;
		}
	}

	// Token: 0x06004FC3 RID: 20419 RVA: 0x001A6344 File Offset: 0x001A4544
	public static void AddSyncTarget(AutoSyncTransforms body)
	{
		PostVRRigPhysicsSynch.k_syncList.Add(body);
	}

	// Token: 0x06004FC4 RID: 20420 RVA: 0x001A6351 File Offset: 0x001A4551
	public static void RemoveSyncTarget(AutoSyncTransforms body)
	{
		PostVRRigPhysicsSynch.k_syncList.Remove(body);
	}

	// Token: 0x04006190 RID: 24976
	private static readonly List<AutoSyncTransforms> k_syncList = new List<AutoSyncTransforms>(5);
}
