using System;
using System.Collections;
using GorillaLocomotion.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.ScavengerHunt
{
	// Token: 0x02000F79 RID: 3961
	public class ScavengerTarget : MonoBehaviour, IGorillaGrabable
	{
		// Token: 0x060062DF RID: 25311 RVA: 0x001FDE6E File Offset: 0x001FC06E
		private void Awake()
		{
			base.StartCoroutine(this.ConnectToScavengerManager());
		}

		// Token: 0x060062E0 RID: 25312 RVA: 0x001FDE7D File Offset: 0x001FC07D
		private IEnumerator ConnectToScavengerManager()
		{
			int num;
			for (int i = 0; i < 30; i = num)
			{
				if (!(ScavengerManager.Instance == null))
				{
					ScavengerManager.Instance.RegisterTarget(this);
					this._manager = ScavengerManager.Instance;
					yield break;
				}
				yield return null;
				num = i + 1;
			}
			Object.Destroy(this);
			throw new Exception(string.Format("No ScavengerManager found within {0} frames of attempts.", 30));
			yield break;
		}

		// Token: 0x060062E1 RID: 25313 RVA: 0x001FDE8C File Offset: 0x001FC08C
		public void Collect()
		{
			this._manager.Collect(this);
		}

		// Token: 0x060062E2 RID: 25314 RVA: 0x00023994 File Offset: 0x00021B94
		public bool MomentaryGrabOnly()
		{
			return true;
		}

		// Token: 0x060062E3 RID: 25315 RVA: 0x001FDE9A File Offset: 0x001FC09A
		public bool CanBeGrabbed(GorillaGrabber grabber)
		{
			return !this._manager.IsCollected(this);
		}

		// Token: 0x060062E4 RID: 25316 RVA: 0x001FDEAB File Offset: 0x001FC0AB
		public void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
		{
			this.Collect();
			grabbedTransform = base.transform;
			localGrabbedPosition = base.transform.InverseTransformPoint(grabber.transform.position);
		}

		// Token: 0x060062E5 RID: 25317 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnGrabReleased(GorillaGrabber grabber)
		{
		}

		// Token: 0x060062E7 RID: 25319 RVA: 0x00014807 File Offset: 0x00012A07
		string IGorillaGrabable.get_name()
		{
			return base.name;
		}

		// Token: 0x040071A1 RID: 29089
		public string HuntName;

		// Token: 0x040071A2 RID: 29090
		public string TargetName;

		// Token: 0x040071A3 RID: 29091
		public UnityEvent[] TargetCollected;

		// Token: 0x040071A4 RID: 29092
		public UnityEvent<ScavengerTarget>[] TargetCollectedArg;

		// Token: 0x040071A5 RID: 29093
		private ScavengerManager _manager;
	}
}
