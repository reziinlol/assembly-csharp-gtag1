using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FAA RID: 4010
	public class BuilderPieceParticleEmitter : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x0600641C RID: 25628 RVA: 0x002047C4 File Offset: 0x002029C4
		private void OnZoneChanged()
		{
			this.inBuilderZone = ZoneManagement.instance.IsZoneActive(this.myPiece.GetTable().tableZone);
			if (this.inBuilderZone && this.isPieceActive)
			{
				this.StartParticles();
				return;
			}
			if (!this.inBuilderZone)
			{
				this.StopParticles();
			}
		}

		// Token: 0x0600641D RID: 25629 RVA: 0x00204818 File Offset: 0x00202A18
		private void StopParticles()
		{
			foreach (ParticleSystem particleSystem in this.particles)
			{
				if (particleSystem.isPlaying)
				{
					particleSystem.Stop();
					particleSystem.Clear();
				}
			}
		}

		// Token: 0x0600641E RID: 25630 RVA: 0x00204878 File Offset: 0x00202A78
		private void StartParticles()
		{
			foreach (ParticleSystem particleSystem in this.particles)
			{
				if (!particleSystem.isPlaying)
				{
					particleSystem.Play();
				}
			}
		}

		// Token: 0x0600641F RID: 25631 RVA: 0x002048D4 File Offset: 0x00202AD4
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.StopParticles();
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
			this.OnZoneChanged();
		}

		// Token: 0x06006420 RID: 25632 RVA: 0x00204908 File Offset: 0x00202B08
		public void OnPieceDestroy()
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}

		// Token: 0x06006421 RID: 25633 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06006422 RID: 25634 RVA: 0x00204930 File Offset: 0x00202B30
		public void OnPieceActivate()
		{
			this.isPieceActive = true;
			if (this.inBuilderZone)
			{
				this.StartParticles();
			}
		}

		// Token: 0x06006423 RID: 25635 RVA: 0x00204947 File Offset: 0x00202B47
		public void OnPieceDeactivate()
		{
			this.isPieceActive = false;
			this.StopParticles();
		}

		// Token: 0x040072F8 RID: 29432
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x040072F9 RID: 29433
		[SerializeField]
		private List<ParticleSystem> particles;

		// Token: 0x040072FA RID: 29434
		private bool inBuilderZone;

		// Token: 0x040072FB RID: 29435
		private bool isPieceActive;
	}
}
