using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000089 RID: 137
public class MenagerieDepositBox : MonoBehaviour
{
	// Token: 0x0600037D RID: 893 RVA: 0x00014818 File Offset: 0x00012A18
	public void OnTriggerEnter(Collider other)
	{
		MenagerieCritter component = other.transform.parent.parent.GetComponent<MenagerieCritter>();
		if (component.IsNotNull())
		{
			MenagerieCritter menagerieCritter = component;
			menagerieCritter.OnReleased = (Action<MenagerieCritter>)Delegate.Combine(menagerieCritter.OnReleased, this.OnCritterInserted);
		}
	}

	// Token: 0x0600037E RID: 894 RVA: 0x00014860 File Offset: 0x00012A60
	public void OnTriggerExit(Collider other)
	{
		MenagerieCritter component = other.transform.parent.GetComponent<MenagerieCritter>();
		if (component.IsNotNull())
		{
			MenagerieCritter menagerieCritter = component;
			menagerieCritter.OnReleased = (Action<MenagerieCritter>)Delegate.Remove(menagerieCritter.OnReleased, this.OnCritterInserted);
		}
	}

	// Token: 0x04000401 RID: 1025
	public Action<MenagerieCritter> OnCritterInserted;
}
