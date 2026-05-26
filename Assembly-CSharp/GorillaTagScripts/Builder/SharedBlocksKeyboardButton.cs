using System;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FC4 RID: 4036
	public class SharedBlocksKeyboardButton : GorillaKeyButton<SharedBlocksKeyboardBindings>
	{
		// Token: 0x060064E5 RID: 25829 RVA: 0x00208CAD File Offset: 0x00206EAD
		protected override void OnButtonPressedEvent()
		{
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.Invoke(this.Binding);
		}
	}
}
