using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200138E RID: 5006
	[AttributeUsage(AttributeTargets.Field)]
	public class ConditionalFieldAttribute : PropertyAttribute
	{
		// Token: 0x17000BF5 RID: 3061
		// (get) Token: 0x06007E07 RID: 32263 RVA: 0x00296FD7 File Offset: 0x002951D7
		public bool ShowRange
		{
			get
			{
				return this.Min != this.Max;
			}
		}

		// Token: 0x06007E08 RID: 32264 RVA: 0x00296FEC File Offset: 0x002951EC
		public ConditionalFieldAttribute(string propertyToCheck = null, object compareValue = null, object compareValue2 = null, object compareValue3 = null, object compareValue4 = null, object compareValue5 = null, object compareValue6 = null)
		{
			this.PropertyToCheck = propertyToCheck;
			this.CompareValue = compareValue;
			this.CompareValue2 = compareValue2;
			this.CompareValue3 = compareValue3;
			this.CompareValue4 = compareValue4;
			this.CompareValue5 = compareValue5;
			this.CompareValue6 = compareValue6;
			this.Label = "";
			this.Tooltip = "";
			this.Min = 0f;
			this.Max = 0f;
		}

		// Token: 0x04008F5B RID: 36699
		public string PropertyToCheck;

		// Token: 0x04008F5C RID: 36700
		public object CompareValue;

		// Token: 0x04008F5D RID: 36701
		public object CompareValue2;

		// Token: 0x04008F5E RID: 36702
		public object CompareValue3;

		// Token: 0x04008F5F RID: 36703
		public object CompareValue4;

		// Token: 0x04008F60 RID: 36704
		public object CompareValue5;

		// Token: 0x04008F61 RID: 36705
		public object CompareValue6;

		// Token: 0x04008F62 RID: 36706
		public string Label;

		// Token: 0x04008F63 RID: 36707
		public string Tooltip;

		// Token: 0x04008F64 RID: 36708
		public float Min;

		// Token: 0x04008F65 RID: 36709
		public float Max;
	}
}
