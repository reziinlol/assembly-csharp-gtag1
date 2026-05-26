using System;
using UnityEngine;

// Token: 0x02000111 RID: 273
public class SIGadgetPlatformDeployerPlatform : MonoBehaviour, ISIGameDeployable
{
	// Token: 0x06000683 RID: 1667 RVA: 0x00024390 File Offset: 0x00022590
	public void ApplyUpgrades(SIUpgradeSet upgrades)
	{
		bool flag = upgrades.Contains(SIUpgradeType.Platform_Duration);
		float num = flag ? this.extendedDuration : this.defaultDuration;
		this.timeToDie = Time.time + num;
		this.extendedDurationFrame.SetActive(flag);
		this.checkBounds = new Bounds(this.activeCollider.center, this.activeCollider.size);
		Vector3 size = this.checkBounds.size;
		Vector3 lossyScale = this.activeCollider.transform.lossyScale;
		size.x *= lossyScale.x;
		size.y *= lossyScale.y;
		size.z *= lossyScale.z;
		this.checkBounds.size = size;
		this.checkOffset = this.activeCollider.transform.position;
		this.checkRot = this.activeCollider.transform.rotation;
		this.CheckHeadOverlap();
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x00024484 File Offset: 0x00022684
	public void CheckHeadOverlap()
	{
		if (this.activeCollider == null)
		{
			return;
		}
		Vector3 position = GorillaTagger.Instance.headCollider.transform.position;
		float num = GorillaTagger.Instance.headCollider.radius * GorillaTagger.Instance.headCollider.transform.lossyScale.x;
		Vector3 vector = Quaternion.Inverse(this.checkRot) * (position - this.checkOffset);
		if (Vector3.Magnitude(this.checkBounds.ClosestPoint(vector) - vector) < num)
		{
			this.isOverlappingHead = true;
			this.activeCollider.enabled = false;
			return;
		}
		this.isOverlappingHead = false;
		this.activeCollider.enabled = true;
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x0002453D File Offset: 0x0002273D
	private void LateUpdate()
	{
		if (Time.time > this.timeToDie)
		{
			ObjectPools.instance.Destroy(base.gameObject);
			return;
		}
		if (this.isOverlappingHead)
		{
			this.CheckHeadOverlap();
		}
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x0002456B File Offset: 0x0002276B
	private void OnDisable()
	{
		Action onDisabled = this.OnDisabled;
		if (onDisabled != null)
		{
			onDisabled();
		}
		this.OnDisabled = null;
	}

	// Token: 0x040007EF RID: 2031
	[SerializeField]
	private GameObject extendedDurationFrame;

	// Token: 0x040007F0 RID: 2032
	[SerializeField]
	private float defaultDuration = 10f;

	// Token: 0x040007F1 RID: 2033
	[SerializeField]
	private float extendedDuration = 20f;

	// Token: 0x040007F2 RID: 2034
	[SerializeField]
	private BoxCollider activeCollider;

	// Token: 0x040007F3 RID: 2035
	private bool isOverlappingHead;

	// Token: 0x040007F4 RID: 2036
	private float timeToDie = -1f;

	// Token: 0x040007F5 RID: 2037
	private Bounds checkBounds;

	// Token: 0x040007F6 RID: 2038
	private Vector3 checkOffset;

	// Token: 0x040007F7 RID: 2039
	private Quaternion checkRot;

	// Token: 0x040007F8 RID: 2040
	public Action OnDisabled;
}
