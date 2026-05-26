using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000E9C RID: 3740
	public class DoubleAverages : AverageCalculator<double>
	{
		// Token: 0x06005BF0 RID: 23536 RVA: 0x001D3E42 File Offset: 0x001D2042
		public DoubleAverages(int sampleCount) : base(sampleCount)
		{
			this.Reset();
		}

		// Token: 0x06005BF1 RID: 23537 RVA: 0x001D3E51 File Offset: 0x001D2051
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double PlusEquals(double value, double sample)
		{
			return value + sample;
		}

		// Token: 0x06005BF2 RID: 23538 RVA: 0x001D3E56 File Offset: 0x001D2056
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double MinusEquals(double value, double sample)
		{
			return value - sample;
		}

		// Token: 0x06005BF3 RID: 23539 RVA: 0x001D3E5B File Offset: 0x001D205B
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double Divide(double value, int sampleCount)
		{
			return value / (double)sampleCount;
		}

		// Token: 0x06005BF4 RID: 23540 RVA: 0x001D3E61 File Offset: 0x001D2061
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double Multiply(double value, int sampleCount)
		{
			return value * (double)sampleCount;
		}
	}
}
