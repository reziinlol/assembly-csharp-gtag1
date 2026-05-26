using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000EDB RID: 3803
	public class BuilderRecycler : MonoBehaviour
	{
		// Token: 0x06005DC7 RID: 24007 RVA: 0x001DBDA0 File Offset: 0x001D9FA0
		private void Awake()
		{
			this.hasFans = (this.effectBehaviors.Count > 0 && this.bladeSoundPlayer != null && this.recycleParticles != null);
			this.hasPipes = (this.outputPipes.Count > 0);
		}

		// Token: 0x06005DC8 RID: 24008 RVA: 0x001DBDF4 File Offset: 0x001D9FF4
		private void Start()
		{
			if (this.hasPipes)
			{
				this.numPipes = Mathf.Min(this.outputPipes.Count, 3);
				this.props = new MaterialPropertyBlock();
				this.ResetOutputPipes();
				this.totalRecycledCost = new int[3];
				this.currentChainCost = new int[3];
				for (int i = 0; i < this.totalRecycledCost.Length; i++)
				{
					this.totalRecycledCost[i] = 0;
					this.currentChainCost[i] = 0;
				}
			}
			this.zoneRenderers.Clear();
			if (this.hasPipes)
			{
				this.zoneRenderers.AddRange(this.outputPipes);
			}
			if (this.hasFans)
			{
				foreach (MonoBehaviour monoBehaviour in this.effectBehaviors)
				{
					Renderer component = monoBehaviour.GetComponent<Renderer>();
					if (component != null)
					{
						this.zoneRenderers.Add(component);
					}
				}
			}
			this.inBuilderZone = true;
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
			this.OnZoneChanged();
		}

		// Token: 0x06005DC9 RID: 24009 RVA: 0x001DBF28 File Offset: 0x001DA128
		private void OnDestroy()
		{
			if (ZoneManagement.instance != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
			}
		}

		// Token: 0x06005DCA RID: 24010 RVA: 0x001DBF60 File Offset: 0x001DA160
		private void OnZoneChanged()
		{
			bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
			if (flag && !this.inBuilderZone)
			{
				using (List<Renderer>.Enumerator enumerator = this.zoneRenderers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Renderer renderer = enumerator.Current;
						renderer.enabled = true;
					}
					goto IL_8B;
				}
			}
			if (!flag && this.inBuilderZone)
			{
				foreach (Renderer renderer2 in this.zoneRenderers)
				{
					renderer2.enabled = false;
				}
			}
			IL_8B:
			this.inBuilderZone = flag;
		}

		// Token: 0x06005DCB RID: 24011 RVA: 0x001DC01C File Offset: 0x001DA21C
		private void OnTriggerEnter(Collider other)
		{
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(other);
			if (builderPieceFromCollider == null)
			{
				return;
			}
			if (!builderPieceFromCollider.isBuiltIntoTable && !builderPieceFromCollider.isArmShelf)
			{
				this.table.RequestRecyclePiece(builderPieceFromCollider, true, this.recyclerID);
			}
		}

		// Token: 0x06005DCC RID: 24012 RVA: 0x001DC060 File Offset: 0x001DA260
		public void OnRecycleRequestedAtRecycler(BuilderPiece piece)
		{
			if (this.hasPipes)
			{
				this.AddPieceCost(piece.cost);
			}
			if (this.hasFans)
			{
				foreach (MonoBehaviour monoBehaviour in this.effectBehaviors)
				{
					monoBehaviour.enabled = true;
				}
				this.recycleParticles.SetActive(true);
				this.bladeSoundPlayer.Play();
				this.timeToStopBlades = (double)(Time.time + this.recycleEffectDuration);
				this.playingBladeEffect = true;
			}
		}

		// Token: 0x06005DCD RID: 24013 RVA: 0x001DC100 File Offset: 0x001DA300
		private void AddPieceCost(BuilderResources cost)
		{
			foreach (BuilderResourceQuantity builderResourceQuantity in cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					this.totalRecycledCost[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
				}
			}
			if (!this.playingPipeEffect)
			{
				this.UpdatePipeLoop();
			}
		}

		// Token: 0x06005DCE RID: 24014 RVA: 0x001DC188 File Offset: 0x001DA388
		private Vector2 GetUVShiftOffset()
		{
			float y = Shader.GetGlobalVector(ShaderProps._Time).y;
			Vector4 vector = new Vector4(500f, 0f, 0f, 0f);
			Vector4 vector2 = vector / this.recycleEffectDuration;
			return new Vector2(-1f * (Mathf.Floor(y * vector2.x) * 1f / vector.x % 1f) * vector.x - vector.x + 165f, 0f);
		}

		// Token: 0x06005DCF RID: 24015 RVA: 0x001DC214 File Offset: 0x001DA414
		private void UpdatePipeLoop()
		{
			bool flag = false;
			for (int i = 0; i < this.numPipes; i++)
			{
				if (this.totalRecycledCost[i] > 0)
				{
					flag = true;
					this.outputPipes[i].GetPropertyBlock(this.props, 1);
					Vector4 value = new Vector4(500f, 0f, 0f, 0f) / this.recycleEffectDuration;
					Vector2 uvshiftOffset = this.GetUVShiftOffset();
					this.props.SetColor(ShaderProps._BaseColor, this.builderResourceColors.colors[i].color);
					this.props.SetVector(ShaderProps._UvShiftRate, value);
					this.props.SetVector(ShaderProps._UvShiftOffset, uvshiftOffset);
					this.outputPipes[i].SetPropertyBlock(this.props, 1);
					this.totalRecycledCost[i] = Mathf.Max(this.totalRecycledCost[i] - 1, 0);
				}
				else
				{
					this.outputPipes[i].GetPropertyBlock(this.props, 1);
					this.props.SetColor(ShaderProps._BaseColor, Color.black);
					this.outputPipes[i].SetPropertyBlock(this.props, 1);
				}
			}
			if (flag)
			{
				this.playingPipeEffect = true;
				this.timeToCheckPipes = (double)(Time.time + this.recycleEffectDuration);
				return;
			}
			this.playingPipeEffect = false;
		}

		// Token: 0x06005DD0 RID: 24016 RVA: 0x001DC378 File Offset: 0x001DA578
		private void ResetOutputPipes()
		{
			foreach (MeshRenderer meshRenderer in this.outputPipes)
			{
				meshRenderer.GetPropertyBlock(this.props, 1);
				this.props.SetColor(ShaderProps._BaseColor, Color.black);
				meshRenderer.SetPropertyBlock(this.props, 1);
			}
		}

		// Token: 0x06005DD1 RID: 24017 RVA: 0x001DC3F4 File Offset: 0x001DA5F4
		public void UpdateRecycler()
		{
			if (this.playingBladeEffect && (double)Time.time > this.timeToStopBlades)
			{
				if (this.hasFans)
				{
					foreach (MonoBehaviour monoBehaviour in this.effectBehaviors)
					{
						monoBehaviour.enabled = false;
					}
					this.recycleParticles.SetActive(false);
				}
				this.playingBladeEffect = false;
			}
			if (this.playingPipeEffect && (double)Time.time > this.timeToCheckPipes)
			{
				this.UpdatePipeLoop();
			}
		}

		// Token: 0x04006C56 RID: 27734
		public float recycleEffectDuration = 0.25f;

		// Token: 0x04006C57 RID: 27735
		private double timeToStopBlades = double.MinValue;

		// Token: 0x04006C58 RID: 27736
		private bool playingBladeEffect;

		// Token: 0x04006C59 RID: 27737
		private bool playingPipeEffect;

		// Token: 0x04006C5A RID: 27738
		private double timeToCheckPipes = double.MinValue;

		// Token: 0x04006C5B RID: 27739
		public List<MonoBehaviour> effectBehaviors;

		// Token: 0x04006C5C RID: 27740
		public GameObject recycleParticles;

		// Token: 0x04006C5D RID: 27741
		public SoundBankPlayer bladeSoundPlayer;

		// Token: 0x04006C5E RID: 27742
		public List<MeshRenderer> outputPipes;

		// Token: 0x04006C5F RID: 27743
		public BuilderResourceColors builderResourceColors;

		// Token: 0x04006C60 RID: 27744
		private bool hasFans;

		// Token: 0x04006C61 RID: 27745
		private bool hasPipes;

		// Token: 0x04006C62 RID: 27746
		private MaterialPropertyBlock props;

		// Token: 0x04006C63 RID: 27747
		private int[] totalRecycledCost;

		// Token: 0x04006C64 RID: 27748
		private int[] currentChainCost;

		// Token: 0x04006C65 RID: 27749
		private int numPipes;

		// Token: 0x04006C66 RID: 27750
		internal int recyclerID = -1;

		// Token: 0x04006C67 RID: 27751
		internal BuilderTable table;

		// Token: 0x04006C68 RID: 27752
		private List<Renderer> zoneRenderers = new List<Renderer>(10);

		// Token: 0x04006C69 RID: 27753
		private bool inBuilderZone;
	}
}
