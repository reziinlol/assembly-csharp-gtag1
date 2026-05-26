using System;
using System.Collections;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000059 RID: 89
public class CrittersCageDeposit : CrittersActorDeposit
{
	// Token: 0x14000005 RID: 5
	// (add) Token: 0x060001C0 RID: 448 RVA: 0x0000A94C File Offset: 0x00008B4C
	// (remove) Token: 0x060001C1 RID: 449 RVA: 0x0000A984 File Offset: 0x00008B84
	public event Action<Menagerie.CritterData, int> OnDepositCritter;

	// Token: 0x060001C2 RID: 450 RVA: 0x0000A9B9 File Offset: 0x00008BB9
	private void Awake()
	{
		this.attachPoint.OnGrabbedChild += this.StartProcessCage;
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000A9D2 File Offset: 0x00008BD2
	protected override bool CanDeposit(CrittersActor depositActor)
	{
		return base.CanDeposit(depositActor) && !this.isHandlingDeposit;
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000A9E8 File Offset: 0x00008BE8
	private void StartProcessCage(CrittersActor depositedActor)
	{
		this.currentCage = depositedActor;
		base.StartCoroutine(this.ProcessCage());
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x0000A9FE File Offset: 0x00008BFE
	private IEnumerator ProcessCage()
	{
		this.isHandlingDeposit = true;
		bool isLocalDeposit = this.currentCage.lastGrabbedPlayer == PhotonNetwork.LocalPlayer.ActorNumber;
		this.depositAudio.GTPlayOneShot(this.depositStartSound, isLocalDeposit ? 1f : 0.5f);
		float transition = 0f;
		CrittersPawn crittersPawn = this.currentCage.GetComponentInChildren<CrittersPawn>();
		int lastGrabbedPlayer = this.currentCage.lastGrabbedPlayer;
		Menagerie.CritterData critterData;
		if (crittersPawn.IsNotNull())
		{
			critterData = new Menagerie.CritterData(crittersPawn.visuals);
		}
		else
		{
			critterData = new Menagerie.CritterData();
		}
		while (transition < this.submitDuration)
		{
			transition += Time.deltaTime;
			this.attachPoint.transform.localPosition = Vector3.Lerp(this.depositStartLocation, this.depositEndLocation, Mathf.Min(transition / this.submitDuration, 1f));
			yield return null;
		}
		if (crittersPawn.IsNotNull())
		{
			Action<Menagerie.CritterData, int> onDepositCritter = this.OnDepositCritter;
			if (onDepositCritter != null)
			{
				onDepositCritter(critterData, lastGrabbedPlayer);
			}
			CrittersActor crittersActor = crittersPawn;
			bool keepWorldPosition = false;
			Vector3 zero = Vector3.zero;
			crittersActor.Released(keepWorldPosition, default(Quaternion), zero, default(Vector3), default(Vector3));
			crittersPawn.gameObject.SetActive(false);
			this.depositAudio.GTPlayOneShot(this.depositCritterSound, isLocalDeposit ? 1f : 0.5f);
		}
		else
		{
			this.depositAudio.GTPlayOneShot(this.depositEmptySound, isLocalDeposit ? 1f : 0.5f);
		}
		this.currentCage.transform.position = Vector3.zero;
		this.currentCage.gameObject.SetActive(false);
		this.currentCage = null;
		transition = 0f;
		while (transition < this.returnDuration)
		{
			transition += Time.deltaTime;
			this.attachPoint.transform.localPosition = Vector3.Lerp(this.depositEndLocation, this.depositStartLocation, Mathf.Min(transition / this.returnDuration, 1f));
			yield return null;
		}
		this.isHandlingDeposit = false;
		yield break;
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x0000AA10 File Offset: 0x00008C10
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.TransformPoint(this.depositStartLocation), 0.1f);
		Gizmos.DrawLine(base.transform.TransformPoint(this.depositStartLocation), base.transform.TransformPoint(this.depositEndLocation));
		Gizmos.DrawWireSphere(base.transform.TransformPoint(this.depositEndLocation), 0.1f);
	}

	// Token: 0x040001F1 RID: 497
	private bool isHandlingDeposit;

	// Token: 0x040001F2 RID: 498
	public Vector3 depositStartLocation;

	// Token: 0x040001F3 RID: 499
	public Vector3 depositEndLocation;

	// Token: 0x040001F4 RID: 500
	public float submitDuration = 0.5f;

	// Token: 0x040001F5 RID: 501
	public float returnDuration = 1f;

	// Token: 0x040001F6 RID: 502
	public AudioSource depositAudio;

	// Token: 0x040001F7 RID: 503
	public AudioClip depositStartSound;

	// Token: 0x040001F8 RID: 504
	public AudioClip depositEmptySound;

	// Token: 0x040001F9 RID: 505
	public AudioClip depositCritterSound;

	// Token: 0x040001FA RID: 506
	private CrittersActor currentCage;
}
