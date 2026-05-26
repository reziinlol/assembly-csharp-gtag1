using System;
using UnityEngine;

namespace PerformanceSystems
{
	// Token: 0x02000EA7 RID: 3751
	public class TimeSliceControllerBehaviour : MonoBehaviour
	{
		// Token: 0x06005C1F RID: 23583 RVA: 0x001D42D8 File Offset: 0x001D24D8
		private void Awake()
		{
			this._timeSliceControllerAsset.InitializeReferenceTransformWithMainCam();
		}

		// Token: 0x06005C20 RID: 23584 RVA: 0x001D42E5 File Offset: 0x001D24E5
		private void Update()
		{
			this._timeSliceControllerAsset.Update();
		}

		// Token: 0x04006A91 RID: 27281
		[SerializeField]
		private TimeSliceControllerAsset _timeSliceControllerAsset;
	}
}
