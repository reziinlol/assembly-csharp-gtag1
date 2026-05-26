using System;
using PerformanceSystems;
using UnityEngine;

// Token: 0x02000D1B RID: 3355
public class DemoCubeATimeSliceBehaviourEvents : TimeSliceLodBehaviour
{
	// Token: 0x060052D9 RID: 21209 RVA: 0x001B2A7C File Offset: 0x001B0C7C
	protected new void Awake()
	{
		base.Awake();
		this._renderer = base.GetComponent<Renderer>();
	}

	// Token: 0x060052DA RID: 21210 RVA: 0x001B2A90 File Offset: 0x001B0C90
	public override void SliceUpdate(float deltaTime)
	{
		float num = 0f;
		for (int i = 0; i < this._iterationsOfExpensiveOp; i++)
		{
			num += Mathf.Sqrt((float)i * deltaTime);
		}
	}

	// Token: 0x060052DB RID: 21211 RVA: 0x001B2AC0 File Offset: 0x001B0CC0
	public void OnLod0Enter()
	{
		this._renderer.material = this._red;
		base.gameObject.SetActive(true);
	}

	// Token: 0x060052DC RID: 21212 RVA: 0x001B2ADF File Offset: 0x001B0CDF
	public void OnLod1Enter()
	{
		this._renderer.material = this._green;
		base.gameObject.SetActive(true);
	}

	// Token: 0x060052DD RID: 21213 RVA: 0x000440BC File Offset: 0x000422BC
	public void OnLodExit()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04006444 RID: 25668
	[SerializeField]
	private int _iterationsOfExpensiveOp = 200;

	// Token: 0x04006445 RID: 25669
	[SerializeField]
	private Material _red;

	// Token: 0x04006446 RID: 25670
	[SerializeField]
	private Material _green;

	// Token: 0x04006447 RID: 25671
	private Renderer _renderer;
}
