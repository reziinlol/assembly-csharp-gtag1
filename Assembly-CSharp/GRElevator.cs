using System;
using System.Collections.Generic;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000760 RID: 1888
public class GRElevator : MonoBehaviour
{
	// Token: 0x06002FD2 RID: 12242 RVA: 0x0010394C File Offset: 0x00101B4C
	private void OnEnable()
	{
		GRElevatorManager.RegisterElevator(this);
		this.ambientAudio.clip = this.ambientLoopClip;
		this.ambientAudio.Play();
	}

	// Token: 0x06002FD3 RID: 12243 RVA: 0x00103970 File Offset: 0x00101B70
	private void OnDisable()
	{
		GRElevatorManager.DeregisterElevator(this);
	}

	// Token: 0x06002FD4 RID: 12244 RVA: 0x00103978 File Offset: 0x00101B78
	private void Awake()
	{
		this.typeButtonDict = new Dictionary<GRElevator.ButtonType, GRElevatorButton>();
		for (int i = 0; i < this.elevatorButtons.Count; i++)
		{
			this.typeButtonDict.TryAdd(this.elevatorButtons[i].buttonType, this.elevatorButtons[i]);
		}
		this.travelDistance = (this.openTargetTop.position - this.closedTargetTop.position).magnitude;
		this.doorOpenSpeed = this.travelDistance / this.openTravelDuration;
		this.doorCloseSpeed = this.travelDistance / this.closeTravelDuration;
		this.state = GRElevator.ElevatorState.DoorClosed;
		this.UpdateLocalState(this.state);
	}

	// Token: 0x06002FD5 RID: 12245 RVA: 0x00103A31 File Offset: 0x00101C31
	public void PressButton(int type)
	{
		GRElevatorManager.ElevatorButtonPressed((GRElevator.ButtonType)type, this.location);
	}

	// Token: 0x06002FD6 RID: 12246 RVA: 0x00103A40 File Offset: 0x00101C40
	public void PressButtonVisuals(GRElevator.ButtonType type)
	{
		GRElevatorButton grelevatorButton;
		if (this.typeButtonDict.TryGetValue(type, out grelevatorButton))
		{
			grelevatorButton.Pressed();
		}
	}

	// Token: 0x06002FD7 RID: 12247 RVA: 0x00103A63 File Offset: 0x00101C63
	public void PlayDing()
	{
		this.ambientAudio.PlayOneShot(this.dingClip);
	}

	// Token: 0x06002FD8 RID: 12248 RVA: 0x00103A76 File Offset: 0x00101C76
	public void PlayButtonPress()
	{
		this.buttonBank.Play();
	}

	// Token: 0x06002FD9 RID: 12249 RVA: 0x00103A84 File Offset: 0x00101C84
	public void PlayElevatorMoving()
	{
		if (this.ambientAudio.isPlaying && this.ambientAudio.clip == this.travellingLoopClip)
		{
			return;
		}
		this.ambientAudio.clip = this.travellingLoopClip;
		this.ambientAudio.loop = true;
		this.ambientAudio.time = 0f;
		this.ambientAudio.Play();
	}

	// Token: 0x06002FDA RID: 12250 RVA: 0x00103AF0 File Offset: 0x00101CF0
	public void PlayElevatorStopped()
	{
		if (this.ambientAudio.isPlaying && this.ambientAudio.clip == this.ambientLoopClip)
		{
			return;
		}
		this.ambientAudio.clip = this.ambientLoopClip;
		this.ambientAudio.loop = true;
		this.ambientAudio.time = 0f;
		this.ambientAudio.Play();
	}

	// Token: 0x06002FDB RID: 12251 RVA: 0x00103B5B File Offset: 0x00101D5B
	public void PlayElevatorMusic(float time = 0f)
	{
		if (this.musicAudio.isPlaying)
		{
			return;
		}
		this.musicAudio.time = time;
		this.musicAudio.Play();
	}

	// Token: 0x06002FDC RID: 12252 RVA: 0x00103B82 File Offset: 0x00101D82
	public void PlayDoorOpenBegin()
	{
		this.doorAudio.clip = this.doorOpenClip;
		this.doorAudio.time = 0f;
		this.doorAudio.Play();
	}

	// Token: 0x06002FDD RID: 12253 RVA: 0x00103BB0 File Offset: 0x00101DB0
	public void PlayDoorCloseBegin()
	{
		this.doorAudio.clip = this.doorCloseClip;
		this.doorAudio.time = 0f;
		this.doorAudio.Play();
	}

	// Token: 0x06002FDE RID: 12254 RVA: 0x00103BDE File Offset: 0x00101DDE
	public void PlayDoorOpenTravel()
	{
		this.doorAudio.time = this.adjustedOffsetTime + this.openBeginDuration;
	}

	// Token: 0x06002FDF RID: 12255 RVA: 0x00103BF8 File Offset: 0x00101DF8
	public void PlayDoorCloseTravel()
	{
		this.doorAudio.time = this.adjustedOffsetTime + this.closeBeginDuration;
	}

	// Token: 0x06002FE0 RID: 12256 RVA: 0x00103C14 File Offset: 0x00101E14
	public bool DoorsFullyClosed()
	{
		return (this.upperDoor.position - this.closedTargetTop.position).sqrMagnitude < 0.0001f;
	}

	// Token: 0x06002FE1 RID: 12257 RVA: 0x00103C4C File Offset: 0x00101E4C
	public bool DoorsFullyOpen()
	{
		return (this.upperDoor.position - this.openTargetTop.position).sqrMagnitude < 0.0001f;
	}

	// Token: 0x06002FE2 RID: 12258 RVA: 0x00103C84 File Offset: 0x00101E84
	public void UpdateLocalState(GRElevator.ElevatorState newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (newState)
		{
		case GRElevator.ElevatorState.DoorBeginClosing:
			if (this.DoorsFullyClosed())
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorClosed);
				return;
			}
			this.doorMoveBeginTime = Time.time;
			this.SetDoorClosedBeginTime();
			this.PlayDoorCloseBegin();
			return;
		case GRElevator.ElevatorState.DoorMovingClosing:
			this.PlayDoorCloseTravel();
			return;
		case GRElevator.ElevatorState.DoorEndClosing:
		case GRElevator.ElevatorState.DoorEndOpening:
			break;
		case GRElevator.ElevatorState.DoorClosed:
			this.upperDoor.position = this.closedTargetTop.position;
			this.lowerDoor.position = this.closedTargetBottom.position;
			return;
		case GRElevator.ElevatorState.DoorBeginOpening:
			if (this.DoorsFullyOpen())
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorOpen);
				return;
			}
			this.doorMoveBeginTime = Time.time;
			this.SetDoorOpenBeginTime();
			this.PlayDoorOpenBegin();
			return;
		case GRElevator.ElevatorState.DoorMovingOpening:
			this.PlayDoorOpenTravel();
			return;
		case GRElevator.ElevatorState.DoorOpen:
			this.upperDoor.position = this.openTargetTop.position;
			this.lowerDoor.position = this.openTargetBottom.position;
			break;
		default:
			return;
		}
	}

	// Token: 0x06002FE3 RID: 12259 RVA: 0x00103D80 File Offset: 0x00101F80
	public void UpdateRemoteState(GRElevator.ElevatorState remoteNewState)
	{
		if (GRElevator.StateIsOpeningState(remoteNewState) && GRElevator.StateIsClosingState(this.state))
		{
			this.UpdateLocalState(GRElevator.ElevatorState.DoorBeginOpening);
			return;
		}
		if (GRElevator.StateIsClosingState(remoteNewState) && GRElevator.StateIsOpeningState(this.state))
		{
			this.UpdateLocalState(GRElevator.ElevatorState.DoorBeginClosing);
		}
	}

	// Token: 0x06002FE4 RID: 12260 RVA: 0x00103DBC File Offset: 0x00101FBC
	public void SetDoorOpenBeginTime()
	{
		float num = (this.travelDistance - (this.upperDoor.position - this.openTargetTop.position).magnitude) / this.travelDistance;
		this.adjustedOffsetTime = num * this.openTravelDuration;
	}

	// Token: 0x06002FE5 RID: 12261 RVA: 0x00103E0C File Offset: 0x0010200C
	public void SetDoorClosedBeginTime()
	{
		float num = (this.travelDistance - (this.upperDoor.position - this.closedTargetTop.position).magnitude) / this.travelDistance;
		this.adjustedOffsetTime = num * this.closeTravelDuration;
	}

	// Token: 0x06002FE6 RID: 12262 RVA: 0x00103E59 File Offset: 0x00102059
	public static bool StateIsOpeningState(GRElevator.ElevatorState checkState)
	{
		return checkState == GRElevator.ElevatorState.DoorMovingOpening || checkState == GRElevator.ElevatorState.DoorBeginOpening || checkState == GRElevator.ElevatorState.DoorEndOpening || checkState == GRElevator.ElevatorState.DoorOpen;
	}

	// Token: 0x06002FE7 RID: 12263 RVA: 0x00103E6D File Offset: 0x0010206D
	public static bool StateIsClosingState(GRElevator.ElevatorState checkState)
	{
		return checkState == GRElevator.ElevatorState.DoorMovingClosing || checkState == GRElevator.ElevatorState.DoorBeginClosing || checkState == GRElevator.ElevatorState.DoorEndClosing || checkState == GRElevator.ElevatorState.DoorClosed;
	}

	// Token: 0x06002FE8 RID: 12264 RVA: 0x00103E80 File Offset: 0x00102080
	public bool DoorIsOpening()
	{
		return GRElevator.StateIsOpeningState(this.state);
	}

	// Token: 0x06002FE9 RID: 12265 RVA: 0x00103E8D File Offset: 0x0010208D
	public bool DoorIsClosing()
	{
		return GRElevator.StateIsClosingState(this.state);
	}

	// Token: 0x06002FEA RID: 12266 RVA: 0x00103E9C File Offset: 0x0010209C
	public void PhysicalElevatorUpdate()
	{
		switch (this.state)
		{
		case GRElevator.ElevatorState.DoorBeginClosing:
			if (Time.time > this.doorMoveBeginTime + this.closeBeginDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorMovingClosing);
			}
			break;
		case GRElevator.ElevatorState.DoorMovingClosing:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.closeBeginDuration + this.closeTravelDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorEndClosing);
			}
			break;
		case GRElevator.ElevatorState.DoorEndClosing:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.closeBeginDuration + this.closeTravelDuration + this.closeEndDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorClosed);
			}
			break;
		case GRElevator.ElevatorState.DoorBeginOpening:
			if (Time.time > this.doorMoveBeginTime + this.openBeginDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorMovingOpening);
			}
			break;
		case GRElevator.ElevatorState.DoorMovingOpening:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.openBeginDuration + this.openTravelDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorEndOpening);
			}
			break;
		case GRElevator.ElevatorState.DoorEndOpening:
			if (Time.time > this.doorMoveBeginTime - this.adjustedOffsetTime + this.openBeginDuration + this.openTravelDuration + this.openEndDuration)
			{
				this.UpdateLocalState(GRElevator.ElevatorState.DoorOpen);
			}
			break;
		}
		GRElevator.ElevatorState elevatorState = this.state;
		Transform transform;
		Transform transform2;
		float num;
		if (elevatorState != GRElevator.ElevatorState.DoorMovingClosing)
		{
			if (elevatorState == GRElevator.ElevatorState.DoorMovingOpening)
			{
				transform = this.openTargetTop;
				transform2 = this.openTargetBottom;
				num = this.doorOpenSpeed;
			}
			else
			{
				transform = this.upperDoor;
				transform2 = this.lowerDoor;
				num = 1f;
			}
		}
		else
		{
			transform = this.closedTargetTop;
			transform2 = this.closedTargetBottom;
			num = this.doorCloseSpeed;
		}
		this.upperDoor.position = Vector3.MoveTowards(this.upperDoor.position, transform.position, Time.deltaTime * num);
		this.lowerDoor.position = Vector3.MoveTowards(this.lowerDoor.position, transform2.position, Time.deltaTime * num);
	}

	// Token: 0x04003D3E RID: 15678
	public GRElevatorManager.ElevatorLocation location;

	// Token: 0x04003D3F RID: 15679
	public Transform upperDoor;

	// Token: 0x04003D40 RID: 15680
	public Transform lowerDoor;

	// Token: 0x04003D41 RID: 15681
	public Transform closedTargetTop;

	// Token: 0x04003D42 RID: 15682
	public Transform closedTargetBottom;

	// Token: 0x04003D43 RID: 15683
	public Transform openTargetTop;

	// Token: 0x04003D44 RID: 15684
	public Transform openTargetBottom;

	// Token: 0x04003D45 RID: 15685
	public TextMeshPro outerText;

	// Token: 0x04003D46 RID: 15686
	public TextMeshPro innerText;

	// Token: 0x04003D47 RID: 15687
	public List<GRElevatorButton> elevatorButtons;

	// Token: 0x04003D48 RID: 15688
	private Dictionary<GRElevator.ButtonType, GRElevatorButton> typeButtonDict;

	// Token: 0x04003D49 RID: 15689
	public GorillaFriendCollider friendCollider;

	// Token: 0x04003D4A RID: 15690
	public GorillaNetworkJoinTrigger joinTrigger;

	// Token: 0x04003D4B RID: 15691
	public SoundBankPlayer buttonBank;

	// Token: 0x04003D4C RID: 15692
	public AudioSource doorAudio;

	// Token: 0x04003D4D RID: 15693
	public AudioSource ambientAudio;

	// Token: 0x04003D4E RID: 15694
	public AudioSource musicAudio;

	// Token: 0x04003D4F RID: 15695
	public AudioClip travellingLoopClip;

	// Token: 0x04003D50 RID: 15696
	public AudioClip ambientLoopClip;

	// Token: 0x04003D51 RID: 15697
	public AudioClip dingClip;

	// Token: 0x04003D52 RID: 15698
	public AudioClip doorOpenClip;

	// Token: 0x04003D53 RID: 15699
	public AudioClip doorCloseClip;

	// Token: 0x04003D54 RID: 15700
	public float adjustedOffsetTime;

	// Token: 0x04003D55 RID: 15701
	public float doorMoveBeginTime;

	// Token: 0x04003D56 RID: 15702
	public float doorOpenSpeed = 0.5f;

	// Token: 0x04003D57 RID: 15703
	public float doorCloseSpeed = 0.5f;

	// Token: 0x04003D58 RID: 15704
	public float closeBeginDuration;

	// Token: 0x04003D59 RID: 15705
	public float closeTravelDuration;

	// Token: 0x04003D5A RID: 15706
	public float closeEndDuration;

	// Token: 0x04003D5B RID: 15707
	public float openBeginDuration;

	// Token: 0x04003D5C RID: 15708
	public float openTravelDuration;

	// Token: 0x04003D5D RID: 15709
	public float openEndDuration;

	// Token: 0x04003D5E RID: 15710
	public float travelDistance;

	// Token: 0x04003D5F RID: 15711
	public GRElevator.ElevatorState state;

	// Token: 0x04003D60 RID: 15712
	public GameObject collidersAndVisuals;

	// Token: 0x04003D61 RID: 15713
	public GameObject videoDisplay;

	// Token: 0x04003D62 RID: 15714
	public AudioSource videoAudio;

	// Token: 0x02000761 RID: 1889
	public enum ElevatorState
	{
		// Token: 0x04003D64 RID: 15716
		DoorBeginClosing,
		// Token: 0x04003D65 RID: 15717
		DoorMovingClosing,
		// Token: 0x04003D66 RID: 15718
		DoorEndClosing,
		// Token: 0x04003D67 RID: 15719
		DoorClosed,
		// Token: 0x04003D68 RID: 15720
		DoorBeginOpening,
		// Token: 0x04003D69 RID: 15721
		DoorMovingOpening,
		// Token: 0x04003D6A RID: 15722
		DoorEndOpening,
		// Token: 0x04003D6B RID: 15723
		DoorOpen,
		// Token: 0x04003D6C RID: 15724
		None
	}

	// Token: 0x02000762 RID: 1890
	[Serializable]
	public enum ButtonType
	{
		// Token: 0x04003D6E RID: 15726
		Stump = 1,
		// Token: 0x04003D6F RID: 15727
		City,
		// Token: 0x04003D70 RID: 15728
		GhostReactor,
		// Token: 0x04003D71 RID: 15729
		Open,
		// Token: 0x04003D72 RID: 15730
		Close,
		// Token: 0x04003D73 RID: 15731
		Summon,
		// Token: 0x04003D74 RID: 15732
		MonkeBlocks,
		// Token: 0x04003D75 RID: 15733
		VIMExperience1,
		// Token: 0x04003D76 RID: 15734
		VIMExperience2,
		// Token: 0x04003D77 RID: 15735
		VIMExperience3,
		// Token: 0x04003D78 RID: 15736
		VIMExperience4,
		// Token: 0x04003D79 RID: 15737
		Count
	}
}
