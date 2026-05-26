using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000072 RID: 114
public class CritterSpawnTrigger : MonoBehaviour
{
	// Token: 0x060002CD RID: 717 RVA: 0x00011080 File Offset: 0x0000F280
	private ValueDropdownList<int> GetCritterTypeList()
	{
		return new ValueDropdownList<int>();
	}

	// Token: 0x060002CE RID: 718 RVA: 0x00011088 File Offset: 0x0000F288
	private void OnTriggerEnter(Collider other)
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (Time.realtimeSinceStartup < this._nextSpawnTime)
		{
			return;
		}
		CrittersActor componentInParent = other.GetComponentInParent<CrittersActor>();
		if (!componentInParent)
		{
			return;
		}
		if (componentInParent.crittersActorType != this.triggerActorType)
		{
			return;
		}
		if (this.requiredSubObjectIndex >= 0 && componentInParent.subObjectIndex != this.requiredSubObjectIndex)
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.triggerActorName) && !componentInParent.GetActorSubtype().Contains(this.triggerActorName))
		{
			return;
		}
		CrittersManager.instance.DespawnActor(componentInParent);
		CrittersManager.instance.SpawnCritter(this.critterType, this.spawnPoint.position, this.spawnPoint.rotation);
		this._nextSpawnTime = Time.realtimeSinceStartup + this.triggerCooldown;
	}

	// Token: 0x060002CF RID: 719 RVA: 0x00011152 File Offset: 0x0000F352
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(base.transform.position, this.spawnPoint.position);
		Gizmos.DrawWireSphere(this.spawnPoint.position, 0.1f);
	}

	// Token: 0x0400033C RID: 828
	[Header("Trigger Settings")]
	[SerializeField]
	private CrittersActor.CrittersActorType triggerActorType;

	// Token: 0x0400033D RID: 829
	[SerializeField]
	private int requiredSubObjectIndex = -1;

	// Token: 0x0400033E RID: 830
	[SerializeField]
	private string triggerActorName;

	// Token: 0x0400033F RID: 831
	[SerializeField]
	private float triggerCooldown = 1f;

	// Token: 0x04000340 RID: 832
	[Header("Spawn Settings")]
	[SerializeField]
	private Transform spawnPoint;

	// Token: 0x04000341 RID: 833
	[SerializeField]
	private int critterType;

	// Token: 0x04000342 RID: 834
	private float _nextSpawnTime;
}
