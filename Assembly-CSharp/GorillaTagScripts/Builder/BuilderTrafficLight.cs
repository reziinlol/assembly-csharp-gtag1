using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FBF RID: 4031
	public class BuilderTrafficLight : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x060064D0 RID: 25808 RVA: 0x002083A7 File Offset: 0x002065A7
		private void Start()
		{
			this.materialProps = new MaterialPropertyBlock();
		}

		// Token: 0x060064D1 RID: 25809 RVA: 0x002083B4 File Offset: 0x002065B4
		private void SetState(BuilderTrafficLight.LightState state)
		{
			this.lightState = state;
			if (this.materialProps == null)
			{
				this.materialProps = new MaterialPropertyBlock();
			}
			Color value = this.yellowOff;
			Color value2 = this.redOff;
			Color value3 = this.greenOff;
			switch (state)
			{
			case BuilderTrafficLight.LightState.Red:
				value2 = this.redOn;
				break;
			case BuilderTrafficLight.LightState.Yellow:
				value = this.yellowOn;
				break;
			case BuilderTrafficLight.LightState.Green:
				value3 = this.greenOn;
				break;
			}
			this.redLight.GetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, value2);
			this.redLight.SetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, value);
			this.yellowLight.SetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, value3);
			this.greenLight.SetPropertyBlock(this.materialProps);
		}

		// Token: 0x060064D2 RID: 25810 RVA: 0x00208494 File Offset: 0x00206694
		private void Update()
		{
			if (this.piece == null || this.piece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				float num = Time.time;
				if (PhotonNetwork.InRoom)
				{
					uint num2 = (uint)PhotonNetwork.ServerTimestamp;
					if (this.piece != null)
					{
						num2 = (uint)(PhotonNetwork.ServerTimestamp - this.piece.activatedTimeStamp);
					}
					num = num2 / 1000f;
				}
				float num3 = num % this.cycleDuration / this.cycleDuration;
				num3 = (num3 + this.startPercentageOffset) % 1f;
				int num4 = (int)this.stateCurve.Evaluate(num3);
				if (num4 != (int)this.lightState)
				{
					this.SetState((BuilderTrafficLight.LightState)num4);
				}
			}
		}

		// Token: 0x060064D3 RID: 25811 RVA: 0x00208538 File Offset: 0x00206738
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.SetState(BuilderTrafficLight.LightState.Off);
		}

		// Token: 0x060064D4 RID: 25812 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPieceDestroy()
		{
		}

		// Token: 0x060064D5 RID: 25813 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x060064D6 RID: 25814 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnPieceActivate()
		{
		}

		// Token: 0x060064D7 RID: 25815 RVA: 0x00208538 File Offset: 0x00206738
		public void OnPieceDeactivate()
		{
			this.SetState(BuilderTrafficLight.LightState.Off);
		}

		// Token: 0x040073CF RID: 29647
		[SerializeField]
		private BuilderPiece piece;

		// Token: 0x040073D0 RID: 29648
		[SerializeField]
		private MeshRenderer redLight;

		// Token: 0x040073D1 RID: 29649
		[SerializeField]
		private MeshRenderer yellowLight;

		// Token: 0x040073D2 RID: 29650
		[SerializeField]
		private MeshRenderer greenLight;

		// Token: 0x040073D3 RID: 29651
		[SerializeField]
		private float cycleDuration = 10f;

		// Token: 0x040073D4 RID: 29652
		[SerializeField]
		private float startPercentageOffset = 0.5f;

		// Token: 0x040073D5 RID: 29653
		[SerializeField]
		private Color redOn = Color.red;

		// Token: 0x040073D6 RID: 29654
		[SerializeField]
		private Color redOff = Color.gray;

		// Token: 0x040073D7 RID: 29655
		[SerializeField]
		private Color yellowOn = Color.yellow;

		// Token: 0x040073D8 RID: 29656
		[SerializeField]
		private Color yellowOff = Color.gray;

		// Token: 0x040073D9 RID: 29657
		[SerializeField]
		private Color greenOn = Color.green;

		// Token: 0x040073DA RID: 29658
		[SerializeField]
		private Color greenOff = Color.gray;

		// Token: 0x040073DB RID: 29659
		private MaterialPropertyBlock materialProps;

		// Token: 0x040073DC RID: 29660
		[SerializeField]
		private AnimationCurve stateCurve;

		// Token: 0x040073DD RID: 29661
		private BuilderTrafficLight.LightState lightState = BuilderTrafficLight.LightState.Off;

		// Token: 0x02000FC0 RID: 4032
		private enum LightState
		{
			// Token: 0x040073DF RID: 29663
			Red,
			// Token: 0x040073E0 RID: 29664
			Yellow,
			// Token: 0x040073E1 RID: 29665
			Green,
			// Token: 0x040073E2 RID: 29666
			Off
		}
	}
}
