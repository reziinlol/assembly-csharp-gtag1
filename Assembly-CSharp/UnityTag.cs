using System;

// Token: 0x020003AA RID: 938
public enum UnityTag
{
	// Token: 0x0400210B RID: 8459
	Invalid = -1,
	// Token: 0x0400210C RID: 8460
	Untagged,
	// Token: 0x0400210D RID: 8461
	Respawn,
	// Token: 0x0400210E RID: 8462
	Finish,
	// Token: 0x0400210F RID: 8463
	EditorOnly,
	// Token: 0x04002110 RID: 8464
	MainCamera,
	// Token: 0x04002111 RID: 8465
	Player,
	// Token: 0x04002112 RID: 8466
	GameController,
	// Token: 0x04002113 RID: 8467
	SceneChanger,
	// Token: 0x04002114 RID: 8468
	PlayerOffset,
	// Token: 0x04002115 RID: 8469
	GorillaTagManager,
	// Token: 0x04002116 RID: 8470
	GorillaTagCollider,
	// Token: 0x04002117 RID: 8471
	GorillaPlayer,
	// Token: 0x04002118 RID: 8472
	GorillaObject,
	// Token: 0x04002119 RID: 8473
	GorillaGameManager,
	// Token: 0x0400211A RID: 8474
	GorillaCosmetic,
	// Token: 0x0400211B RID: 8475
	projectile,
	// Token: 0x0400211C RID: 8476
	FxTemporaire,
	// Token: 0x0400211D RID: 8477
	SlingshotProjectile,
	// Token: 0x0400211E RID: 8478
	SlingshotProjectileTrail,
	// Token: 0x0400211F RID: 8479
	SlingshotProjectilePlayerImpactFX,
	// Token: 0x04002120 RID: 8480
	SlingshotProjectileSurfaceImpactFX,
	// Token: 0x04002121 RID: 8481
	BalloonPopFX,
	// Token: 0x04002122 RID: 8482
	WorldShareableItem,
	// Token: 0x04002123 RID: 8483
	HornsSlingshotProjectile,
	// Token: 0x04002124 RID: 8484
	HornsSlingshotProjectileTrail,
	// Token: 0x04002125 RID: 8485
	HornsSlingshotProjectilePlayerImpactFX,
	// Token: 0x04002126 RID: 8486
	HornsSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04002127 RID: 8487
	FryingPan,
	// Token: 0x04002128 RID: 8488
	LeafPileImpactFX,
	// Token: 0x04002129 RID: 8489
	BalloonPopFx,
	// Token: 0x0400212A RID: 8490
	CloudSlingshotProjectile,
	// Token: 0x0400212B RID: 8491
	CloudSlingshotProjectileTrail,
	// Token: 0x0400212C RID: 8492
	CloudSlingshotProjectilePlayerImpactFX,
	// Token: 0x0400212D RID: 8493
	CloudSlingshotProjectileSurfaceImpactFX,
	// Token: 0x0400212E RID: 8494
	SnowballProjectile,
	// Token: 0x0400212F RID: 8495
	SnowballProjectileImpactFX,
	// Token: 0x04002130 RID: 8496
	CupidBowProjectile,
	// Token: 0x04002131 RID: 8497
	CupidBowProjectileTrail,
	// Token: 0x04002132 RID: 8498
	CupidBowProjectileSurfaceImpactFX,
	// Token: 0x04002133 RID: 8499
	NoCrazyCheck,
	// Token: 0x04002134 RID: 8500
	IceSlingshotProjectile,
	// Token: 0x04002135 RID: 8501
	IceSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04002136 RID: 8502
	IceSlingshotProjectileTrail,
	// Token: 0x04002137 RID: 8503
	ElfBowProjectile,
	// Token: 0x04002138 RID: 8504
	ElfBowProjectileSurfaceImpactFX,
	// Token: 0x04002139 RID: 8505
	ElfBowProjectileTrail,
	// Token: 0x0400213A RID: 8506
	RenderIfSmall,
	// Token: 0x0400213B RID: 8507
	DeleteOnNonBetaBuild,
	// Token: 0x0400213C RID: 8508
	DeleteOnNonDebugBuild,
	// Token: 0x0400213D RID: 8509
	FlagColoringCauldon,
	// Token: 0x0400213E RID: 8510
	WaterRippleEffect,
	// Token: 0x0400213F RID: 8511
	WaterSplashEffect,
	// Token: 0x04002140 RID: 8512
	FireworkMortarProjectile,
	// Token: 0x04002141 RID: 8513
	FireworkMortarProjectileImpactFX,
	// Token: 0x04002142 RID: 8514
	WaterBalloonProjectile,
	// Token: 0x04002143 RID: 8515
	WaterBalloonProjectileImpactFX,
	// Token: 0x04002144 RID: 8516
	PlayerHeadTrigger,
	// Token: 0x04002145 RID: 8517
	WizardStaff,
	// Token: 0x04002146 RID: 8518
	LurkerGhost,
	// Token: 0x04002147 RID: 8519
	HauntedObject,
	// Token: 0x04002148 RID: 8520
	WanderingGhost,
	// Token: 0x04002149 RID: 8521
	LavaSurfaceRock,
	// Token: 0x0400214A RID: 8522
	LavaRockProjectile,
	// Token: 0x0400214B RID: 8523
	LavaRockProjectileImpactFX,
	// Token: 0x0400214C RID: 8524
	MoltenSlingshotProjectile,
	// Token: 0x0400214D RID: 8525
	MoltenSlingshotProjectileTrail,
	// Token: 0x0400214E RID: 8526
	MoltenSlingshotProjectileSurfaceImpactFX,
	// Token: 0x0400214F RID: 8527
	MoltenSlingshotProjectilePlayerImpactFX,
	// Token: 0x04002150 RID: 8528
	SpiderBowProjectile,
	// Token: 0x04002151 RID: 8529
	SpiderBowProjectileTrail,
	// Token: 0x04002152 RID: 8530
	SpiderBowProjectileSurfaceImpactFX,
	// Token: 0x04002153 RID: 8531
	SpiderBowProjectilePlayerImpactFX,
	// Token: 0x04002154 RID: 8532
	ZoneRoot,
	// Token: 0x04002155 RID: 8533
	DontProcessMaterials,
	// Token: 0x04002156 RID: 8534
	OrnamentProjectileSurfaceImpactFX,
	// Token: 0x04002157 RID: 8535
	BucketGiftCane,
	// Token: 0x04002158 RID: 8536
	BucketGiftCoal,
	// Token: 0x04002159 RID: 8537
	BucketGiftRoll,
	// Token: 0x0400215A RID: 8538
	BucketGiftRound,
	// Token: 0x0400215B RID: 8539
	BucketGiftSquare,
	// Token: 0x0400215C RID: 8540
	OrnamentProjectile,
	// Token: 0x0400215D RID: 8541
	OrnamentShatterFX,
	// Token: 0x0400215E RID: 8542
	ScienceCandyProjectile,
	// Token: 0x0400215F RID: 8543
	ScienceCandyImpactFX,
	// Token: 0x04002160 RID: 8544
	PaperAirplaneProjectile,
	// Token: 0x04002161 RID: 8545
	DevilBowProjectile,
	// Token: 0x04002162 RID: 8546
	DevilBowProjectileTrail,
	// Token: 0x04002163 RID: 8547
	DevilBowProjectileSurfaceImpactFX,
	// Token: 0x04002164 RID: 8548
	DevilBowProjectilePlayerImpactFX,
	// Token: 0x04002165 RID: 8549
	FireFX,
	// Token: 0x04002166 RID: 8550
	FishFood,
	// Token: 0x04002167 RID: 8551
	FishFoodImpactFX,
	// Token: 0x04002168 RID: 8552
	LeafNinjaStarProjectile,
	// Token: 0x04002169 RID: 8553
	LeafNinjaStarProjectileC1,
	// Token: 0x0400216A RID: 8554
	LeafNinjaStarProjectileC2,
	// Token: 0x0400216B RID: 8555
	SamuraiBowProjectile,
	// Token: 0x0400216C RID: 8556
	SamuraiBowProjectileTrail,
	// Token: 0x0400216D RID: 8557
	SamuraiBowProjectileSurfaceImpactFX,
	// Token: 0x0400216E RID: 8558
	SamuraiBowProjectilePlayerImpactFX,
	// Token: 0x0400216F RID: 8559
	DragonSlingProjectile,
	// Token: 0x04002170 RID: 8560
	DragonSlingProjectileTrail,
	// Token: 0x04002171 RID: 8561
	DragonSlingProjectileSurfaceImpactFX,
	// Token: 0x04002172 RID: 8562
	DragonSlingProjectilePlayerImpactFX,
	// Token: 0x04002173 RID: 8563
	FireballProjectile,
	// Token: 0x04002174 RID: 8564
	StealthHandTapFX,
	// Token: 0x04002175 RID: 8565
	EnvPieceTree01,
	// Token: 0x04002176 RID: 8566
	FxSnapPiecePlaced,
	// Token: 0x04002177 RID: 8567
	FxSnapPieceDisconnected,
	// Token: 0x04002178 RID: 8568
	FxSnapPieceGrabbed,
	// Token: 0x04002179 RID: 8569
	FxSnapPieceLocationLock,
	// Token: 0x0400217A RID: 8570
	CyberNinjaStarProjectile,
	// Token: 0x0400217B RID: 8571
	RoomLight,
	// Token: 0x0400217C RID: 8572
	SamplesInfoPanel,
	// Token: 0x0400217D RID: 8573
	GorillaHandLeft,
	// Token: 0x0400217E RID: 8574
	GorillaHandRight,
	// Token: 0x0400217F RID: 8575
	GorillaHandSocket,
	// Token: 0x04002180 RID: 8576
	PlayingCardProjectile,
	// Token: 0x04002181 RID: 8577
	RottenPumpkinProjectile,
	// Token: 0x04002182 RID: 8578
	FxSnapPieceRecycle,
	// Token: 0x04002183 RID: 8579
	FxSnapPieceDispenser,
	// Token: 0x04002184 RID: 8580
	AppleProjectile,
	// Token: 0x04002185 RID: 8581
	AppleProjectileSurfaceImpactFX,
	// Token: 0x04002186 RID: 8582
	RecyclerForceVolumeFX,
	// Token: 0x04002187 RID: 8583
	FxSnapPieceTooHeavy,
	// Token: 0x04002188 RID: 8584
	FxBuilderPrivatePlotClaimed,
	// Token: 0x04002189 RID: 8585
	TrickTreatCandy,
	// Token: 0x0400218A RID: 8586
	TrickTreatEyeball,
	// Token: 0x0400218B RID: 8587
	TrickTreatBat,
	// Token: 0x0400218C RID: 8588
	TrickTreatBomb,
	// Token: 0x0400218D RID: 8589
	TrickTreatSurfaceImpact,
	// Token: 0x0400218E RID: 8590
	TrickTreatBatImpact,
	// Token: 0x0400218F RID: 8591
	TrickTreatBombImpact,
	// Token: 0x04002190 RID: 8592
	GuardianSlapFX,
	// Token: 0x04002191 RID: 8593
	GuardianSlamFX,
	// Token: 0x04002192 RID: 8594
	GuardianIdolLandedFX,
	// Token: 0x04002193 RID: 8595
	GuardianIdolFallFX,
	// Token: 0x04002194 RID: 8596
	GuardianIdolTappedFX,
	// Token: 0x04002195 RID: 8597
	VotingRockProjectile,
	// Token: 0x04002196 RID: 8598
	LeafPileImpactFXMedium,
	// Token: 0x04002197 RID: 8599
	LeafPileImpactFXSmall,
	// Token: 0x04002198 RID: 8600
	WoodenSword,
	// Token: 0x04002199 RID: 8601
	WoodenShield,
	// Token: 0x0400219A RID: 8602
	FxBuilderShrink,
	// Token: 0x0400219B RID: 8603
	FxBuilderGrow,
	// Token: 0x0400219C RID: 8604
	FxSnapPieceWreathJump,
	// Token: 0x0400219D RID: 8605
	ElfLauncherElf,
	// Token: 0x0400219E RID: 8606
	RubberBandCar,
	// Token: 0x0400219F RID: 8607
	SnowPileImpactFX,
	// Token: 0x040021A0 RID: 8608
	FirecrackersProjectile,
	// Token: 0x040021A1 RID: 8609
	PaperAirplaneSquareProjectile,
	// Token: 0x040021A2 RID: 8610
	SmokeBombProjectile,
	// Token: 0x040021A3 RID: 8611
	ThrowableHeartProjectile,
	// Token: 0x040021A4 RID: 8612
	SunFlowers,
	// Token: 0x040021A5 RID: 8613
	RobotCannonProjectile,
	// Token: 0x040021A6 RID: 8614
	RobotCannonProjectileImpact,
	// Token: 0x040021A7 RID: 8615
	SmokeBombExplosionEffect,
	// Token: 0x040021A8 RID: 8616
	FireCrackerExplosionEffect,
	// Token: 0x040021A9 RID: 8617
	GorillaMouth
}
