using System;
using GorillaTagScripts.UI;

namespace GorillaTagScripts.VirtualStumpCustomMaps.UI
{
	// Token: 0x02000F54 RID: 3924
	public class CustomMapsKeyboard : GorillaKeyWrapper<CustomMapKeyboardBinding>
	{
		// Token: 0x060061EA RID: 25066 RVA: 0x001F9577 File Offset: 0x001F7777
		public static string BindingToString(CustomMapKeyboardBinding binding)
		{
			return CustomMapsKeyButton.BindingToString(binding);
		}
	}
}
