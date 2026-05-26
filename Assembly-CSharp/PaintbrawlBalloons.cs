using System;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020004B4 RID: 1204
public class PaintbrawlBalloons : MonoBehaviour
{
	// Token: 0x06001D59 RID: 7513 RVA: 0x0009E790 File Offset: 0x0009C990
	protected void Awake()
	{
		this.matPropBlock = new MaterialPropertyBlock();
		this.renderers = new Renderer[this.balloons.Length];
		this.balloonsCachedActiveState = new bool[this.balloons.Length];
		for (int i = 0; i < this.balloons.Length; i++)
		{
			this.renderers[i] = this.balloons[i].GetComponentInChildren<Renderer>();
			this.balloonsCachedActiveState[i] = this.balloons[i].activeSelf;
		}
		this.colorShaderPropID = ShaderProps._Color;
	}

	// Token: 0x06001D5A RID: 7514 RVA: 0x0009E816 File Offset: 0x0009CA16
	protected void OnEnable()
	{
		this.UpdateBalloonColors();
	}

	// Token: 0x06001D5B RID: 7515 RVA: 0x0009E820 File Offset: 0x0009CA20
	protected void LateUpdate()
	{
		if (GorillaGameManager.instance != null && (this.bMgr != null || GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>() != null))
		{
			if (this.bMgr == null)
			{
				this.bMgr = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
			}
			int playerLives = this.bMgr.GetPlayerLives(this.myRig.creator);
			for (int i = 0; i < this.balloons.Length; i++)
			{
				bool flag = playerLives >= i + 1;
				if (flag != this.balloonsCachedActiveState[i])
				{
					this.balloonsCachedActiveState[i] = flag;
					this.balloons[i].SetActive(flag);
					if (!flag)
					{
						this.PopBalloon(i);
					}
				}
			}
		}
		else if (GorillaGameManager.instance != null)
		{
			base.gameObject.SetActive(false);
		}
		this.UpdateBalloonColors();
	}

	// Token: 0x06001D5C RID: 7516 RVA: 0x0009E90C File Offset: 0x0009CB0C
	private void PopBalloon(int i)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(this.balloonPopFXPrefab, true);
		gameObject.transform.position = this.balloons[i].transform.position;
		GorillaColorizableBase componentInChildren = gameObject.GetComponentInChildren<GorillaColorizableBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(this.teamColor);
		}
	}

	// Token: 0x06001D5D RID: 7517 RVA: 0x0009E964 File Offset: 0x0009CB64
	public void UpdateBalloonColors()
	{
		if (this.bMgr != null && this.myRig.creator != null)
		{
			if (this.bMgr.OnRedTeam(this.myRig.creator))
			{
				this.teamColor = this.orangeColor;
			}
			else if (this.bMgr.OnBlueTeam(this.myRig.creator))
			{
				this.teamColor = this.blueColor;
			}
			else
			{
				this.teamColor = (this.myRig ? this.myRig.playerColor : this.defaultColor);
			}
		}
		if (this.teamColor != this.lastColor)
		{
			this.lastColor = this.teamColor;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer)
				{
					foreach (Material material in renderer.materials)
					{
						if (!(material == null))
						{
							if (material.HasProperty(ShaderProps._BaseColor))
							{
								material.SetColor(ShaderProps._BaseColor, this.teamColor);
							}
							if (material.HasProperty(ShaderProps._Color))
							{
								material.SetColor(ShaderProps._Color, this.teamColor);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x04002785 RID: 10117
	public VRRig myRig;

	// Token: 0x04002786 RID: 10118
	public GameObject[] balloons;

	// Token: 0x04002787 RID: 10119
	public Color orangeColor;

	// Token: 0x04002788 RID: 10120
	public Color blueColor;

	// Token: 0x04002789 RID: 10121
	public Color defaultColor;

	// Token: 0x0400278A RID: 10122
	public Color lastColor;

	// Token: 0x0400278B RID: 10123
	public GameObject balloonPopFXPrefab;

	// Token: 0x0400278C RID: 10124
	[HideInInspector]
	public GorillaPaintbrawlManager bMgr;

	// Token: 0x0400278D RID: 10125
	public Player myPlayer;

	// Token: 0x0400278E RID: 10126
	private int colorShaderPropID;

	// Token: 0x0400278F RID: 10127
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x04002790 RID: 10128
	private bool[] balloonsCachedActiveState;

	// Token: 0x04002791 RID: 10129
	private Renderer[] renderers;

	// Token: 0x04002792 RID: 10130
	private Color teamColor;
}
