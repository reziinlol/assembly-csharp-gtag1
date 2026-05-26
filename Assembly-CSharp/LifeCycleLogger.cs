using System;
using UnityEngine;

// Token: 0x020009D9 RID: 2521
public class LifeCycleLogger : MonoBehaviour
{
	// Token: 0x0600408F RID: 16527 RVA: 0x0015896D File Offset: 0x00156B6D
	private void Awake()
	{
		PersistLog.Log(string.Format("[AC][F{0}] {1} Awake", Time.frameCount, base.name));
	}

	// Token: 0x06004090 RID: 16528 RVA: 0x0015898E File Offset: 0x00156B8E
	private void Start()
	{
		PersistLog.Log(string.Format("[AC][F{0}] {1} Start", Time.frameCount, base.name));
	}

	// Token: 0x06004091 RID: 16529 RVA: 0x001589AF File Offset: 0x00156BAF
	private void OnEnable()
	{
		PersistLog.Log(string.Format("[AC][F{0}] {1} Enable", Time.frameCount, base.name));
	}

	// Token: 0x06004092 RID: 16530 RVA: 0x001589D0 File Offset: 0x00156BD0
	private void OnDisable()
	{
		PersistLog.Log(string.Format("[AC][F{0}] {1} Disable", Time.frameCount, base.name));
	}

	// Token: 0x06004093 RID: 16531 RVA: 0x001589F1 File Offset: 0x00156BF1
	private void OnDestroy()
	{
		PersistLog.Log(string.Format("[AC][F{0}] {1} OnDestroy", Time.frameCount, base.name));
	}
}
