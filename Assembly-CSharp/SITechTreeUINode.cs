using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200016E RID: 366
public class SITechTreeUINode : MonoBehaviour
{
	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x060009B0 RID: 2480 RVA: 0x000340E4 File Offset: 0x000322E4
	public List<Image> UpgradeLines { get; } = new List<Image>();

	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x060009B1 RID: 2481 RVA: 0x000340EC File Offset: 0x000322EC
	public List<SITechTreeUINode> Parents { get; } = new List<SITechTreeUINode>();

	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x060009B2 RID: 2482 RVA: 0x000340F4 File Offset: 0x000322F4
	public List<SITechTreeUINode> Children { get; } = new List<SITechTreeUINode>();

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x060009B3 RID: 2483 RVA: 0x000340FC File Offset: 0x000322FC
	public bool IsConfigured
	{
		get
		{
			return this._node != null;
		}
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x00034108 File Offset: 0x00032308
	public void SetTechTreeNode(SITechTreeStation techTreeStation, SIUpgradeType nodeUpgradeType)
	{
		if (!techTreeStation.techTreeSO.TryGetNode(nodeUpgradeType, out this._node))
		{
			Debug.LogError(string.Format("Node {0} doesn't exist in tree.  Disabling.", nodeUpgradeType));
			base.gameObject.SetActive(false);
			return;
		}
		this.upgradeType = nodeUpgradeType;
		float num = (float)(Mathf.Min(this.GetMaxWordLength(this._node.Value.nickName), 14) * 4);
		Vector2 sizeDelta = this.nodeNickName.rectTransform.sizeDelta;
		if (sizeDelta.x < num)
		{
			sizeDelta.x = num;
			this.nodeNickName.rectTransform.sizeDelta = sizeDelta;
		}
		base.name = (this.nodeNickName.text = this._node.Value.nickName);
		this.button.data = this._node.Value.upgradeType.GetNodeId();
		this.button.buttonPressed.RemoveAllListeners();
		this.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(techTreeStation.TouchscreenButtonPressed));
		this.SetGadgetUnlockNode(this._node.Value.unlockedGadgetPrefab);
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x00034234 File Offset: 0x00032434
	public void SetNodeLockStateColor(Color color)
	{
		if (color == Color.red)
		{
			this.circle.sharedMaterial = this.redMat;
		}
		else if (color == Color.black)
		{
			this.circle.sharedMaterial = this.blackMat;
		}
		else if (color == Color.green)
		{
			this.circle.sharedMaterial = this.greenMat;
		}
		foreach (Image image in this.UpgradeLines)
		{
			image.color = color;
		}
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x000342E4 File Offset: 0x000324E4
	private void SetGadgetUnlockNode(bool isUnlockNode)
	{
		this.triangle.gameObject.SetActive(isUnlockNode);
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x000342F8 File Offset: 0x000324F8
	private int GetMaxWordLength(string text)
	{
		string[] array = text.Split(' ', StringSplitOptions.None);
		int num = 0;
		foreach (string text2 in array)
		{
			if (text2.Length > num)
			{
				num = text2.Length;
			}
		}
		return num;
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x00034334 File Offset: 0x00032534
	public void AdjustPosition(Vector3 positionOffset)
	{
		base.transform.localPosition += positionOffset;
		foreach (SITechTreeUINode sitechTreeUINode in this.Children)
		{
			sitechTreeUINode.AdjustPosition(positionOffset);
		}
	}

	// Token: 0x04000BCE RID: 3022
	public SIUpgradeType upgradeType;

	// Token: 0x04000BCF RID: 3023
	public TextMeshProUGUI nodeNickName;

	// Token: 0x04000BD0 RID: 3024
	public MeshRenderer circle;

	// Token: 0x04000BD1 RID: 3025
	public MeshRenderer triangle;

	// Token: 0x04000BD2 RID: 3026
	public SITouchscreenButton button;

	// Token: 0x04000BD3 RID: 3027
	public Material greenMat;

	// Token: 0x04000BD4 RID: 3028
	public Material redMat;

	// Token: 0x04000BD5 RID: 3029
	public Material blackMat;

	// Token: 0x04000BD6 RID: 3030
	public ObjectHierarchyFlattener imageFlattener;

	// Token: 0x04000BD7 RID: 3031
	public ObjectHierarchyFlattener textFlattener;

	// Token: 0x04000BDB RID: 3035
	private GraphNode<SITechTreeNode> _node;
}
