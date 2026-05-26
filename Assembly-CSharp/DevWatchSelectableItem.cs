using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000B1 RID: 177
public class DevWatchSelectableItem : MonoBehaviour
{
	// Token: 0x0600043F RID: 1087 RVA: 0x00018BE4 File Offset: 0x00016DE4
	public void Init(NetworkObject obj)
	{
		this.SelectedObject = obj;
		this.ItemName.text = obj.name;
		this.Button.onClick.AddListener(delegate()
		{
			this.OnSelected(this.ItemName.text, this.SelectedObject);
		});
	}

	// Token: 0x040004A9 RID: 1193
	public Button Button;

	// Token: 0x040004AA RID: 1194
	public TextMeshProUGUI ItemName;

	// Token: 0x040004AB RID: 1195
	public NetworkObject SelectedObject;

	// Token: 0x040004AC RID: 1196
	public Action<string, NetworkObject> OnSelected;
}
