using System;
using UnityEngine;

namespace Cosmetics
{
	// Token: 0x0200112F RID: 4399
	public interface ICreatorCodeProvider
	{
		// Token: 0x17000AA7 RID: 2727
		// (get) Token: 0x06006FBE RID: 28606
		GameObject GameObject { get; }

		// Token: 0x17000AA8 RID: 2728
		// (get) Token: 0x06006FBF RID: 28607
		string TerminalId { get; }

		// Token: 0x06006FC0 RID: 28608
		void GetCreatorCode(out string code, out NexusGroupId[] groups);
	}
}
