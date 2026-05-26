using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000749 RID: 1865
public class GRBay : MonoBehaviour
{
	// Token: 0x06002F48 RID: 12104 RVA: 0x001017A6 File Offset: 0x000FF9A6
	private void Awake()
	{
		if (this.playerName != null)
		{
			this.playerName.text = null;
		}
		if (this.maxDropText != null)
		{
			this.maxDropText.text = null;
		}
	}

	// Token: 0x06002F49 RID: 12105 RVA: 0x001017DC File Offset: 0x000FF9DC
	public void Setup(GhostReactor reactor)
	{
		this.reactor = reactor;
		if (this.shuttleLoc != GRShuttleGroupLoc.Invalid && this.shuttleIndex >= 0 && this.shuttleIndex < 10)
		{
			this.unlockShuttle = GRElevatorManager._instance.GetPlayerShuttle(this.shuttleLoc, this.shuttleIndex);
			if (this.unlockShuttle != null)
			{
				this.unlockShuttle.SetBay(this);
			}
		}
		this.Refresh();
	}

	// Token: 0x06002F4A RID: 12106 RVA: 0x00101848 File Offset: 0x000FFA48
	public void SetOpen(bool open)
	{
		if (this.hideWhenOpen != null)
		{
			for (int i = 0; i < this.hideWhenOpen.Count; i++)
			{
				if (this.hideWhenOpen[i] != null)
				{
					this.hideWhenOpen[i].SetActive(!open);
				}
				else
				{
					Debug.LogErrorFormat("Why is hideWhenOpen null {0} at {1}", new object[]
					{
						base.gameObject.name,
						i
					});
				}
			}
		}
		else
		{
			Debug.LogErrorFormat("Why is hideWhenOpen null {0}", new object[]
			{
				base.gameObject.name
			});
		}
		if (this.hideWhenClosed != null)
		{
			for (int j = 0; j < this.hideWhenClosed.Count; j++)
			{
				if (this.hideWhenClosed[j] != null)
				{
					this.hideWhenClosed[j].SetActive(open);
				}
				else
				{
					Debug.LogErrorFormat("Why is hideWhenClosed null {0} at {1} ", new object[]
					{
						base.gameObject.name,
						j
					});
				}
			}
		}
		else
		{
			Debug.LogErrorFormat("Why is hideWhenClosed null {0}", new object[]
			{
				base.gameObject.name
			});
		}
		if (this.bayDoorAnimation != null && this.isOpen != open)
		{
			if (open)
			{
				this.bayDoorAnimation.Play("BayDoor_Open");
				this.bayDoorAnimation.PlayQueued("BayDoor_Open_Idle");
			}
			else
			{
				this.bayDoorAnimation.Play("BayDoor_Close");
				this.bayDoorAnimation.PlayQueued("BayDoor_Close_Idle");
			}
		}
		this.isOpen = open;
	}

	// Token: 0x06002F4B RID: 12107 RVA: 0x001019DC File Offset: 0x000FFBDC
	public void Refresh()
	{
		bool open = true;
		if (this.unlockShuttle != null)
		{
			NetPlayer owner = this.unlockShuttle.GetOwner();
			bool flag = owner != null && this.unlockShuttle.IsPodUnlocked();
			open = (this.unlockShuttle.GetState() == GRShuttleState.Docked && flag);
			if (this.playerName != null)
			{
				this.playerName.text = ((!flag) ? null : owner.SanitizedNickName);
			}
			if (this.maxDropText != null)
			{
				int num = this.unlockShuttle.GetMaxDropFloor() + 1;
				this.maxDropText.text = ((!flag) ? null : num.ToString());
			}
			for (int i = 0; i < this.showWhenOwned.Count; i++)
			{
				this.showWhenOwned[i].SetActive(flag);
			}
			for (int j = 0; j < this.showWhenNotOwned.Count; j++)
			{
				this.showWhenNotOwned[j].SetActive(!flag);
			}
		}
		else if (this.unlockByDrillLevel > 0)
		{
			open = ((this.reactor != null && this.reactor.GetDepthLevel() >= this.unlockByDrillLevel) || GhostReactorManager.bayUnlockEnabled);
		}
		this.SetOpen(open);
	}

	// Token: 0x04003CAB RID: 15531
	public List<GameObject> hideWhenOpen;

	// Token: 0x04003CAC RID: 15532
	public List<GameObject> hideWhenClosed;

	// Token: 0x04003CAD RID: 15533
	public Animation bayDoorAnimation;

	// Token: 0x04003CAE RID: 15534
	private bool isOpen;

	// Token: 0x04003CAF RID: 15535
	public TMP_Text playerName;

	// Token: 0x04003CB0 RID: 15536
	public TMP_Text maxDropText;

	// Token: 0x04003CB1 RID: 15537
	public List<GameObject> showWhenOwned;

	// Token: 0x04003CB2 RID: 15538
	public List<GameObject> showWhenNotOwned;

	// Token: 0x04003CB3 RID: 15539
	public int unlockByDrillLevel = -1;

	// Token: 0x04003CB4 RID: 15540
	public GRShuttleGroupLoc shuttleLoc = GRShuttleGroupLoc.Invalid;

	// Token: 0x04003CB5 RID: 15541
	public int shuttleIndex = -1;

	// Token: 0x04003CB6 RID: 15542
	[NonSerialized]
	public bool debugForceUnlockedByLevel;

	// Token: 0x04003CB7 RID: 15543
	private GRShuttle unlockShuttle;

	// Token: 0x04003CB8 RID: 15544
	private GhostReactor reactor;
}
