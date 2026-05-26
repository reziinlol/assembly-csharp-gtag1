using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020011FB RID: 4603
	public class PlayerSpeakerSwapper : MonoBehaviour
	{
		// Token: 0x0600737E RID: 29566 RVA: 0x002587A4 File Offset: 0x002569A4
		private void OnEnable()
		{
			NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerCountChanged;
			NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerCountChanged;
			this.OnPlayerCountChanged(null);
		}

		// Token: 0x0600737F RID: 29567 RVA: 0x002587FC File Offset: 0x002569FC
		private void OnDisable()
		{
			NetworkSystem.Instance.OnPlayerJoined -= this.OnPlayerCountChanged;
			NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerCountChanged;
		}

		// Token: 0x06007380 RID: 29568 RVA: 0x0025884C File Offset: 0x00256A4C
		private void OnPlayerCountChanged(NetPlayer _)
		{
			int num = NetworkSystem.Instance.AllNetPlayers.Length;
			this._lowPassFilter.enabled = (num >= 10);
		}

		// Token: 0x040083D9 RID: 33753
		[SerializeField]
		private Behaviour _lowPassFilter;
	}
}
