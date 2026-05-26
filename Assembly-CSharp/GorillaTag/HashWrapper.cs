using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001148 RID: 4424
	[Serializable]
	public struct HashWrapper : IEquatable<int>
	{
		// Token: 0x0600702A RID: 28714 RVA: 0x00249453 File Offset: 0x00247653
		public HashWrapper(int hash = -1)
		{
			this.hashCode = hash;
		}

		// Token: 0x0600702B RID: 28715 RVA: 0x0024945C File Offset: 0x0024765C
		public override int GetHashCode()
		{
			return this.hashCode;
		}

		// Token: 0x0600702C RID: 28716 RVA: 0x00249464 File Offset: 0x00247664
		public override bool Equals(object obj)
		{
			return this.hashCode.Equals(obj);
		}

		// Token: 0x0600702D RID: 28717 RVA: 0x00249472 File Offset: 0x00247672
		public bool Equals(int i)
		{
			return this.hashCode.Equals(i);
		}

		// Token: 0x0600702E RID: 28718 RVA: 0x0024945C File Offset: 0x0024765C
		public static implicit operator int(in HashWrapper hash)
		{
			return hash.hashCode;
		}

		// Token: 0x0400800B RID: 32779
		[SerializeField]
		private int hashCode;

		// Token: 0x0400800C RID: 32780
		public const int NULL_HASH = -1;
	}
}
