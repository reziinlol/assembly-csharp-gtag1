using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020005D8 RID: 1496
public class GravityOverrideVolume : MonoBehaviour
{
	// Token: 0x0600254E RID: 9550 RVA: 0x000C615C File Offset: 0x000C435C
	private void OnEnable()
	{
		if (this.triggerEvents != null)
		{
			this.triggerEvents.CompositeTriggerEnter += this.OnColliderEnteredVolume;
			this.triggerEvents.CompositeTriggerExit += this.OnColliderExitedVolume;
		}
	}

	// Token: 0x0600254F RID: 9551 RVA: 0x000C619A File Offset: 0x000C439A
	private void OnDisable()
	{
		if (this.triggerEvents != null)
		{
			this.triggerEvents.CompositeTriggerEnter -= this.OnColliderEnteredVolume;
			this.triggerEvents.CompositeTriggerExit -= this.OnColliderExitedVolume;
		}
	}

	// Token: 0x06002550 RID: 9552 RVA: 0x000C61D8 File Offset: 0x000C43D8
	private void OnColliderEnteredVolume(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
		}
	}

	// Token: 0x06002551 RID: 9553 RVA: 0x000C6218 File Offset: 0x000C4418
	private void OnColliderExitedVolume(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			instance.UnsetGravityOverride(this);
		}
	}

	// Token: 0x06002552 RID: 9554 RVA: 0x000C624C File Offset: 0x000C444C
	public void GravityOverrideFunction(GTPlayer player)
	{
		GravityOverrideVolume.GravityType gravityType = this.gravityType;
		if (gravityType == GravityOverrideVolume.GravityType.Directional)
		{
			Vector3 forward = this.referenceTransform.forward;
			player.AddForce(forward * this.strength, ForceMode.Acceleration);
			return;
		}
		if (gravityType != GravityOverrideVolume.GravityType.Radial)
		{
			return;
		}
		Vector3 normalized = (this.referenceTransform.position - player.headCollider.transform.position).normalized;
		player.AddForce(normalized * this.strength, ForceMode.Acceleration);
	}

	// Token: 0x040030D3 RID: 12499
	[SerializeField]
	private GravityOverrideVolume.GravityType gravityType;

	// Token: 0x040030D4 RID: 12500
	[SerializeField]
	private float strength = 9.8f;

	// Token: 0x040030D5 RID: 12501
	[SerializeField]
	[Tooltip("In Radial: the center point of gravity, In Directional: the forward vector of this transform defines the direction")]
	private Transform referenceTransform;

	// Token: 0x040030D6 RID: 12502
	[SerializeField]
	private CompositeTriggerEvents triggerEvents;

	// Token: 0x020005D9 RID: 1497
	public enum GravityType
	{
		// Token: 0x040030D8 RID: 12504
		Directional,
		// Token: 0x040030D9 RID: 12505
		Radial
	}
}
