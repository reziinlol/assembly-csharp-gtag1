using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CosmeticRoom
{
	// Token: 0x02000FF8 RID: 4088
	public class EvolvingCosmeticKiosk : MonoBehaviour
	{
		// Token: 0x1700099A RID: 2458
		// (get) Token: 0x0600663A RID: 26170 RVA: 0x0020F4E8 File Offset: 0x0020D6E8
		// (set) Token: 0x0600663B RID: 26171 RVA: 0x0020F4F0 File Offset: 0x0020D6F0
		public bool Initialized { get; private set; }

		// Token: 0x1700099B RID: 2459
		// (get) Token: 0x0600663C RID: 26172 RVA: 0x0020F4F9 File Offset: 0x0020D6F9
		public VRRig VRRig
		{
			get
			{
				return VRRig.LocalRig;
			}
		}

		// Token: 0x1700099C RID: 2460
		// (get) Token: 0x0600663D RID: 26173 RVA: 0x0020F500 File Offset: 0x0020D700
		// (set) Token: 0x0600663E RID: 26174 RVA: 0x0020F508 File Offset: 0x0020D708
		public bool CosmeticsListBuilding { get; private set; }

		// Token: 0x0600663F RID: 26175 RVA: 0x0020F514 File Offset: 0x0020D714
		private void Awake()
		{
			EvolvingCosmeticKioskButtonSet[] buttonSets = this._buttonSets;
			for (int i = 0; i < buttonSets.Length; i++)
			{
				buttonSets[i].RegisterKiosk(this);
			}
			this.Initialized = true;
		}

		// Token: 0x06006640 RID: 26176 RVA: 0x0020F548 File Offset: 0x0020D748
		private Task BuildCosmeticsList()
		{
			EvolvingCosmeticKiosk.<BuildCosmeticsList>d__14 <BuildCosmeticsList>d__;
			<BuildCosmeticsList>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<BuildCosmeticsList>d__.<>4__this = this;
			<BuildCosmeticsList>d__.<>1__state = -1;
			<BuildCosmeticsList>d__.<>t__builder.Start<EvolvingCosmeticKiosk.<BuildCosmeticsList>d__14>(ref <BuildCosmeticsList>d__);
			return <BuildCosmeticsList>d__.<>t__builder.Task;
		}

		// Token: 0x06006641 RID: 26177 RVA: 0x0020F58C File Offset: 0x0020D78C
		private void ResetButtonSets()
		{
			this._cosmeticIdx = 0;
			EvolvingCosmeticKioskButtonSet[] buttonSets = this._buttonSets;
			for (int i = 0; i < buttonSets.Length; i++)
			{
				buttonSets[i].Reset();
			}
		}

		// Token: 0x06006642 RID: 26178 RVA: 0x0020F5C0 File Offset: 0x0020D7C0
		private void UpdateButtonSets()
		{
			for (int i = 0; i < this._buttonSets.Length; i++)
			{
				int num = this._cosmeticIdx + i;
				if (num >= this._cosmetics.Count)
				{
					this._buttonSets[i].Reset();
				}
				else
				{
					EvolvingCosmeticKiosk.CosmeticData cosmeticData = this._cosmetics[num];
					this._buttonSets[i].SetCosmetic(cosmeticData.PlayfabId, cosmeticData.EvolvingCosmetic);
				}
			}
		}

		// Token: 0x06006643 RID: 26179 RVA: 0x0020F62C File Offset: 0x0020D82C
		public void OnHandScanned(NetPlayer player)
		{
			EvolvingCosmeticKiosk.<OnHandScanned>d__17 <OnHandScanned>d__;
			<OnHandScanned>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnHandScanned>d__.<>4__this = this;
			<OnHandScanned>d__.player = player;
			<OnHandScanned>d__.<>1__state = -1;
			<OnHandScanned>d__.<>t__builder.Start<EvolvingCosmeticKiosk.<OnHandScanned>d__17>(ref <OnHandScanned>d__);
		}

		// Token: 0x06006644 RID: 26180 RVA: 0x0020F66B File Offset: 0x0020D86B
		public void ScrollForward()
		{
			this.Scroll(1);
		}

		// Token: 0x06006645 RID: 26181 RVA: 0x0020F674 File Offset: 0x0020D874
		public void ScrollBackward()
		{
			this.Scroll(-1);
		}

		// Token: 0x06006646 RID: 26182 RVA: 0x0020F67D File Offset: 0x0020D87D
		private void Scroll(int direction)
		{
			this._cosmeticIdx = Math.Clamp(this._cosmeticIdx + direction, 0, this._cosmetics.Count - 1);
			this.UpdateButtonSets();
		}

		// Token: 0x040075C5 RID: 30149
		[SerializeField]
		private EvolvingCosmeticKioskButtonSet[] _buttonSets;

		// Token: 0x040075C6 RID: 30150
		private readonly List<EvolvingCosmeticKiosk.CosmeticData> _cosmetics = new List<EvolvingCosmeticKiosk.CosmeticData>();

		// Token: 0x040075C8 RID: 30152
		private int _cosmeticIdx;

		// Token: 0x02000FF9 RID: 4089
		[NullableContext(1)]
		[Nullable(0)]
		private class CosmeticData : IEquatable<EvolvingCosmeticKiosk.CosmeticData>
		{
			// Token: 0x1700099D RID: 2461
			// (get) Token: 0x06006648 RID: 26184 RVA: 0x0020F6B9 File Offset: 0x0020D8B9
			[CompilerGenerated]
			protected virtual Type EqualityContract
			{
				[CompilerGenerated]
				get
				{
					return typeof(EvolvingCosmeticKiosk.CosmeticData);
				}
			}

			// Token: 0x06006649 RID: 26185 RVA: 0x0020F6C8 File Offset: 0x0020D8C8
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("CosmeticData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x0600664A RID: 26186 RVA: 0x0020F714 File Offset: 0x0020D914
			[CompilerGenerated]
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				RuntimeHelpers.EnsureSufficientExecutionStack();
				builder.Append("EvolvingCosmetic = ");
				builder.Append(this.EvolvingCosmetic);
				builder.Append(", PlayfabId = ");
				builder.Append(this.PlayfabId);
				return true;
			}

			// Token: 0x0600664B RID: 26187 RVA: 0x0020F74E File Offset: 0x0020D94E
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator !=(EvolvingCosmeticKiosk.CosmeticData left, EvolvingCosmeticKiosk.CosmeticData right)
			{
				return !(left == right);
			}

			// Token: 0x0600664C RID: 26188 RVA: 0x0020F75A File Offset: 0x0020D95A
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator ==(EvolvingCosmeticKiosk.CosmeticData left, EvolvingCosmeticKiosk.CosmeticData right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			// Token: 0x0600664D RID: 26189 RVA: 0x0020F76E File Offset: 0x0020D96E
			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<EvolvingCosmetic>.Default.GetHashCode(this.EvolvingCosmetic)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.PlayfabId);
			}

			// Token: 0x0600664E RID: 26190 RVA: 0x0020F7AE File Offset: 0x0020D9AE
			[NullableContext(2)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as EvolvingCosmeticKiosk.CosmeticData);
			}

			// Token: 0x0600664F RID: 26191 RVA: 0x0020F7BC File Offset: 0x0020D9BC
			[NullableContext(2)]
			[CompilerGenerated]
			public virtual bool Equals(EvolvingCosmeticKiosk.CosmeticData other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<EvolvingCosmetic>.Default.Equals(this.EvolvingCosmetic, other.EvolvingCosmetic) && EqualityComparer<string>.Default.Equals(this.PlayfabId, other.PlayfabId));
			}

			// Token: 0x06006651 RID: 26193 RVA: 0x0020F81D File Offset: 0x0020DA1D
			[CompilerGenerated]
			protected CosmeticData(EvolvingCosmeticKiosk.CosmeticData original)
			{
				this.EvolvingCosmetic = original.EvolvingCosmetic;
				this.PlayfabId = original.PlayfabId;
			}

			// Token: 0x06006652 RID: 26194 RVA: 0x00002050 File Offset: 0x00000250
			public CosmeticData()
			{
			}

			// Token: 0x040075C9 RID: 30153
			[Nullable(0)]
			public EvolvingCosmetic EvolvingCosmetic;

			// Token: 0x040075CA RID: 30154
			[Nullable(0)]
			public string PlayfabId;
		}
	}
}
