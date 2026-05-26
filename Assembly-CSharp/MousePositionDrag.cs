using System;
using UnityEngine;

// Token: 0x02000023 RID: 35
public class MousePositionDrag : MonoBehaviour
{
	// Token: 0x06000088 RID: 136 RVA: 0x00004BFA File Offset: 0x00002DFA
	private void Start()
	{
		this.m_currFrameHasFocus = false;
		this.m_prevFrameHasFocus = false;
	}

	// Token: 0x06000089 RID: 137 RVA: 0x00004C0C File Offset: 0x00002E0C
	private void Update()
	{
		this.m_currFrameHasFocus = Application.isFocused;
		bool prevFrameHasFocus = this.m_prevFrameHasFocus;
		this.m_prevFrameHasFocus = this.m_currFrameHasFocus;
		if (!prevFrameHasFocus && !this.m_currFrameHasFocus)
		{
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		Vector3 prevMousePosition = this.m_prevMousePosition;
		Vector3 a = mousePosition - prevMousePosition;
		this.m_prevMousePosition = mousePosition;
		if (!prevFrameHasFocus)
		{
			return;
		}
		if (Input.GetMouseButton(0))
		{
			base.transform.position += 0.02f * a;
		}
	}

	// Token: 0x0400009F RID: 159
	private bool m_currFrameHasFocus;

	// Token: 0x040000A0 RID: 160
	private bool m_prevFrameHasFocus;

	// Token: 0x040000A1 RID: 161
	private Vector3 m_prevMousePosition;
}
