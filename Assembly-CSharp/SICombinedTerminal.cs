using System;
using System.Collections.Generic;
using System.IO;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000138 RID: 312
public class SICombinedTerminal : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x17000096 RID: 150
	// (get) Token: 0x060007C4 RID: 1988 RVA: 0x0002A73F File Offset: 0x0002893F
	public bool IsAuthority
	{
		get
		{
			return this.superInfection.siManager.gameEntityManager.IsAuthority();
		}
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x060007C5 RID: 1989 RVA: 0x0002A756 File Offset: 0x00028956
	public SuperInfectionManager SIManager
	{
		get
		{
			return this.superInfection.siManager;
		}
	}

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x060007C6 RID: 1990 RVA: 0x0002A763 File Offset: 0x00028963
	public int ActivePage
	{
		get
		{
			return this._activePage;
		}
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x00018E08 File Offset: 0x00017008
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x00018E11 File Offset: 0x00017011
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x0002A76C File Offset: 0x0002896C
	public void SliceUpdate()
	{
		this.wasOccupied = this.isOccupied;
		this.isOccupied = false;
		this.isOccupiedByActivePlayer = false;
		VRRigCache.Instance.GetActiveRigs(this.rigs);
		for (int i = 0; i < this.rigs.Count; i++)
		{
			if (this.activeUserBounds.bounds.Contains(this.rigs[i].transform.position))
			{
				this.isOccupied = true;
				if (this.rigs[i].Creator.IsLocal)
				{
					this.isOccupiedByActivePlayer = true;
					break;
				}
			}
		}
		if (this.isOccupied)
		{
			float num = Time.time - SIProgression.Instance.timeTelemetryLastChecked;
			if (this.activePlayer != null && this.activePlayer.ActorNr == SIPlayer.LocalPlayer.ActorNr && this.isOccupiedByLocalPlayer)
			{
				SIProgression.Instance.activeTerminalTimeInterval += num;
				SIProgression.Instance.activeTerminalTimeTotal += num;
			}
			if (!this.wasOccupied && this.state == EKioskAnimState.Closing)
			{
				this.AnimQueueState(EKioskAnimState.Opening);
			}
			this.foldupTimeStart = Time.time;
			return;
		}
		if (this.state == EKioskAnimState.Opening && Time.time > this.foldupTimeStart + this.foldupDelay && !this.isOccupied)
		{
			this.AnimQueueState(EKioskAnimState.Closing);
		}
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x0002A8C8 File Offset: 0x00028AC8
	public void Reset()
	{
		this.activePlayer = null;
		this.SetActivePage(0);
		this.dispenser.Initialize();
		this.techTree.Initialize();
		this.resourceCollection.Initialize();
		this.dispenser.Reset();
		this.techTree.Reset();
		this.resourceCollection.Reset();
		this.AnimQueueState(EKioskAnimState.Closing);
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x0002A92C File Offset: 0x00028B2C
	public void Awake()
	{
		if (this.superInfection == null)
		{
			this.superInfection = base.GetComponentInParent<SuperInfection>();
		}
		this.dispenser.Initialize();
		this.techTree.Initialize();
		this.resourceCollection.Initialize();
		this.Reset();
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x0002A97C File Offset: 0x00028B7C
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.activePlayer != null)
		{
			stream.SendNext(this.activePlayer.ActorNr);
		}
		else
		{
			stream.SendNext(-1);
		}
		stream.SendNext(this._activePage);
		this.dispenser.WriteDataPUN(stream, info);
		this.techTree.WriteDataPUN(stream, info);
		this.resourceCollection.WriteDataPUN(stream, info);
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x0002A9F4 File Offset: 0x00028BF4
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.activePlayer = SIPlayer.Get((int)stream.ReceiveNext());
		this._activePage = (int)stream.ReceiveNext();
		this.dispenser.ReadDataPUN(stream, info);
		this.techTree.ReadDataPUN(stream, info);
		this.resourceCollection.ReadDataPUN(stream, info);
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0002AA4F File Offset: 0x00028C4F
	public void SerializeZoneData(BinaryWriter writer)
	{
		writer.Write(this._activePage);
		this.dispenser.ZoneDataSerializeWrite(writer);
		this.techTree.ZoneDataSerializeWrite(writer);
		this.resourceCollection.ZoneDataSerializeWrite(writer);
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x0002AA81 File Offset: 0x00028C81
	public void DeserializeZoneData(BinaryReader reader)
	{
		this._activePage = reader.ReadInt32();
		this.SetActivePage(this._activePage);
		this.dispenser.ZoneDataSerializeRead(reader);
		this.techTree.ZoneDataSerializeRead(reader);
		this.resourceCollection.ZoneDataSerializeRead(reader);
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x0002AAC0 File Offset: 0x00028CC0
	public void PlayerHandScanned(int actorNr)
	{
		if (!this.IsAuthority)
		{
			this.superInfection.siManager.CallRPC(SuperInfectionManager.ClientToAuthorityRPC.CombinedTerminalHandScan, new object[]
			{
				this.index
			});
			return;
		}
		SIPlayer x = SIPlayer.Get(actorNr);
		if (this.activePlayer != null && this.activePlayer.isActiveAndEnabled && x != this.activePlayer && this.activeUserBounds.bounds.Contains(this.activePlayer.transform.position))
		{
			return;
		}
		this.activePlayer = x;
		this.dispenser.PlayerHandScanned(actorNr);
		this.techTree.PlayerHandScanned(actorNr);
		this.resourceCollection.PlayerHandScanned(actorNr);
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x0002AB7C File Offset: 0x00028D7C
	public void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr, SICombinedTerminal.TerminalSubFunction subFunction)
	{
		if (!this.IsAuthority)
		{
			this.SIManager.CallRPC(SuperInfectionManager.ClientToAuthorityRPC.CombinedTerminalButtonPress, new object[]
			{
				(int)buttonType,
				data,
				(int)subFunction,
				this.index
			});
			return;
		}
		switch (subFunction)
		{
		case SICombinedTerminal.TerminalSubFunction.TechTree:
			this.techTree.TouchscreenButtonPressed(buttonType, data, actorNr);
			return;
		case SICombinedTerminal.TerminalSubFunction.GadgetDispenser:
			this.dispenser.TouchscreenButtonPressed(buttonType, data, actorNr);
			return;
		case SICombinedTerminal.TerminalSubFunction.ResourceCollection:
			this.resourceCollection.TouchscreenButtonPressed(buttonType, data, actorNr);
			return;
		default:
			return;
		}
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x0002AC0E File Offset: 0x00028E0E
	public void SetActivePage(int pageId)
	{
		this._activePage = pageId;
		if (this.techTree.IsValidPage(pageId))
		{
			this.techTree.SetActivePage();
		}
		if (this.dispenser.IsValidPage(pageId))
		{
			this.dispenser.SetActivePage();
		}
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x0002AC4C File Offset: 0x00028E4C
	private void AnimQueueState(EKioskAnimState newState)
	{
		this.state = newState;
		for (int i = 0; i < this.m_gtAnimators.Length; i++)
		{
			if (!(this.m_gtAnimators[i] == null))
			{
				this.m_gtAnimators[i].QueueState((long)newState);
			}
		}
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x0002AC92 File Offset: 0x00028E92
	public void PlayWrongPlayerBuzz(Transform xForm)
	{
		this.wrongPlayerBuzz.transform.position = xForm.position;
		this.wrongPlayerBuzz.PlayOneShot(this.wrongPlayerBuzz.clip);
	}

	// Token: 0x040009D3 RID: 2515
	[DebugReadout]
	internal int index;

	// Token: 0x040009D4 RID: 2516
	[DebugReadout]
	internal SIPlayer activePlayer;

	// Token: 0x040009D5 RID: 2517
	[DebugReadout]
	internal bool isOccupiedByActivePlayer;

	// Token: 0x040009D6 RID: 2518
	[DebugReadout]
	internal bool isOccupiedByLocalPlayer;

	// Token: 0x040009D7 RID: 2519
	[DebugReadout]
	internal bool isOccupied;

	// Token: 0x040009D8 RID: 2520
	[DebugReadout]
	internal bool wasOccupied;

	// Token: 0x040009D9 RID: 2521
	[DebugReadout]
	internal SuperInfection superInfection;

	// Token: 0x040009DA RID: 2522
	public SIGadgetDispenser dispenser;

	// Token: 0x040009DB RID: 2523
	public SITechTreeStation techTree;

	// Token: 0x040009DC RID: 2524
	public SIResourceCollection resourceCollection;

	// Token: 0x040009DD RID: 2525
	[SerializeField]
	private GTAnimator[] m_gtAnimators;

	// Token: 0x040009DE RID: 2526
	public Collider activeUserBounds;

	// Token: 0x040009DF RID: 2527
	public float foldupDelay = 20f;

	// Token: 0x040009E0 RID: 2528
	private float foldupTimeStart;

	// Token: 0x040009E1 RID: 2529
	private EKioskAnimState state;

	// Token: 0x040009E2 RID: 2530
	[DebugReadout]
	private int _activePage;

	// Token: 0x040009E3 RID: 2531
	[Header("Flattener")]
	public Transform zeroZeroImage;

	// Token: 0x040009E4 RID: 2532
	public Transform onePointTwoText;

	// Token: 0x040009E5 RID: 2533
	private List<VRRig> rigs = new List<VRRig>();

	// Token: 0x040009E6 RID: 2534
	public AudioSource wrongPlayerBuzz;

	// Token: 0x02000139 RID: 313
	public enum TerminalSubFunction
	{
		// Token: 0x040009E8 RID: 2536
		TechTree,
		// Token: 0x040009E9 RID: 2537
		GadgetDispenser,
		// Token: 0x040009EA RID: 2538
		ResourceCollection
	}
}
