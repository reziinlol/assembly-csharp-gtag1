using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000EF8 RID: 3832
	[NetworkBehaviourWeaved(1)]
	public class ChristmasTree : NetworkComponent
	{
		// Token: 0x06005F37 RID: 24375 RVA: 0x001EA79C File Offset: 0x001E899C
		protected override void Awake()
		{
			base.Awake();
			foreach (AttachPoint attachPoint in this.hangers.GetComponentsInChildren<AttachPoint>())
			{
				this.attachPointsList.Add(attachPoint);
				AttachPoint attachPoint2 = attachPoint;
				attachPoint2.onHookedChanged = (UnityAction)Delegate.Combine(attachPoint2.onHookedChanged, new UnityAction(this.UpdateHangers));
			}
			this.lightRenderers = this.lights.GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = this.lightRenderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].material = this.lightsOffMaterial;
			}
			this.wasActive = false;
			this.isActive = false;
		}

		// Token: 0x06005F38 RID: 24376 RVA: 0x001EA83D File Offset: 0x001E8A3D
		private void Update()
		{
			if (this.spinTheTop && this.topOrnament)
			{
				this.topOrnament.transform.Rotate(0f, this.spinSpeed * Time.deltaTime, 0f, Space.World);
			}
		}

		// Token: 0x06005F39 RID: 24377 RVA: 0x001EA87C File Offset: 0x001E8A7C
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (AttachPoint attachPoint in this.attachPointsList)
			{
				attachPoint.onHookedChanged = (UnityAction)Delegate.Remove(attachPoint.onHookedChanged, new UnityAction(this.UpdateHangers));
			}
			this.attachPointsList.Clear();
		}

		// Token: 0x06005F3A RID: 24378 RVA: 0x001EA8FC File Offset: 0x001E8AFC
		private void UpdateHangers()
		{
			if (this.attachPointsList.Count == 0)
			{
				return;
			}
			using (List<AttachPoint>.Enumerator enumerator = this.attachPointsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsHooked())
					{
						if (base.IsMine)
						{
							this.updateLight(true);
						}
						return;
					}
				}
			}
			if (base.IsMine)
			{
				this.updateLight(false);
			}
		}

		// Token: 0x06005F3B RID: 24379 RVA: 0x001EA97C File Offset: 0x001E8B7C
		private void updateLight(bool enable)
		{
			this.isActive = enable;
			for (int i = 0; i < this.lightRenderers.Length; i++)
			{
				this.lightRenderers[i].material = (enable ? this.lightsOnMaterials[i % this.lightsOnMaterials.Length] : this.lightsOffMaterial);
			}
			this.spinTheTop = enable;
		}

		// Token: 0x17000908 RID: 2312
		// (get) Token: 0x06005F3C RID: 24380 RVA: 0x001EA9D3 File Offset: 0x001E8BD3
		// (set) Token: 0x06005F3D RID: 24381 RVA: 0x001EA9FD File Offset: 0x001E8BFD
		[Networked]
		[NetworkedWeaved(0, 1)]
		private unsafe NetworkBool Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ChristmasTree.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(NetworkBool*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ChristmasTree.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(NetworkBool*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06005F3E RID: 24382 RVA: 0x001EAA28 File Offset: 0x001E8C28
		public override void WriteDataFusion()
		{
			this.Data = this.isActive;
		}

		// Token: 0x06005F3F RID: 24383 RVA: 0x001EAA3B File Offset: 0x001E8C3B
		public override void ReadDataFusion()
		{
			this.wasActive = this.isActive;
			this.isActive = this.Data;
			if (this.wasActive != this.isActive)
			{
				this.updateLight(this.isActive);
			}
		}

		// Token: 0x06005F40 RID: 24384 RVA: 0x001EAA74 File Offset: 0x001E8C74
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			stream.SendNext(this.isActive);
		}

		// Token: 0x06005F41 RID: 24385 RVA: 0x001EAA98 File Offset: 0x001E8C98
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			this.wasActive = this.isActive;
			this.isActive = (bool)stream.ReceiveNext();
			if (this.wasActive != this.isActive)
			{
				this.updateLight(this.isActive);
			}
		}

		// Token: 0x06005F43 RID: 24387 RVA: 0x001EAB08 File Offset: 0x001E8D08
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06005F44 RID: 24388 RVA: 0x001EAB20 File Offset: 0x001E8D20
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x04006DF8 RID: 28152
		public GameObject hangers;

		// Token: 0x04006DF9 RID: 28153
		public GameObject lights;

		// Token: 0x04006DFA RID: 28154
		public GameObject topOrnament;

		// Token: 0x04006DFB RID: 28155
		public float spinSpeed = 60f;

		// Token: 0x04006DFC RID: 28156
		private readonly List<AttachPoint> attachPointsList = new List<AttachPoint>();

		// Token: 0x04006DFD RID: 28157
		private MeshRenderer[] lightRenderers;

		// Token: 0x04006DFE RID: 28158
		private bool wasActive;

		// Token: 0x04006DFF RID: 28159
		private bool isActive;

		// Token: 0x04006E00 RID: 28160
		private bool spinTheTop;

		// Token: 0x04006E01 RID: 28161
		[SerializeField]
		private Material lightsOffMaterial;

		// Token: 0x04006E02 RID: 28162
		[SerializeField]
		private Material[] lightsOnMaterials;

		// Token: 0x04006E03 RID: 28163
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private NetworkBool _Data;
	}
}
