using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000E9D RID: 3741
	public class FloatAverages : AverageCalculator<float>
	{
		// Token: 0x06005BF5 RID: 23541 RVA: 0x001D3E67 File Offset: 0x001D2067
		public FloatAverages(int sampleCount) : base(sampleCount)
		{
			this.Reset();
		}

		// Token: 0x06005BF6 RID: 23542 RVA: 0x001D3E51 File Offset: 0x001D2051
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float PlusEquals(float value, float sample)
		{
			return value + sample;
		}

		// Token: 0x06005BF7 RID: 23543 RVA: 0x001D3E56 File Offset: 0x001D2056
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float MinusEquals(float value, float sample)
		{
			return value - sample;
		}

		// Token: 0x06005BF8 RID: 23544 RVA: 0x001D3E76 File Offset: 0x001D2076
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float Divide(float value, int sampleCount)
		{
			return value / (float)sampleCount;
		}

		// Token: 0x06005BF9 RID: 23545 RVA: 0x001D3E7C File Offset: 0x001D207C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override float Multiply(float value, int sampleCount)
		{
			return value * (float)sampleCount;
		}
	}
}
