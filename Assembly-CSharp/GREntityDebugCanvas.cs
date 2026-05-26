using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

// Token: 0x0200079B RID: 1947
public class GREntityDebugCanvas : MonoBehaviour
{
	// Token: 0x060031D5 RID: 12757 RVA: 0x00111BC8 File Offset: 0x0010FDC8
	private void Awake()
	{
		this.builder = new StringBuilder(50);
	}

	// Token: 0x060031D6 RID: 12758 RVA: 0x00111BD8 File Offset: 0x0010FDD8
	private void Start()
	{
		if (this.text == null && this.textPanelPrefab != null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.textPanelPrefab, base.transform.position + this.prefabAttachOffset, Quaternion.identity, base.transform);
			this.text = gameObject.GetComponent<TMP_Text>();
		}
		if (this.text != null)
		{
			this.text.fontSize = this.fontSize;
			this.text.gameObject.SetActive(false);
		}
	}

	// Token: 0x060031D7 RID: 12759 RVA: 0x00111C6C File Offset: 0x0010FE6C
	private bool UpdateActive()
	{
		bool entityDebugEnabled = GhostReactorManager.entityDebugEnabled;
		if (this.text != null)
		{
			this.text.gameObject.SetActive(entityDebugEnabled);
		}
		return entityDebugEnabled;
	}

	// Token: 0x060031D8 RID: 12760 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Update()
	{
	}

	// Token: 0x060031D9 RID: 12761 RVA: 0x00111CA0 File Offset: 0x0010FEA0
	private void UpdateText()
	{
		if (this.text)
		{
			this.builder.Clear();
			List<IGameEntityDebugComponent> list = new List<IGameEntityDebugComponent>();
			base.GetComponents<IGameEntityDebugComponent>(list);
			foreach (IGameEntityDebugComponent gameEntityDebugComponent in list)
			{
				List<string> list2 = new List<string>();
				gameEntityDebugComponent.GetDebugTextLines(out list2);
				foreach (string value in list2)
				{
					this.builder.AppendLine(value);
				}
			}
			this.text.text = this.builder.ToString();
		}
	}

	// Token: 0x040040A8 RID: 16552
	[SerializeField]
	public TMP_Text text;

	// Token: 0x040040A9 RID: 16553
	public GameObject textPanelPrefab;

	// Token: 0x040040AA RID: 16554
	public Vector3 prefabAttachOffset = new Vector3(0f, 0.5f, 0f);

	// Token: 0x040040AB RID: 16555
	public float fontSize = 100f;

	// Token: 0x040040AC RID: 16556
	private StringBuilder builder;
}
