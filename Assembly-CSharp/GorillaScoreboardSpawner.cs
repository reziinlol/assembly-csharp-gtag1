using System;
using System.Collections;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000A14 RID: 2580
public class GorillaScoreboardSpawner : MonoBehaviour
{
	// Token: 0x060041FE RID: 16894 RVA: 0x00160E97 File Offset: 0x0015F097
	public void Awake()
	{
		base.StartCoroutine(this.UpdateBoard());
	}

	// Token: 0x060041FF RID: 16895 RVA: 0x00160EA6 File Offset: 0x0015F0A6
	private void Start()
	{
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x06004200 RID: 16896 RVA: 0x00160EDE File Offset: 0x0015F0DE
	public bool IsCurrentScoreboard()
	{
		return base.gameObject.activeInHierarchy;
	}

	// Token: 0x06004201 RID: 16897 RVA: 0x00160EEC File Offset: 0x0015F0EC
	public void OnJoinedRoom()
	{
		Debug.Log("SCOREBOARD JOIN ROOM");
		if (this.IsCurrentScoreboard())
		{
			this.notInRoomText.SetActive(false);
			this.currentScoreboard = Object.Instantiate<GameObject>(this.scoreboardPrefab, base.transform).GetComponent<GorillaScoreBoard>();
			this.currentScoreboard.transform.rotation = base.transform.rotation;
			if (this.includeMMR)
			{
				this.currentScoreboard.GetComponent<GorillaScoreBoard>().includeMMR = true;
				this.currentScoreboard.GetComponent<Text>().text = "Player                     Color         Level        MMR";
			}
		}
	}

	// Token: 0x06004202 RID: 16898 RVA: 0x00160F7C File Offset: 0x0015F17C
	public bool IsVisible()
	{
		if (!this.forOverlay)
		{
			return this.controllingParentGameObject.activeSelf;
		}
		return GTPlayer.Instance.inOverlay;
	}

	// Token: 0x06004203 RID: 16899 RVA: 0x00160F9C File Offset: 0x0015F19C
	private IEnumerator UpdateBoard()
	{
		for (;;)
		{
			try
			{
				if (this.currentScoreboard != null)
				{
					bool flag = this.IsVisible();
					foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
					{
						if (flag != gorillaPlayerScoreboardLine.lastVisible)
						{
							gorillaPlayerScoreboardLine.lastVisible = flag;
						}
					}
					if (this.currentScoreboard.boardText.enabled != flag)
					{
						this.currentScoreboard.boardText.enabled = flag;
					}
					if (this.currentScoreboard.buttonText.enabled != flag)
					{
						this.currentScoreboard.buttonText.enabled = flag;
					}
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x06004204 RID: 16900 RVA: 0x00160FAB File Offset: 0x0015F1AB
	public void OnLeftRoom()
	{
		this.Cleanup();
		if (this.notInRoomText)
		{
			this.notInRoomText.SetActive(true);
		}
	}

	// Token: 0x06004205 RID: 16901 RVA: 0x00160FCC File Offset: 0x0015F1CC
	public void Cleanup()
	{
		if (this.currentScoreboard != null)
		{
			Object.Destroy(this.currentScoreboard.gameObject);
			this.currentScoreboard = null;
		}
	}

	// Token: 0x040053D6 RID: 21462
	public string gameType;

	// Token: 0x040053D7 RID: 21463
	public bool includeMMR;

	// Token: 0x040053D8 RID: 21464
	public GameObject scoreboardPrefab;

	// Token: 0x040053D9 RID: 21465
	public GameObject notInRoomText;

	// Token: 0x040053DA RID: 21466
	public GameObject controllingParentGameObject;

	// Token: 0x040053DB RID: 21467
	public bool isActive = true;

	// Token: 0x040053DC RID: 21468
	public GorillaScoreBoard currentScoreboard;

	// Token: 0x040053DD RID: 21469
	public bool lastVisible;

	// Token: 0x040053DE RID: 21470
	public bool forOverlay;
}
