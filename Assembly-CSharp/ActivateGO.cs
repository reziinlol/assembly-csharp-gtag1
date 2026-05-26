using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200000B RID: 11
public class ActivateGO : MonoBehaviour
{
	// Token: 0x0600001D RID: 29 RVA: 0x000023DB File Offset: 0x000005DB
	private void OnEnable()
	{
		this.active = PlayerPrefFlags.Check(this.flag);
		this.SetGOsActive(0);
		PlayerPrefFlags.OnFlagChange = (Action<PlayerPrefFlags.Flag, bool>)Delegate.Combine(PlayerPrefFlags.OnFlagChange, new Action<PlayerPrefFlags.Flag, bool>(this.OnFlagChange));
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00002415 File Offset: 0x00000615
	private void OnDisable()
	{
		PlayerPrefFlags.OnFlagChange = (Action<PlayerPrefFlags.Flag, bool>)Delegate.Remove(PlayerPrefFlags.OnFlagChange, new Action<PlayerPrefFlags.Flag, bool>(this.OnFlagChange));
	}

	// Token: 0x0600001F RID: 31 RVA: 0x00002415 File Offset: 0x00000615
	private void OnDestroy()
	{
		PlayerPrefFlags.OnFlagChange = (Action<PlayerPrefFlags.Flag, bool>)Delegate.Remove(PlayerPrefFlags.OnFlagChange, new Action<PlayerPrefFlags.Flag, bool>(this.OnFlagChange));
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00002437 File Offset: 0x00000637
	public void OnFlagChange(PlayerPrefFlags.Flag f, bool value)
	{
		if (f != this.flag)
		{
			return;
		}
		this.active = value;
		this.SetGOsActive(this.flashes);
	}

	// Token: 0x06000021 RID: 33 RVA: 0x00002458 File Offset: 0x00000658
	private void SetGOsActive(int fls)
	{
		ActivateGO.<SetGOsActive>d__10 <SetGOsActive>d__;
		<SetGOsActive>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetGOsActive>d__.<>4__this = this;
		<SetGOsActive>d__.fls = fls;
		<SetGOsActive>d__.<>1__state = -1;
		<SetGOsActive>d__.<>t__builder.Start<ActivateGO.<SetGOsActive>d__10>(ref <SetGOsActive>d__);
	}

	// Token: 0x06000022 RID: 34 RVA: 0x00002498 File Offset: 0x00000698
	private void toggle(List<Renderer> renderers, bool state)
	{
		for (int i = 0; i < renderers.Count; i++)
		{
			if ((this.layerMask.value & 1 << renderers[i].gameObject.layer) != 0)
			{
				renderers[i].forceRenderingOff = !state;
			}
		}
	}

	// Token: 0x04000009 RID: 9
	[SerializeField]
	private GameObject targetGO;

	// Token: 0x0400000A RID: 10
	[SerializeField]
	private PlayerPrefFlags.Flag flag;

	// Token: 0x0400000B RID: 11
	[SerializeField]
	private int flashes;

	// Token: 0x0400000C RID: 12
	[SerializeField]
	private LayerMask layerMask;

	// Token: 0x0400000D RID: 13
	private bool active;

	// Token: 0x0400000E RID: 14
	private bool flashing;
}
