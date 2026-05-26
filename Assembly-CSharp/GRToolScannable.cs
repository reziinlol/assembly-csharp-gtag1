using System;

// Token: 0x0200080A RID: 2058
public class GRToolScannable : GRScannable
{
	// Token: 0x060034C5 RID: 13509 RVA: 0x00122B14 File Offset: 0x00120D14
	public override void Start()
	{
		base.Start();
		if (this.gameEntity != null)
		{
			this.tool = this.gameEntity.GetComponent<GRTool>();
			this.upgradePiece = this.gameEntity.GetComponent<GRToolUpgradePiece>();
		}
	}

	// Token: 0x060034C6 RID: 13510 RVA: 0x00122B4C File Offset: 0x00120D4C
	private void FetchMetadata(GhostReactor reactor)
	{
		if (this.metadata == null)
		{
			GRToolProgressionManager.ToolParts toolParts = GRToolProgressionManager.ToolParts.None;
			if (this.tool != null)
			{
				toolParts = GRUtils.GetToolPart(this.tool.toolType);
			}
			else if (this.upgradePiece != null)
			{
				toolParts = this.upgradePiece.matchingUpgrade;
			}
			if (toolParts != GRToolProgressionManager.ToolParts.None)
			{
				this.metadata = reactor.toolProgression.GetPartMetadata(toolParts);
			}
		}
	}

	// Token: 0x060034C7 RID: 13511 RVA: 0x00122BB3 File Offset: 0x00120DB3
	public override string GetTitleText(GhostReactor reactor)
	{
		this.FetchMetadata(reactor);
		if (this.metadata == null)
		{
			return "Unknown";
		}
		return this.metadata.name;
	}

	// Token: 0x060034C8 RID: 13512 RVA: 0x00122BD5 File Offset: 0x00120DD5
	public override string GetBodyText(GhostReactor reactor)
	{
		this.FetchMetadata(reactor);
		if (this.metadata == null)
		{
			return "Unknown";
		}
		return this.metadata.description;
	}

	// Token: 0x060034C9 RID: 13513 RVA: 0x00122BF7 File Offset: 0x00120DF7
	public override string GetAnnotationText(GhostReactor reactor)
	{
		this.FetchMetadata(reactor);
		if (this.metadata == null)
		{
			return "Unknown";
		}
		return this.metadata.annotation;
	}

	// Token: 0x040044F2 RID: 17650
	private GRTool tool;

	// Token: 0x040044F3 RID: 17651
	private GRToolUpgradePiece upgradePiece;

	// Token: 0x040044F4 RID: 17652
	private GRToolProgressionManager.ToolProgressionMetaData metadata;
}
