using System;
using UnityEngine;

// Token: 0x02000AAB RID: 2731
public class VirtualStumpRevivePlayer : MonoBehaviour
{
	// Token: 0x060045C8 RID: 17864 RVA: 0x00179AF0 File Offset: 0x00177CF0
	private void OnTriggerEnter(Collider collider)
	{
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			VRRig component = attachedRigidbody.GetComponent<VRRig>();
			if (component != null)
			{
				GRPlayer component2 = component.GetComponent<GRPlayer>();
				if (component2 != null && (component2.State != GRPlayer.GRPlayerState.Alive || component2.Hp < component2.MaxHp))
				{
					if (!NetworkSystem.Instance.InRoom && component == VRRig.LocalRig)
					{
						this.defaultReviveStation.RevivePlayer(component2);
					}
					if (this.ghostReactorManager.IsAuthority())
					{
						this.ghostReactorManager.RequestPlayerRevive(this.defaultReviveStation, component2);
					}
				}
			}
		}
	}

	// Token: 0x04005840 RID: 22592
	[SerializeField]
	private GhostReactorManager ghostReactorManager;

	// Token: 0x04005841 RID: 22593
	[SerializeField]
	private GRReviveStation defaultReviveStation;
}
