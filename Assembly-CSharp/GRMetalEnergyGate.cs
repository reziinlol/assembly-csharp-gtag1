using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007AB RID: 1963
public class GRMetalEnergyGate : MonoBehaviour
{
	// Token: 0x06003216 RID: 12822 RVA: 0x00113273 File Offset: 0x00111473
	private void OnEnable()
	{
		this.tool.OnEnergyChange += this.OnEnergyChange;
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x06003217 RID: 12823 RVA: 0x001132A4 File Offset: 0x001114A4
	private void OnDisable()
	{
		if (this.tool != null)
		{
			this.tool.OnEnergyChange -= this.OnEnergyChange;
		}
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged -= this.OnEntityStateChanged;
		}
	}

	// Token: 0x06003218 RID: 12824 RVA: 0x001132FC File Offset: 0x001114FC
	private void OnEnergyChange(GRTool tool, int energyChange, GameEntityId chargingEntityId)
	{
		GameEntity gameEntity = this.gameEntity.manager.GetGameEntity(chargingEntityId);
		GRPlayer grplayer = null;
		if (gameEntity != null)
		{
			grplayer = GRPlayer.Get(gameEntity.heldByActorNumber);
		}
		if (grplayer != null)
		{
			grplayer.IncrementCoresSpentPlayer(energyChange);
		}
		if (this.state == GRMetalEnergyGate.State.Closed && tool.energy >= tool.GetEnergyMax())
		{
			if (grplayer != null)
			{
				grplayer.IncrementGatesUnlocked(1);
			}
			this.SetState(GRMetalEnergyGate.State.Open);
			if (this.gameEntity.IsAuthority())
			{
				this.gameEntity.RequestState(this.gameEntity.id, 1L);
			}
		}
	}

	// Token: 0x06003219 RID: 12825 RVA: 0x00113394 File Offset: 0x00111594
	private void OnEntityStateChanged(long prevState, long nextState)
	{
		if (!this.gameEntity.IsAuthority())
		{
			this.SetState((GRMetalEnergyGate.State)nextState);
		}
	}

	// Token: 0x0600321A RID: 12826 RVA: 0x001133AC File Offset: 0x001115AC
	public void SetState(GRMetalEnergyGate.State newState)
	{
		if (this.state != newState)
		{
			this.state = newState;
			GRMetalEnergyGate.State state = this.state;
			if (state != GRMetalEnergyGate.State.Closed)
			{
				if (state == GRMetalEnergyGate.State.Open)
				{
					this.audioSource.PlayOneShot(this.doorOpenClip);
					for (int i = 0; i < this.enableObjectsOnOpen.Count; i++)
					{
						this.enableObjectsOnOpen[i].gameObject.SetActive(true);
					}
					for (int j = 0; j < this.disableObjectsOnOpen.Count; j++)
					{
						this.disableObjectsOnOpen[j].gameObject.SetActive(false);
					}
				}
			}
			else
			{
				this.audioSource.PlayOneShot(this.doorCloseClip);
				for (int k = 0; k < this.enableObjectsOnOpen.Count; k++)
				{
					this.enableObjectsOnOpen[k].gameObject.SetActive(false);
				}
				for (int l = 0; l < this.disableObjectsOnOpen.Count; l++)
				{
					this.disableObjectsOnOpen[l].gameObject.SetActive(true);
				}
			}
			if (this.doorAnimationCoroutine == null)
			{
				this.doorAnimationCoroutine = base.StartCoroutine(this.UpdateDoorAnimation());
			}
		}
	}

	// Token: 0x0600321B RID: 12827 RVA: 0x001134D4 File Offset: 0x001116D4
	public void OpenGate()
	{
		this.SetState(GRMetalEnergyGate.State.Open);
	}

	// Token: 0x0600321C RID: 12828 RVA: 0x001134DD File Offset: 0x001116DD
	public void CloseGate()
	{
		this.SetState(GRMetalEnergyGate.State.Closed);
	}

	// Token: 0x0600321D RID: 12829 RVA: 0x001134E6 File Offset: 0x001116E6
	private IEnumerator UpdateDoorAnimation()
	{
		while ((this.state == GRMetalEnergyGate.State.Open && this.openProgress < 1f) || (this.state == GRMetalEnergyGate.State.Closed && this.openProgress > 0f))
		{
			GRMetalEnergyGate.State state = this.state;
			if (state != GRMetalEnergyGate.State.Closed)
			{
				if (state == GRMetalEnergyGate.State.Open)
				{
					this.openProgress = Mathf.MoveTowards(this.openProgress, 1f, Time.deltaTime / this.doorOpenTime);
					float t = this.doorOpenCurve.Evaluate(this.openProgress);
					this.upperDoor.doorTransform.localPosition = Vector3.Lerp(this.upperDoor.doorClosedPosition.localPosition, this.upperDoor.doorOpenPosition.localPosition, t);
					this.lowerDoor.doorTransform.localPosition = Vector3.Lerp(this.lowerDoor.doorClosedPosition.localPosition, this.lowerDoor.doorOpenPosition.localPosition, t);
				}
			}
			else
			{
				this.openProgress = Mathf.MoveTowards(this.openProgress, 0f, Time.deltaTime / this.doorOpenTime);
				float t2 = this.doorCloseCurve.Evaluate(this.openProgress);
				this.upperDoor.doorTransform.localPosition = Vector3.Lerp(this.upperDoor.doorClosedPosition.localPosition, this.upperDoor.doorOpenPosition.localPosition, t2);
				this.lowerDoor.doorTransform.localPosition = Vector3.Lerp(this.lowerDoor.doorClosedPosition.localPosition, this.lowerDoor.doorOpenPosition.localPosition, t2);
			}
			yield return null;
		}
		this.doorAnimationCoroutine = null;
		yield break;
	}

	// Token: 0x04004106 RID: 16646
	[SerializeField]
	public GRMetalEnergyGate.DoorParams upperDoor;

	// Token: 0x04004107 RID: 16647
	[SerializeField]
	public GRMetalEnergyGate.DoorParams lowerDoor;

	// Token: 0x04004108 RID: 16648
	[SerializeField]
	private float doorOpenTime = 1.5f;

	// Token: 0x04004109 RID: 16649
	[SerializeField]
	private float doorCloseTime = 1.5f;

	// Token: 0x0400410A RID: 16650
	[SerializeField]
	private AnimationCurve doorOpenCurve;

	// Token: 0x0400410B RID: 16651
	[SerializeField]
	private AnimationCurve doorCloseCurve;

	// Token: 0x0400410C RID: 16652
	[SerializeField]
	private AudioClip doorOpenClip;

	// Token: 0x0400410D RID: 16653
	[SerializeField]
	private AudioClip doorCloseClip;

	// Token: 0x0400410E RID: 16654
	[SerializeField]
	private List<Transform> enableObjectsOnOpen = new List<Transform>();

	// Token: 0x0400410F RID: 16655
	[SerializeField]
	private List<Transform> disableObjectsOnOpen = new List<Transform>();

	// Token: 0x04004110 RID: 16656
	[SerializeField]
	private GRTool tool;

	// Token: 0x04004111 RID: 16657
	[SerializeField]
	private GameEntity gameEntity;

	// Token: 0x04004112 RID: 16658
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04004113 RID: 16659
	public GRMetalEnergyGate.State state;

	// Token: 0x04004114 RID: 16660
	private float openProgress;

	// Token: 0x04004115 RID: 16661
	private Coroutine doorAnimationCoroutine;

	// Token: 0x020007AC RID: 1964
	public enum State
	{
		// Token: 0x04004117 RID: 16663
		Closed,
		// Token: 0x04004118 RID: 16664
		Open
	}

	// Token: 0x020007AD RID: 1965
	[Serializable]
	public struct DoorParams
	{
		// Token: 0x04004119 RID: 16665
		public Transform doorTransform;

		// Token: 0x0400411A RID: 16666
		public Transform doorClosedPosition;

		// Token: 0x0400411B RID: 16667
		public Transform doorOpenPosition;
	}
}
