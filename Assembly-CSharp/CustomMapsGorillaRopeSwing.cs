using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaLocomotion.Swimming;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000A2D RID: 2605
public class CustomMapsGorillaRopeSwing : GorillaRopeSwing
{
	// Token: 0x06004299 RID: 17049 RVA: 0x00163FC6 File Offset: 0x001621C6
	protected override void Awake()
	{
		base.CalculateId(true);
		base.StartCoroutine(this.WaitForRopeLength());
	}

	// Token: 0x0600429A RID: 17050 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void Start()
	{
	}

	// Token: 0x0600429B RID: 17051 RVA: 0x00163FDC File Offset: 0x001621DC
	protected override void OnEnable()
	{
		if (!this.isRopeLengthSet)
		{
			return;
		}
		base.OnEnable();
	}

	// Token: 0x0600429C RID: 17052 RVA: 0x00163FED File Offset: 0x001621ED
	public void SetRopeLength(int length)
	{
		this.ropeLength = length;
		this.isRopeLengthSet = true;
	}

	// Token: 0x0600429D RID: 17053 RVA: 0x00164000 File Offset: 0x00162200
	public void SetRopeProperties(GTObjectPlaceholder placeholder)
	{
		this.ropePlaceholder = placeholder;
		this.ropeLength = this.ropePlaceholder.ropeLength;
		this.ropeBitGenOffset = this.ropePlaceholder.ropeSegmentGenerationOffset;
		this.preExistingSegments = this.ropePlaceholder.ropeSwingSegments;
		this.ropeScale = this.ropePlaceholder.transform.localScale;
		base.transform.localScale = Vector3.one;
		this.isRopeLengthSet = true;
	}

	// Token: 0x0600429E RID: 17054 RVA: 0x00164074 File Offset: 0x00162274
	private IEnumerator WaitForRopeLength()
	{
		while (!this.isRopeLengthSet)
		{
			yield return null;
		}
		this.RopeGeneration();
		base.Awake();
		base.OnEnable();
		base.Start();
		yield break;
	}

	// Token: 0x0600429F RID: 17055 RVA: 0x00164084 File Offset: 0x00162284
	private void RopeGeneration()
	{
		List<Transform> list = new List<Transform>();
		if (this.preExistingSegments != null && this.preExistingSegments.Count > 0)
		{
			for (int i = 0; i < this.preExistingSegments.Count; i++)
			{
				this.preExistingSegments[i].transform.SetParent(base.transform);
				GorillaClimbable gorillaClimbable = this.preExistingSegments[i].AddComponent<GorillaClimbable>();
				gorillaClimbable.snapX = this.snapX;
				gorillaClimbable.snapY = this.snapY;
				gorillaClimbable.snapZ = this.snapZ;
				gorillaClimbable.maxDistanceSnap = this.maxDistanceSnap;
				gorillaClimbable.clip = this.onGrabSFX;
				gorillaClimbable.clipOnFullRelease = this.OnReleaseSFX;
				GorillaRopeSegment gorillaRopeSegment = this.preExistingSegments[i].AddComponent<GorillaRopeSegment>();
				gorillaRopeSegment.swing = this;
				gorillaRopeSegment.boneIndex = this.preExistingSegments[i].boneIndex;
				list.Add(this.preExistingSegments[i].transform);
			}
			base.transform.localScale = this.ropeScale;
			this.ropePlaceholder.transform.localScale = Vector3.one;
		}
		else
		{
			Vector3 vector = Vector3.zero;
			float y = this.prefabRopeBit.GetComponentInChildren<Renderer>().bounds.size.y;
			WaterVolume[] array = Object.FindObjectsByType<WaterVolume>(FindObjectsSortMode.None);
			List<Collider> list2 = new List<Collider>(array.Length);
			WaterVolume[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				foreach (Collider collider in array2[j].volumeColliders)
				{
					if (!(collider == null))
					{
						list2.Add(collider);
					}
				}
			}
			for (int k = 0; k < this.ropeLength + 1; k++)
			{
				bool flag = false;
				if (list2.Count > 0)
				{
					Collider collider2 = list2[0];
					if (collider2 != null)
					{
						Vector3 vector2 = base.transform.position + vector;
						Vector3 point = vector2 + new Vector3(0f, -y, 0f);
						flag = (collider2.bounds.Contains(vector2) || collider2.bounds.Contains(point));
					}
				}
				GameObject gameObject = Object.Instantiate<GameObject>(flag ? this.partiallyUnderwaterPrefab : this.prefabRopeBit, base.transform);
				gameObject.name = string.Format("RopeBone_{0:00}", k);
				gameObject.transform.localPosition = vector;
				gameObject.transform.localRotation = Quaternion.identity;
				vector += new Vector3(0f, -this.ropeBitGenOffset, 0f);
				GorillaRopeSegment component = gameObject.GetComponent<GorillaRopeSegment>();
				component.swing = this;
				component.boneIndex = k;
				list.Add(gameObject.transform);
			}
			list[0].GetComponent<BoxCollider>().center = new Vector3(0f, -0.65f, 0f);
			list[0].GetComponent<BoxCollider>().size = new Vector3(0.3f, 0.65f, 0.3f);
		}
		if (list.Count > 0)
		{
			list.Last<Transform>().gameObject.SetActive(false);
		}
		this.nodes = list.ToArray();
	}

	// Token: 0x04005495 RID: 21653
	[SerializeField]
	private GameObject partiallyUnderwaterPrefab;

	// Token: 0x04005496 RID: 21654
	private bool isRopeLengthSet;

	// Token: 0x04005497 RID: 21655
	private List<RopeSwingSegment> preExistingSegments;

	// Token: 0x04005498 RID: 21656
	private GTObjectPlaceholder ropePlaceholder;

	// Token: 0x04005499 RID: 21657
	private Vector3 ropeScale = Vector3.one;

	// Token: 0x0400549A RID: 21658
	public bool snapX;

	// Token: 0x0400549B RID: 21659
	public bool snapY;

	// Token: 0x0400549C RID: 21660
	public bool snapZ;

	// Token: 0x0400549D RID: 21661
	public float maxDistanceSnap = 0.05f;

	// Token: 0x0400549E RID: 21662
	public AudioClip onGrabSFX;

	// Token: 0x0400549F RID: 21663
	public AudioClip OnReleaseSFX;
}
