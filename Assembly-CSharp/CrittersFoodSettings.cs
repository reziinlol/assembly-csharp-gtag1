using System;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class CrittersFoodSettings : CrittersActorSettings
{
	// Token: 0x060001E3 RID: 483 RVA: 0x0000B348 File Offset: 0x00009548
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersFood crittersFood = (CrittersFood)this.parentActor;
		crittersFood.maxFood = this._maxFood;
		crittersFood.currentFood = this._currentFood;
		crittersFood.startingSize = this._startingSize;
		crittersFood.currentSize = this._currentSize;
		crittersFood.food = this._food;
		crittersFood.disableWhenEmpty = this._disableWhenEmpty;
		crittersFood.SpawnData(this._maxFood, this._currentFood, this._startingSize);
	}

	// Token: 0x04000221 RID: 545
	public float _maxFood;

	// Token: 0x04000222 RID: 546
	public float _currentFood;

	// Token: 0x04000223 RID: 547
	public float _startingSize;

	// Token: 0x04000224 RID: 548
	public float _currentSize;

	// Token: 0x04000225 RID: 549
	public Transform _food;

	// Token: 0x04000226 RID: 550
	public bool _disableWhenEmpty;
}
