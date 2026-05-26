using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020002EC RID: 748
public abstract class ProjectileWeapon : TransferrableObject
{
	// Token: 0x0600130A RID: 4874
	protected abstract Vector3 GetLaunchPosition();

	// Token: 0x0600130B RID: 4875
	protected abstract Vector3 GetLaunchVelocity();

	// Token: 0x0600130C RID: 4876 RVA: 0x00064CC3 File Offset: 0x00062EC3
	internal override void OnEnable()
	{
		base.OnEnable();
		if (base.myOnlineRig != null)
		{
			base.myOnlineRig.projectileWeapon = this;
		}
		if (base.myRig != null)
		{
			base.myRig.projectileWeapon = this;
		}
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x00064D00 File Offset: 0x00062F00
	protected void LaunchProjectile()
	{
		int hash = PoolUtils.GameObjHashCode(this.projectilePrefab);
		int trailHash = PoolUtils.GameObjHashCode(this.projectileTrail);
		GameObject gameObject = ObjectPools.instance.Instantiate(hash, true);
		float num = Mathf.Abs(base.transform.lossyScale.x);
		gameObject.transform.localScale = Vector3.one * num;
		Vector3 launchPosition = this.GetLaunchPosition();
		Vector3 launchVelocity = this.GetLaunchVelocity();
		bool blueTeam;
		bool orangeTeam;
		bool flag;
		this.GetIsOnTeams(out blueTeam, out orangeTeam, out flag);
		this.AttachTrail(trailHash, gameObject, launchPosition, blueTeam, orangeTeam, flag && this.targetRig, this.targetRig ? this.targetRig.playerColor : default(Color));
		SlingshotProjectile component = gameObject.GetComponent<SlingshotProjectile>();
		if (NetworkSystem.Instance.InRoom)
		{
			int projectileCount = ProjectileTracker.AddAndIncrementLocalProjectile(component, launchVelocity, launchPosition, num);
			component.Launch(launchPosition, launchVelocity, NetworkSystem.Instance.LocalPlayer, blueTeam, orangeTeam, projectileCount, num, flag, base.myRig.playerColor);
			TransferrableObject.PositionState currentState = this.currentState;
			RoomSystem.SendLaunchProjectile(launchPosition, launchVelocity, RoomSystem.ProjectileSource.ProjectileWeapon, projectileCount, false, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			this.PlayLaunchSfx();
		}
		else
		{
			component.Launch(launchPosition, launchVelocity, NetworkSystem.Instance.LocalPlayer, blueTeam, orangeTeam, 0, num, flag, base.myRig.playerColor);
			this.PlayLaunchSfx();
		}
		PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x00064E78 File Offset: 0x00063078
	internal virtual SlingshotProjectile LaunchNetworkedProjectile(Vector3 location, Vector3 velocity, RoomSystem.ProjectileSource projectileSource, int projectileCounter, float scale, bool shouldOverrideColor, Color color, PhotonMessageInfoWrapped info)
	{
		GameObject gameObject = null;
		SlingshotProjectile slingshotProjectile = null;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		try
		{
			int hash = -1;
			int num = -1;
			if (projectileSource == RoomSystem.ProjectileSource.ProjectileWeapon)
			{
				if (this.currentState == TransferrableObject.PositionState.OnChest || this.currentState == TransferrableObject.PositionState.None)
				{
					return null;
				}
				hash = PoolUtils.GameObjHashCode(this.projectilePrefab);
				num = PoolUtils.GameObjHashCode(this.projectileTrail);
			}
			gameObject = ObjectPools.instance.Instantiate(hash, true);
			slingshotProjectile = gameObject.GetComponent<SlingshotProjectile>();
			bool blueTeam;
			bool orangeTeam;
			bool flag;
			this.GetIsOnTeams(out blueTeam, out orangeTeam, out flag);
			if (flag && !shouldOverrideColor && this.targetRig)
			{
				shouldOverrideColor = true;
				color = this.targetRig.playerColor;
			}
			if (num != -1)
			{
				this.AttachTrail(num, slingshotProjectile.gameObject, location, blueTeam, orangeTeam, shouldOverrideColor, color);
			}
			slingshotProjectile.Launch(location, velocity, player, blueTeam, orangeTeam, projectileCounter, scale, shouldOverrideColor, color);
			this.PlayLaunchSfx();
		}
		catch
		{
			MonkeAgent.instance.SendReport("projectile error", player.UserId, player.NickName);
			if (slingshotProjectile != null && slingshotProjectile)
			{
				slingshotProjectile.transform.position = Vector3.zero;
				slingshotProjectile.Deactivate();
				slingshotProjectile = null;
			}
			else if (gameObject.IsNotNull())
			{
				ObjectPools.instance.Destroy(gameObject);
			}
		}
		return slingshotProjectile;
	}

	// Token: 0x0600130F RID: 4879 RVA: 0x00064FC0 File Offset: 0x000631C0
	protected void GetIsOnTeams(out bool blueTeam, out bool orangeTeam, out bool shouldUsePlayerColor)
	{
		NetPlayer player = base.OwningPlayer();
		blueTeam = false;
		orangeTeam = false;
		shouldUsePlayerColor = false;
		if (GorillaGameManager.instance != null)
		{
			GorillaPaintbrawlManager component = GorillaGameManager.instance.GetComponent<GorillaPaintbrawlManager>();
			if (component != null)
			{
				blueTeam = component.OnBlueTeam(player);
				orangeTeam = component.OnRedTeam(player);
				shouldUsePlayerColor = (!blueTeam && !orangeTeam);
			}
		}
	}

	// Token: 0x06001310 RID: 4880 RVA: 0x00065020 File Offset: 0x00063220
	private void AttachTrail(int trailHash, GameObject newProjectile, Vector3 location, bool blueTeam, bool orangeTeam, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(trailHash, true);
		SlingshotProjectileTrail component = gameObject.GetComponent<SlingshotProjectileTrail>();
		if (component.IsNull())
		{
			ObjectPools.instance.Destroy(gameObject);
		}
		newProjectile.transform.position = location;
		component.AttachTrail(newProjectile, blueTeam, orangeTeam, shouldOverrideColor, overrideColor);
	}

	// Token: 0x06001311 RID: 4881 RVA: 0x00065070 File Offset: 0x00063270
	private void PlayLaunchSfx()
	{
		if (this.shootSfx != null && this.shootSfxClips != null && this.shootSfxClips.Length != 0)
		{
			this.shootSfx.GTPlayOneShot(this.shootSfxClips[Random.Range(0, this.shootSfxClips.Length)], 1f);
		}
	}

	// Token: 0x0400173F RID: 5951
	[SerializeField]
	protected GameObject projectilePrefab;

	// Token: 0x04001740 RID: 5952
	[SerializeField]
	private GameObject projectileTrail;

	// Token: 0x04001741 RID: 5953
	public AudioClip[] shootSfxClips;

	// Token: 0x04001742 RID: 5954
	public AudioSource shootSfx;
}
