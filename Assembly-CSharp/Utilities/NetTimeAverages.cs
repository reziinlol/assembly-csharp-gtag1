using System;
using System.Runtime.CompilerServices;

namespace Utilities
{
	// Token: 0x02000E9E RID: 3742
	public class NetTimeAverages : DoubleAverages
	{
		// Token: 0x06005BFA RID: 23546 RVA: 0x001D3E82 File Offset: 0x001D2082
		public NetTimeAverages(int sampleCount) : base(sampleCount)
		{
		}

		// Token: 0x06005BFB RID: 23547 RVA: 0x001D3E8B File Offset: 0x001D208B
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override double DefaultTypeValue()
		{
			return 1.0;
		}
	}
}
