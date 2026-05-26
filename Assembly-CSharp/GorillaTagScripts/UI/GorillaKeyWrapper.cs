using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.UI
{
	// Token: 0x02000F5F RID: 3935
	public class GorillaKeyWrapper<TBinding> : MonoBehaviour where TBinding : Enum
	{
		// Token: 0x0600620D RID: 25101 RVA: 0x001FA5AC File Offset: 0x001F87AC
		public void Start()
		{
			if (!this.defineButtonsManually)
			{
				this.FindMatchingButtons(base.gameObject);
				return;
			}
			if (this.buttons.Count > 0)
			{
				for (int i = this.buttons.Count - 1; i >= 0; i--)
				{
					if (this.buttons[i].IsNull())
					{
						this.buttons.RemoveAt(i);
					}
					else
					{
						this.buttons[i].OnKeyButtonPressed.AddListener(new UnityAction<TBinding>(this.OnKeyButtonPressed));
					}
				}
			}
		}

		// Token: 0x0600620E RID: 25102 RVA: 0x001FA638 File Offset: 0x001F8838
		public void OnDestroy()
		{
			for (int i = 0; i < this.buttons.Count; i++)
			{
				if (this.buttons[i].IsNotNull())
				{
					this.buttons[i].OnKeyButtonPressed.RemoveListener(new UnityAction<TBinding>(this.OnKeyButtonPressed));
				}
			}
		}

		// Token: 0x0600620F RID: 25103 RVA: 0x001FA690 File Offset: 0x001F8890
		public void FindMatchingButtons(GameObject obj)
		{
			if (obj.IsNull())
			{
				return;
			}
			for (int i = 0; i < obj.transform.childCount; i++)
			{
				Transform child = obj.transform.GetChild(i);
				if (child.IsNotNull())
				{
					this.FindMatchingButtons(child.gameObject);
				}
			}
			GorillaKeyButton<TBinding> component = obj.GetComponent<GorillaKeyButton<TBinding>>();
			if (component.IsNotNull() && !this.buttons.Contains(component))
			{
				this.buttons.Add(component);
				component.OnKeyButtonPressed.AddListener(new UnityAction<TBinding>(this.OnKeyButtonPressed));
			}
		}

		// Token: 0x06006210 RID: 25104 RVA: 0x001FA71D File Offset: 0x001F891D
		private void OnKeyButtonPressed(TBinding binding)
		{
			UnityEvent<TBinding> onKeyPressed = this.OnKeyPressed;
			if (onKeyPressed == null)
			{
				return;
			}
			onKeyPressed.Invoke(binding);
		}

		// Token: 0x040070D5 RID: 28885
		public UnityEvent<TBinding> OnKeyPressed = new UnityEvent<TBinding>();

		// Token: 0x040070D6 RID: 28886
		public bool defineButtonsManually;

		// Token: 0x040070D7 RID: 28887
		public List<GorillaKeyButton<TBinding>> buttons = new List<GorillaKeyButton<TBinding>>();
	}
}
