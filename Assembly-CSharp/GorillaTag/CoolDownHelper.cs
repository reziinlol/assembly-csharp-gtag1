using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001170 RID: 4464
	[Serializable]
	public class CoolDownHelper
	{
		// Token: 0x0600710C RID: 28940 RVA: 0x0024F4AB File Offset: 0x0024D6AB
		public CoolDownHelper()
		{
			this.coolDown = 1f;
			this.checkTime = 0f;
		}

		// Token: 0x0600710D RID: 28941 RVA: 0x0024F4C9 File Offset: 0x0024D6C9
		public CoolDownHelper(float cd)
		{
			this.coolDown = cd;
			this.checkTime = 0f;
		}

		// Token: 0x0600710E RID: 28942 RVA: 0x0024F4E4 File Offset: 0x0024D6E4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CheckCooldown()
		{
			float unscaledTime = Time.unscaledTime;
			if (unscaledTime < this.checkTime)
			{
				return false;
			}
			this.OnCheckPass();
			this.checkTime = unscaledTime + this.coolDown;
			return true;
		}

		// Token: 0x0600710F RID: 28943 RVA: 0x0024F517 File Offset: 0x0024D717
		public virtual void Start()
		{
			this.checkTime = Time.unscaledTime + this.coolDown;
		}

		// Token: 0x06007110 RID: 28944 RVA: 0x0024F52B File Offset: 0x0024D72B
		public virtual void Stop()
		{
			this.checkTime = float.MaxValue;
		}

		// Token: 0x06007111 RID: 28945 RVA: 0x000028C5 File Offset: 0x00000AC5
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void OnCheckPass()
		{
		}

		// Token: 0x04008128 RID: 33064
		public float coolDown;

		// Token: 0x04008129 RID: 33065
		[NonSerialized]
		public float checkTime;
	}
}
