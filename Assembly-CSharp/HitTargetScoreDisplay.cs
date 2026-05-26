using System;
using System.Collections;
using GorillaTag;
using UnityEngine;

// Token: 0x020004AC RID: 1196
public class HitTargetScoreDisplay : MonoBehaviour
{
	// Token: 0x06001D23 RID: 7459 RVA: 0x0009DAC8 File Offset: 0x0009BCC8
	protected void Awake()
	{
		this.rotateTimeTotal = 180f / (float)this.rotateSpeed;
		this.matPropBlock = new MaterialPropertyBlock();
		this.networkedScore.AddCallback(new Action<int>(this.OnScoreChanged), true);
		this.ResetRotation();
		this.tensOld = 0;
		this.hundredsOld = 0;
		this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[0]);
		this.singlesRend.SetPropertyBlock(this.matPropBlock);
		this.tensRend.SetPropertyBlock(this.matPropBlock);
		this.hundredsRend.SetPropertyBlock(this.matPropBlock);
	}

	// Token: 0x06001D24 RID: 7460 RVA: 0x0009DB6E File Offset: 0x0009BD6E
	private void OnDestroy()
	{
		this.networkedScore.RemoveCallback(new Action<int>(this.OnScoreChanged));
	}

	// Token: 0x06001D25 RID: 7461 RVA: 0x0009DB88 File Offset: 0x0009BD88
	private void ResetRotation()
	{
		Quaternion rotation = base.transform.rotation;
		this.singlesCard.rotation = rotation;
		this.tensCard.rotation = rotation;
		this.hundredsCard.rotation = rotation;
	}

	// Token: 0x06001D26 RID: 7462 RVA: 0x0009DBC5 File Offset: 0x0009BDC5
	private IEnumerator RotatingCo()
	{
		float timeElapsedSinceHit = 0f;
		int singlesPlace = this.currentScore % 10;
		int tensPlace = this.currentScore / 10 % 10;
		bool tensChange = this.tensOld != tensPlace;
		this.tensOld = tensPlace;
		int hundredsPlace = this.currentScore / 100 % 10;
		bool hundredsChange = this.hundredsOld != hundredsPlace;
		this.hundredsOld = hundredsPlace;
		bool digitsChange = true;
		while (timeElapsedSinceHit < this.rotateTimeTotal)
		{
			this.singlesCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
			Vector3 localEulerAngles = this.singlesCard.localEulerAngles;
			localEulerAngles.x = Mathf.Clamp(localEulerAngles.x, 0f, 180f);
			this.singlesCard.localEulerAngles = localEulerAngles;
			if (tensChange)
			{
				this.tensCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
				Vector3 localEulerAngles2 = this.tensCard.localEulerAngles;
				localEulerAngles2.x = Mathf.Clamp(localEulerAngles2.x, 0f, 180f);
				this.tensCard.localEulerAngles = localEulerAngles2;
			}
			if (hundredsChange)
			{
				this.hundredsCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
				Vector3 localEulerAngles3 = this.hundredsCard.localEulerAngles;
				localEulerAngles3.x = Mathf.Clamp(localEulerAngles3.x, 0f, 180f);
				this.hundredsCard.localEulerAngles = localEulerAngles3;
			}
			if (digitsChange && timeElapsedSinceHit >= this.rotateTimeTotal / 2f)
			{
				this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[singlesPlace]);
				this.singlesRend.SetPropertyBlock(this.matPropBlock);
				if (tensChange)
				{
					this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[tensPlace]);
					this.tensRend.SetPropertyBlock(this.matPropBlock);
				}
				if (hundredsChange)
				{
					this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[hundredsPlace]);
					this.hundredsRend.SetPropertyBlock(this.matPropBlock);
				}
				digitsChange = false;
			}
			yield return null;
			timeElapsedSinceHit += Time.deltaTime;
		}
		this.ResetRotation();
		yield break;
		yield break;
	}

	// Token: 0x06001D27 RID: 7463 RVA: 0x0009DBD4 File Offset: 0x0009BDD4
	private void OnScoreChanged(int newScore)
	{
		if (newScore == this.currentScore)
		{
			return;
		}
		if (this.currentRotationCoroutine != null)
		{
			base.StopCoroutine(this.currentRotationCoroutine);
		}
		this.currentScore = newScore;
		if (base.gameObject.activeInHierarchy)
		{
			this.currentRotationCoroutine = base.StartCoroutine(this.RotatingCo());
		}
	}

	// Token: 0x0400274C RID: 10060
	[SerializeField]
	private WatchableIntSO networkedScore;

	// Token: 0x0400274D RID: 10061
	private int currentScore;

	// Token: 0x0400274E RID: 10062
	private int tensOld;

	// Token: 0x0400274F RID: 10063
	private int hundredsOld;

	// Token: 0x04002750 RID: 10064
	private float rotateTimeTotal;

	// Token: 0x04002751 RID: 10065
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x04002752 RID: 10066
	private readonly Vector4[] numberSheet = new Vector4[]
	{
		new Vector4(1f, 1f, 0.8f, -0.5f),
		new Vector4(1f, 1f, 0f, 0f),
		new Vector4(1f, 1f, 0.2f, 0f),
		new Vector4(1f, 1f, 0.4f, 0f),
		new Vector4(1f, 1f, 0.6f, 0f),
		new Vector4(1f, 1f, 0.8f, 0f),
		new Vector4(1f, 1f, 0f, -0.5f),
		new Vector4(1f, 1f, 0.2f, -0.5f),
		new Vector4(1f, 1f, 0.4f, -0.5f),
		new Vector4(1f, 1f, 0.6f, -0.5f)
	};

	// Token: 0x04002753 RID: 10067
	public int rotateSpeed = 180;

	// Token: 0x04002754 RID: 10068
	public Transform singlesCard;

	// Token: 0x04002755 RID: 10069
	public Transform tensCard;

	// Token: 0x04002756 RID: 10070
	public Transform hundredsCard;

	// Token: 0x04002757 RID: 10071
	public Renderer singlesRend;

	// Token: 0x04002758 RID: 10072
	public Renderer tensRend;

	// Token: 0x04002759 RID: 10073
	public Renderer hundredsRend;

	// Token: 0x0400275A RID: 10074
	private Coroutine currentRotationCoroutine;
}
