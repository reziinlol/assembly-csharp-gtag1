using System;
using GorillaGameModes;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200085C RID: 2140
public class GorillaGuardianEjectWatch : MonoBehaviour
{
	// Token: 0x06003795 RID: 14229 RVA: 0x0012FD28 File Offset: 0x0012DF28
	private void Start()
	{
		if (this.ejectButton != null)
		{
			this.ejectButton.onPressButton.AddListener(new UnityAction(this.OnEjectButtonPressed));
		}
	}

	// Token: 0x06003796 RID: 14230 RVA: 0x0012FD54 File Offset: 0x0012DF54
	private void OnDestroy()
	{
		if (this.ejectButton != null)
		{
			this.ejectButton.onPressButton.RemoveListener(new UnityAction(this.OnEjectButtonPressed));
		}
	}

	// Token: 0x06003797 RID: 14231 RVA: 0x0012FD80 File Offset: 0x0012DF80
	private void OnEjectButtonPressed()
	{
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null)
		{
			gorillaGuardianManager.RequestEjectGuardian(NetworkSystem.Instance.LocalPlayer);
		}
	}

	// Token: 0x0400479B RID: 18331
	[SerializeField]
	private HeldButton ejectButton;
}
