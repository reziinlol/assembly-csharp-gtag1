using System;
using PerformanceSystems;
using UnityEngine;

// Token: 0x02000D1C RID: 3356
public class DemoSpheresRotate : TimeSliceLodBehaviour
{
	// Token: 0x060052DF RID: 21215 RVA: 0x001B2B11 File Offset: 0x001B0D11
	public void OnLod0Enter()
	{
		this._renderer.material = this._red;
		this.SwapToTimeSlicer(0);
		base.gameObject.SetActive(true);
	}

	// Token: 0x060052E0 RID: 21216 RVA: 0x001B2B37 File Offset: 0x001B0D37
	public void OnLod1Enter()
	{
		this._renderer.material = this._green;
		this.SwapToTimeSlicer(1);
		base.gameObject.SetActive(true);
	}

	// Token: 0x060052E1 RID: 21217 RVA: 0x001B2B5D File Offset: 0x001B0D5D
	public void OnLod2Enter()
	{
		this._renderer.material = this._black;
		this.SwapToTimeSlicer(2);
		base.gameObject.SetActive(true);
	}

	// Token: 0x060052E2 RID: 21218 RVA: 0x000440BC File Offset: 0x000422BC
	public void OnLodExit()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x060052E3 RID: 21219 RVA: 0x001B2B83 File Offset: 0x001B0D83
	public override void SliceUpdate(float deltaTime)
	{
		base.transform.Rotate(Vector3.up * this._rotationSpeed * deltaTime);
	}

	// Token: 0x060052E4 RID: 21220 RVA: 0x001B2BA6 File Offset: 0x001B0DA6
	private void SwapToTimeSlicer(int index)
	{
		if (this._timeSliceControllerAssets[index] == this._timeSliceControllerAsset)
		{
			return;
		}
		this._timeSliceControllerAsset.RemoveTimeSliceBehaviour(this);
		this._timeSliceControllerAsset = this._timeSliceControllerAssets[index];
		this._timeSliceControllerAsset.AddTimeSliceBehaviour(this);
	}

	// Token: 0x04006448 RID: 25672
	[SerializeField]
	private TimeSliceControllerAsset[] _timeSliceControllerAssets;

	// Token: 0x04006449 RID: 25673
	[SerializeField]
	private float _rotationSpeed = 10f;

	// Token: 0x0400644A RID: 25674
	[SerializeField]
	private Material _red;

	// Token: 0x0400644B RID: 25675
	[SerializeField]
	private Material _green;

	// Token: 0x0400644C RID: 25676
	[SerializeField]
	private Material _black;

	// Token: 0x0400644D RID: 25677
	[SerializeField]
	private Renderer _renderer;
}
