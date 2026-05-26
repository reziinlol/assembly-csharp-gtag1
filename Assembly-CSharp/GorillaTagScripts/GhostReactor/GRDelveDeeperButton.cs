using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor
{
	// Token: 0x02000F87 RID: 3975
	[RequireComponent(typeof(GorillaPressableButton))]
	public sealed class GRDelveDeeperButton : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06006333 RID: 25395 RVA: 0x001FEAAB File Offset: 0x001FCCAB
		private void Awake()
		{
			this.CountMonkes();
		}

		// Token: 0x06006334 RID: 25396 RVA: 0x001FEAB4 File Offset: 0x001FCCB4
		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawCube(this._drillCollider.bounds.center, this._drillCollider.bounds.size);
		}

		// Token: 0x06006335 RID: 25397 RVA: 0x001FEAEC File Offset: 0x001FCCEC
		private void CountMonkes()
		{
			int num = Physics.OverlapBoxNonAlloc(this._drillCollider.bounds.center, this._drillCollider.bounds.extents, this._overlapBoxResults, this._drillCollider.transform.rotation, 2048);
			this._numGorillasInDrill = 0;
			for (int i = 0; i < num; i++)
			{
				if (this._overlapBoxResults[i].GetComponent<VRRig>() != null && this._drillCollider.bounds.Contains(this._overlapBoxResults[i].transform.position))
				{
					this._numGorillasInDrill++;
				}
			}
		}

		// Token: 0x06006336 RID: 25398 RVA: 0x001FEB9D File Offset: 0x001FCD9D
		private void OnEnable()
		{
			if (this._shiftManager == null)
			{
				throw new Exception("_shiftManager unset for GREndShiftButton.");
			}
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			this._button = base.GetComponent<GorillaPressableButton>();
			this.UpdateButton();
		}

		// Token: 0x06006337 RID: 25399 RVA: 0x00011DE0 File Offset: 0x0000FFE0
		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x06006338 RID: 25400 RVA: 0x001FEBD1 File Offset: 0x001FCDD1
		public void SliceUpdate()
		{
			this.CountMonkes();
			this.UpdateButton();
		}

		// Token: 0x06006339 RID: 25401 RVA: 0x001FEBE0 File Offset: 0x001FCDE0
		private void UpdateButton()
		{
			if (this._shiftManager.authorizedToDelveDeeper && this._numGorillasInDrill == this._shiftManager.reactor.NumActivePlayers)
			{
				this._button.enabled = true;
				this._text.text = "DELVE\nNOW";
				return;
			}
			this._button.enabled = false;
			this._text.text = "DISABLED";
		}

		// Token: 0x0600633A RID: 25402 RVA: 0x001FEC50 File Offset: 0x001FCE50
		public void DelveDeeper()
		{
			this._shiftManager.EndShift();
		}

		// Token: 0x040071CE RID: 29134
		[SerializeField]
		private BoxCollider _drillCollider;

		// Token: 0x040071CF RID: 29135
		[SerializeField]
		private GhostReactorShiftManager _shiftManager;

		// Token: 0x040071D0 RID: 29136
		[SerializeField]
		private TextMeshPro _text;

		// Token: 0x040071D1 RID: 29137
		private GorillaPressableButton _button;

		// Token: 0x040071D2 RID: 29138
		private int _numGorillasInDrill;

		// Token: 0x040071D3 RID: 29139
		private readonly Collider[] _overlapBoxResults = new Collider[200];
	}
}
