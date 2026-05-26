using System;

namespace TagEffects
{
	// Token: 0x020010D6 RID: 4310
	[Serializable]
	public class TagEffectsCombo : IEquatable<TagEffectsCombo>
	{
		// Token: 0x06006BF1 RID: 27633 RVA: 0x0022F2C8 File Offset: 0x0022D4C8
		bool IEquatable<TagEffectsCombo>.Equals(TagEffectsCombo other)
		{
			return (other.inputA == this.inputA && other.inputB == this.inputB) || (other.inputA == this.inputB && other.inputB == this.inputA);
		}

		// Token: 0x06006BF2 RID: 27634 RVA: 0x0022F323 File Offset: 0x0022D523
		public override bool Equals(object obj)
		{
			return this.Equals((TagEffectsCombo)obj);
		}

		// Token: 0x06006BF3 RID: 27635 RVA: 0x0022F331 File Offset: 0x0022D531
		public override int GetHashCode()
		{
			return this.inputA.GetHashCode() * this.inputB.GetHashCode();
		}

		// Token: 0x04007C22 RID: 31778
		public TagEffectPack inputA;

		// Token: 0x04007C23 RID: 31779
		public TagEffectPack inputB;
	}
}
