using System;
using UnityEngine;

// Token: 0x020005E4 RID: 1508
[CreateAssetMenu(fileName = "New AudioMixVarPool", menuName = "ScriptableObjects/AudioMixVarPool", order = 0)]
public class AudioMixVarPool : ScriptableObject
{
	// Token: 0x0600257F RID: 9599 RVA: 0x000C6954 File Offset: 0x000C4B54
	public bool Rent(out AudioMixVar mixVar)
	{
		for (int i = 0; i < this._vars.Length; i++)
		{
			if (!this._vars[i].taken)
			{
				this._vars[i].taken = true;
				mixVar = this._vars[i];
				return true;
			}
		}
		mixVar = null;
		return false;
	}

	// Token: 0x06002580 RID: 9600 RVA: 0x000C69A4 File Offset: 0x000C4BA4
	public void Return(AudioMixVar mixVar)
	{
		if (mixVar == null)
		{
			return;
		}
		int num = this._vars.IndexOfRef(mixVar);
		if (num == -1)
		{
			return;
		}
		this._vars[num].taken = false;
	}

	// Token: 0x040030F4 RID: 12532
	[SerializeField]
	private AudioMixVar[] _vars = new AudioMixVar[0];
}
