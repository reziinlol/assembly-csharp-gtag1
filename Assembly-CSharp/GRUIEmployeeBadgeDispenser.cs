using System;
using TMPro;
using UnityEngine;

// Token: 0x0200081B RID: 2075
public class GRUIEmployeeBadgeDispenser : MonoBehaviour
{
	// Token: 0x06003545 RID: 13637 RVA: 0x00126D92 File Offset: 0x00124F92
	public void Setup(GhostReactor reactor, int employeeIndex)
	{
		this.reactor = reactor;
	}

	// Token: 0x06003546 RID: 13638 RVA: 0x00126D9C File Offset: 0x00124F9C
	public void Refresh()
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.actorNr);
		if (player != null && player.InRoom)
		{
			this.playerName.text = player.SanitizedNickName;
			if (this.idBadge != null)
			{
				this.idBadge.RefreshText(player);
				return;
			}
		}
		else
		{
			this.playerName.text = "";
		}
	}

	// Token: 0x06003547 RID: 13639 RVA: 0x00126E04 File Offset: 0x00125004
	public void CreateBadge(NetPlayer player, GameEntityManager entityManager)
	{
		if (entityManager.IsAuthority())
		{
			entityManager.RequestCreateItem(this.idBadgePrefab.name.GetStaticHash(), this.spawnLocation.position, this.spawnLocation.rotation, (long)(player.ActorNumber * 100 + this.index));
		}
	}

	// Token: 0x06003548 RID: 13640 RVA: 0x00126E57 File Offset: 0x00125057
	public Transform GetSpawnMarker()
	{
		return this.spawnLocation;
	}

	// Token: 0x06003549 RID: 13641 RVA: 0x00126E5F File Offset: 0x0012505F
	public bool IsDispenserForBadge(GRBadge badge)
	{
		return badge == this.idBadge;
	}

	// Token: 0x0600354A RID: 13642 RVA: 0x00126E6D File Offset: 0x0012506D
	public Vector3 GetSpawnPosition()
	{
		return this.spawnLocation.position;
	}

	// Token: 0x0600354B RID: 13643 RVA: 0x00126E7A File Offset: 0x0012507A
	public Quaternion GetSpawnRotation()
	{
		return this.spawnLocation.rotation;
	}

	// Token: 0x0600354C RID: 13644 RVA: 0x00126E87 File Offset: 0x00125087
	public void ClearBadge()
	{
		this.actorNr = -1;
		this.idBadge = null;
	}

	// Token: 0x0600354D RID: 13645 RVA: 0x00126E98 File Offset: 0x00125098
	public void AttachIDBadge(GRBadge linkedBadge, NetPlayer _player)
	{
		this.actorNr = ((_player == null) ? -1 : _player.ActorNumber);
		this.idBadge = linkedBadge;
		this.playerName.text = ((_player == null) ? null : _player.SanitizedNickName);
		this.idBadge.Setup(_player, this.index);
	}

	// Token: 0x040045CE RID: 17870
	[SerializeField]
	private TMP_Text msg;

	// Token: 0x040045CF RID: 17871
	[SerializeField]
	private TMP_Text playerName;

	// Token: 0x040045D0 RID: 17872
	[SerializeField]
	private Transform spawnLocation;

	// Token: 0x040045D1 RID: 17873
	[SerializeField]
	private GameEntity idBadgePrefab;

	// Token: 0x040045D2 RID: 17874
	[SerializeField]
	private LayerMask badgeLayerMask;

	// Token: 0x040045D3 RID: 17875
	public int index;

	// Token: 0x040045D4 RID: 17876
	public int actorNr;

	// Token: 0x040045D5 RID: 17877
	public GRBadge idBadge;

	// Token: 0x040045D6 RID: 17878
	private GhostReactor reactor;

	// Token: 0x040045D7 RID: 17879
	private Coroutine getSpawnedBadgeCoroutine;

	// Token: 0x040045D8 RID: 17880
	private static Collider[] overlapColliders = new Collider[10];

	// Token: 0x040045D9 RID: 17881
	private bool isEmployee;

	// Token: 0x040045DA RID: 17882
	private const string GR_DATA_KEY = "GRData";
}
