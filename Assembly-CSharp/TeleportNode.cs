using System;
using System.Collections;
using GorillaLocomotion;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x02000DC6 RID: 3526
public class TeleportNode : GorillaTriggerBox
{
	// Token: 0x0600566B RID: 22123 RVA: 0x001C0960 File Offset: 0x001BEB60
	public override void OnBoxTriggered()
	{
		if (this.subsOnly && !SubscriptionManager.IsLocalSubscribed())
		{
			return;
		}
		if (Time.time - this.teleportTime < 0.1f)
		{
			return;
		}
		base.OnBoxTriggered();
		Transform transform;
		if (!this.teleportFromRef.TryResolve<Transform>(out transform))
		{
			Debug.LogError("[TeleportNode] Failed to resolve teleportFromRef.");
			return;
		}
		Transform transform2;
		if (!this.teleportToRef.TryResolve<Transform>(out transform2))
		{
			Debug.LogError("[TeleportNode] Failed to resolve teleportToRef.");
			return;
		}
		GTPlayer instance = GTPlayer.Instance;
		if (instance == null)
		{
			Debug.LogError("[TeleportNode] GTPlayer.Instance is null.");
			return;
		}
		Physics.SyncTransforms();
		Vector3 position = transform2.transform.position;
		if (this.seamless)
		{
			position = transform2.TransformPoint(transform.InverseTransformPoint(instance.transform.position));
		}
		Quaternion rhs = Quaternion.Inverse(transform.rotation) * instance.transform.rotation;
		Quaternion rotation = transform2.rotation * rhs;
		base.StartCoroutine(this.DelayedTeleport(instance, position, rotation));
		this.teleportTime = Time.time;
	}

	// Token: 0x0600566C RID: 22124 RVA: 0x001C0A5E File Offset: 0x001BEC5E
	private IEnumerator DelayedTeleport(GTPlayer p, Vector3 position, Quaternion rotation)
	{
		yield return null;
		p.TeleportTo(position, rotation, this.keepVelocity, !this.seamless);
		yield break;
	}

	// Token: 0x0400664E RID: 26190
	[SerializeField]
	private XSceneRef teleportFromRef;

	// Token: 0x0400664F RID: 26191
	[SerializeField]
	private XSceneRef teleportToRef;

	// Token: 0x04006650 RID: 26192
	[SerializeField]
	private bool seamless = true;

	// Token: 0x04006651 RID: 26193
	[SerializeField]
	private bool keepVelocity = true;

	// Token: 0x04006652 RID: 26194
	[SerializeField]
	private bool subsOnly;

	// Token: 0x04006653 RID: 26195
	private float teleportTime;
}
