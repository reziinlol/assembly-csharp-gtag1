using System;
using UnityEngine;

// Token: 0x02000024 RID: 36
public class MouseRotationDrag : MonoBehaviour
{
	// Token: 0x0600008B RID: 139 RVA: 0x00004C8C File Offset: 0x00002E8C
	private void Start()
	{
		this.m_currFrameHasFocus = false;
		this.m_prevFrameHasFocus = false;
	}

	// Token: 0x0600008C RID: 140 RVA: 0x00004C9C File Offset: 0x00002E9C
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
		Vector3 vector = mousePosition - prevMousePosition;
		this.m_prevMousePosition = mousePosition;
		if (!prevFrameHasFocus)
		{
			this.m_euler = base.transform.rotation.eulerAngles;
			return;
		}
		if (Input.GetMouseButton(0))
		{
			this.m_euler.x = this.m_euler.x + vector.y;
			this.m_euler.y = this.m_euler.y + vector.x;
			base.transform.rotation = Quaternion.Euler(this.m_euler);
		}
	}

	// Token: 0x040000A2 RID: 162
	private bool m_currFrameHasFocus;

	// Token: 0x040000A3 RID: 163
	private bool m_prevFrameHasFocus;

	// Token: 0x040000A4 RID: 164
	private Vector3 m_prevMousePosition;

	// Token: 0x040000A5 RID: 165
	private Vector3 m_euler;
}
