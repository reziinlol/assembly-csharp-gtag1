using System;

namespace GorillaNetworking
{
	// Token: 0x02001058 RID: 4184
	public static class GorillaKeyboardBindingExtensions
	{
		// Token: 0x0600691C RID: 26908 RVA: 0x002205D8 File Offset: 0x0021E7D8
		public static bool FromNumberBindingToInt(this GorillaKeyboardBindings binding, out int result)
		{
			result = -1;
			switch (binding)
			{
			case GorillaKeyboardBindings.zero:
				result = 0;
				break;
			case GorillaKeyboardBindings.one:
				result = 1;
				break;
			case GorillaKeyboardBindings.two:
				result = 2;
				break;
			case GorillaKeyboardBindings.three:
				result = 3;
				break;
			case GorillaKeyboardBindings.four:
				result = 4;
				break;
			case GorillaKeyboardBindings.five:
				result = 5;
				break;
			case GorillaKeyboardBindings.six:
				result = 6;
				break;
			case GorillaKeyboardBindings.seven:
				result = 7;
				break;
			case GorillaKeyboardBindings.eight:
				result = 8;
				break;
			case GorillaKeyboardBindings.nine:
				result = 9;
				break;
			default:
				return false;
			}
			return true;
		}
	}
}
