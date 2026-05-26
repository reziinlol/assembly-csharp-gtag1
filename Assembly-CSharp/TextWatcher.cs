using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020009EC RID: 2540
public class TextWatcher : MonoBehaviour
{
	// Token: 0x06004106 RID: 16646 RVA: 0x0015BAE5 File Offset: 0x00159CE5
	private void Start()
	{
		this.myText = base.GetComponent<Text>();
		this.textToCopy.AddCallback(new Action<string>(this.OnTextChanged), true);
	}

	// Token: 0x06004107 RID: 16647 RVA: 0x0015BB0B File Offset: 0x00159D0B
	private void OnDestroy()
	{
		this.textToCopy.RemoveCallback(new Action<string>(this.OnTextChanged));
	}

	// Token: 0x06004108 RID: 16648 RVA: 0x0015BB24 File Offset: 0x00159D24
	private void OnTextChanged(string newText)
	{
		this.myText.text = newText;
	}

	// Token: 0x040051B5 RID: 20917
	public WatchableStringSO textToCopy;

	// Token: 0x040051B6 RID: 20918
	private Text myText;
}
