using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020009F0 RID: 2544
public static class UberShader
{
	// Token: 0x17000609 RID: 1545
	// (get) Token: 0x06004115 RID: 16661 RVA: 0x0015BD29 File Offset: 0x00159F29
	public static Material ReferenceMaterial
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceMaterial;
		}
	}

	// Token: 0x1700060A RID: 1546
	// (get) Token: 0x06004116 RID: 16662 RVA: 0x0015BD35 File Offset: 0x00159F35
	public static Shader ReferenceShader
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceShader;
		}
	}

	// Token: 0x1700060B RID: 1547
	// (get) Token: 0x06004117 RID: 16663 RVA: 0x0015BD41 File Offset: 0x00159F41
	public static Material ReferenceMaterialNonSRP
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceMaterialNonSRP;
		}
	}

	// Token: 0x1700060C RID: 1548
	// (get) Token: 0x06004118 RID: 16664 RVA: 0x0015BD4D File Offset: 0x00159F4D
	public static Shader ReferenceShaderNonSRP
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kReferenceShaderNonSRP;
		}
	}

	// Token: 0x1700060D RID: 1549
	// (get) Token: 0x06004119 RID: 16665 RVA: 0x0015BD59 File Offset: 0x00159F59
	public static UberShaderProperty[] AllProperties
	{
		get
		{
			UberShader.InitDependencies();
			return UberShader.kProperties;
		}
	}

	// Token: 0x0600411A RID: 16666 RVA: 0x0015BD68 File Offset: 0x00159F68
	public static bool IsAnimated(Material m)
	{
		if (m == null)
		{
			return false;
		}
		if ((double)UberShader.UvShiftToggle.GetValue<float>(m) <= 0.5)
		{
			return false;
		}
		Vector2 value = UberShader.UvShiftRate.GetValue<Vector2>(m);
		return value.x > 0f || value.y > 0f;
	}

	// Token: 0x0600411B RID: 16667 RVA: 0x0015BDC3 File Offset: 0x00159FC3
	private static UberShaderProperty GetProperty(int i)
	{
		UberShader.InitDependencies();
		return UberShader.kProperties[i];
	}

	// Token: 0x0600411C RID: 16668 RVA: 0x0015BDC3 File Offset: 0x00159FC3
	private static UberShaderProperty GetProperty(int i, string expectedName)
	{
		UberShader.InitDependencies();
		return UberShader.kProperties[i];
	}

	// Token: 0x0600411D RID: 16669 RVA: 0x0015BDD4 File Offset: 0x00159FD4
	private static void InitDependencies()
	{
		if (UberShader.gInitialized)
		{
			return;
		}
		UberShader.kReferenceShader = Shader.Find("GorillaTag/UberShader");
		UberShader.kReferenceMaterial = new Material(UberShader.kReferenceShader);
		UberShader.kReferenceShaderNonSRP = Shader.Find("GorillaTag/UberShaderNonSRP");
		UberShader.kReferenceMaterialNonSRP = new Material(UberShader.kReferenceShaderNonSRP);
		UberShader.kProperties = UberShader.EnumerateAllProperties(UberShader.kReferenceShader);
		UberShader.gInitialized = true;
	}

	// Token: 0x0600411E RID: 16670 RVA: 0x0015BD35 File Offset: 0x00159F35
	public static Shader GetShader()
	{
		UberShader.InitDependencies();
		return UberShader.kReferenceShader;
	}

	// Token: 0x0600411F RID: 16671 RVA: 0x0015BE3C File Offset: 0x0015A03C
	private static UberShaderProperty[] EnumerateAllProperties(Shader uberShader)
	{
		int propertyCount = uberShader.GetPropertyCount();
		UberShaderProperty[] array = new UberShaderProperty[propertyCount];
		for (int i = 0; i < propertyCount; i++)
		{
			UberShaderProperty uberShaderProperty = new UberShaderProperty
			{
				index = i,
				flags = uberShader.GetPropertyFlags(i),
				type = uberShader.GetPropertyType(i),
				nameID = uberShader.GetPropertyNameId(i),
				name = uberShader.GetPropertyName(i),
				attributes = uberShader.GetPropertyAttributes(i)
			};
			if (uberShaderProperty.type == ShaderPropertyType.Range)
			{
				uberShaderProperty.rangeLimits = uberShader.GetPropertyRangeLimits(uberShaderProperty.index);
			}
			string[] attributes = uberShaderProperty.attributes;
			if (attributes != null && attributes.Length != 0)
			{
				foreach (string text in attributes)
				{
					if (!string.IsNullOrWhiteSpace(text))
					{
						bool flag = text.StartsWith("Toggle(");
						uberShaderProperty.isKeywordToggle = flag;
						if (flag)
						{
							string keyword = text.Split('(', StringSplitOptions.RemoveEmptyEntries)[1].RemoveEnd(")", StringComparison.InvariantCulture);
							uberShaderProperty.keyword = keyword;
						}
					}
				}
			}
			array[i] = uberShaderProperty;
		}
		return array;
	}

	// Token: 0x040051C3 RID: 20931
	private static Shader kReferenceShader;

	// Token: 0x040051C4 RID: 20932
	private static Material kReferenceMaterial;

	// Token: 0x040051C5 RID: 20933
	private static Shader kReferenceShaderNonSRP;

	// Token: 0x040051C6 RID: 20934
	private static Material kReferenceMaterialNonSRP;

	// Token: 0x040051C7 RID: 20935
	private static UberShaderProperty[] kProperties;

	// Token: 0x040051C8 RID: 20936
	private static bool gInitialized = false;

	// Token: 0x040051C9 RID: 20937
	public static UberShaderProperty TransparencyMode = UberShader.GetProperty(0);

	// Token: 0x040051CA RID: 20938
	public static UberShaderProperty Cutoff = UberShader.GetProperty(1);

	// Token: 0x040051CB RID: 20939
	public static UberShaderProperty ColorSource = UberShader.GetProperty(2);

	// Token: 0x040051CC RID: 20940
	public static UberShaderProperty BaseColor = UberShader.GetProperty(3);

	// Token: 0x040051CD RID: 20941
	public static UberShaderProperty GChannelColor = UberShader.GetProperty(4);

	// Token: 0x040051CE RID: 20942
	public static UberShaderProperty BChannelColor = UberShader.GetProperty(5);

	// Token: 0x040051CF RID: 20943
	public static UberShaderProperty AChannelColor = UberShader.GetProperty(6);

	// Token: 0x040051D0 RID: 20944
	public static UberShaderProperty BaseMap = UberShader.GetProperty(7);

	// Token: 0x040051D1 RID: 20945
	public static UberShaderProperty BaseMap_WH = UberShader.GetProperty(8);

	// Token: 0x040051D2 RID: 20946
	public static UberShaderProperty TexelSnapToggle = UberShader.GetProperty(9);

	// Token: 0x040051D3 RID: 20947
	public static UberShaderProperty TexelSnap_Factor = UberShader.GetProperty(10);

	// Token: 0x040051D4 RID: 20948
	public static UberShaderProperty UVSource = UberShader.GetProperty(11);

	// Token: 0x040051D5 RID: 20949
	public static UberShaderProperty AlphaDetailToggle = UberShader.GetProperty(12);

	// Token: 0x040051D6 RID: 20950
	public static UberShaderProperty AlphaDetail_ST = UberShader.GetProperty(13);

	// Token: 0x040051D7 RID: 20951
	public static UberShaderProperty AlphaDetail_Opacity = UberShader.GetProperty(14);

	// Token: 0x040051D8 RID: 20952
	public static UberShaderProperty AlphaDetail_WorldSpace = UberShader.GetProperty(15);

	// Token: 0x040051D9 RID: 20953
	public static UberShaderProperty MaskMapToggle = UberShader.GetProperty(16);

	// Token: 0x040051DA RID: 20954
	public static UberShaderProperty MaskMap = UberShader.GetProperty(17);

	// Token: 0x040051DB RID: 20955
	public static UberShaderProperty MaskMap_WH = UberShader.GetProperty(18);

	// Token: 0x040051DC RID: 20956
	public static UberShaderProperty LavaLampToggle = UberShader.GetProperty(19);

	// Token: 0x040051DD RID: 20957
	public static UberShaderProperty GradientMapToggle = UberShader.GetProperty(20);

	// Token: 0x040051DE RID: 20958
	public static UberShaderProperty GradientMap = UberShader.GetProperty(21);

	// Token: 0x040051DF RID: 20959
	public static UberShaderProperty DoTextureRotation = UberShader.GetProperty(22);

	// Token: 0x040051E0 RID: 20960
	public static UberShaderProperty RotateAngle = UberShader.GetProperty(23);

	// Token: 0x040051E1 RID: 20961
	public static UberShaderProperty RotateAnim = UberShader.GetProperty(24);

	// Token: 0x040051E2 RID: 20962
	public static UberShaderProperty UseWaveWarp = UberShader.GetProperty(25);

	// Token: 0x040051E3 RID: 20963
	public static UberShaderProperty WaveAmplitude = UberShader.GetProperty(26);

	// Token: 0x040051E4 RID: 20964
	public static UberShaderProperty WaveFrequency = UberShader.GetProperty(27);

	// Token: 0x040051E5 RID: 20965
	public static UberShaderProperty WaveScale = UberShader.GetProperty(28);

	// Token: 0x040051E6 RID: 20966
	public static UberShaderProperty WaveTimeScale = UberShader.GetProperty(29);

	// Token: 0x040051E7 RID: 20967
	public static UberShaderProperty UseWeatherMap = UberShader.GetProperty(30);

	// Token: 0x040051E8 RID: 20968
	public static UberShaderProperty WeatherMap = UberShader.GetProperty(31);

	// Token: 0x040051E9 RID: 20969
	public static UberShaderProperty WeatherMapDissolveEdgeSize = UberShader.GetProperty(32);

	// Token: 0x040051EA RID: 20970
	public static UberShaderProperty ReflectToggle = UberShader.GetProperty(33);

	// Token: 0x040051EB RID: 20971
	public static UberShaderProperty ReflectBoxProjectToggle = UberShader.GetProperty(34);

	// Token: 0x040051EC RID: 20972
	public static UberShaderProperty ReflectBoxCubePos = UberShader.GetProperty(35);

	// Token: 0x040051ED RID: 20973
	public static UberShaderProperty ReflectBoxSize = UberShader.GetProperty(36);

	// Token: 0x040051EE RID: 20974
	public static UberShaderProperty ReflectBoxRotation = UberShader.GetProperty(37);

	// Token: 0x040051EF RID: 20975
	public static UberShaderProperty ReflectMatcapToggle = UberShader.GetProperty(38);

	// Token: 0x040051F0 RID: 20976
	public static UberShaderProperty ReflectMatcapPerspToggle = UberShader.GetProperty(39);

	// Token: 0x040051F1 RID: 20977
	public static UberShaderProperty ReflectNormalToggle = UberShader.GetProperty(40);

	// Token: 0x040051F2 RID: 20978
	public static UberShaderProperty ReflectTex = UberShader.GetProperty(41);

	// Token: 0x040051F3 RID: 20979
	public static UberShaderProperty ReflectNormalTex = UberShader.GetProperty(42);

	// Token: 0x040051F4 RID: 20980
	public static UberShaderProperty ReflectAlbedoTint = UberShader.GetProperty(43);

	// Token: 0x040051F5 RID: 20981
	public static UberShaderProperty ReflectTint = UberShader.GetProperty(44);

	// Token: 0x040051F6 RID: 20982
	public static UberShaderProperty ReflectOpacity = UberShader.GetProperty(45);

	// Token: 0x040051F7 RID: 20983
	public static UberShaderProperty ReflectExposure = UberShader.GetProperty(46);

	// Token: 0x040051F8 RID: 20984
	public static UberShaderProperty ReflectOffset = UberShader.GetProperty(47);

	// Token: 0x040051F9 RID: 20985
	public static UberShaderProperty ReflectScale = UberShader.GetProperty(48);

	// Token: 0x040051FA RID: 20986
	public static UberShaderProperty ReflectRotate = UberShader.GetProperty(49);

	// Token: 0x040051FB RID: 20987
	public static UberShaderProperty HalfLambertToggle = UberShader.GetProperty(50);

	// Token: 0x040051FC RID: 20988
	public static UberShaderProperty ZFightOffset = UberShader.GetProperty(51);

	// Token: 0x040051FD RID: 20989
	public static UberShaderProperty ParallaxPlanarToggle = UberShader.GetProperty(52);

	// Token: 0x040051FE RID: 20990
	public static UberShaderProperty ParallaxToggle = UberShader.GetProperty(53);

	// Token: 0x040051FF RID: 20991
	public static UberShaderProperty ParallaxAAToggle = UberShader.GetProperty(54);

	// Token: 0x04005200 RID: 20992
	public static UberShaderProperty ParallaxAABias = UberShader.GetProperty(55);

	// Token: 0x04005201 RID: 20993
	public static UberShaderProperty DepthMap = UberShader.GetProperty(56);

	// Token: 0x04005202 RID: 20994
	public static UberShaderProperty ParallaxAmplitude = UberShader.GetProperty(57);

	// Token: 0x04005203 RID: 20995
	public static UberShaderProperty ParallaxSamplesMinMax = UberShader.GetProperty(58);

	// Token: 0x04005204 RID: 20996
	public static UberShaderProperty UvShiftToggle = UberShader.GetProperty(59);

	// Token: 0x04005205 RID: 20997
	public static UberShaderProperty UvShiftSteps = UberShader.GetProperty(60);

	// Token: 0x04005206 RID: 20998
	public static UberShaderProperty UvShiftRate = UberShader.GetProperty(61);

	// Token: 0x04005207 RID: 20999
	public static UberShaderProperty UvShiftOffset = UberShader.GetProperty(62);

	// Token: 0x04005208 RID: 21000
	public static UberShaderProperty UseGridEffect = UberShader.GetProperty(63);

	// Token: 0x04005209 RID: 21001
	public static UberShaderProperty UseCrystalEffect = UberShader.GetProperty(64);

	// Token: 0x0400520A RID: 21002
	public static UberShaderProperty CrystalPower = UberShader.GetProperty(65);

	// Token: 0x0400520B RID: 21003
	public static UberShaderProperty CrystalRimColor = UberShader.GetProperty(66);

	// Token: 0x0400520C RID: 21004
	public static UberShaderProperty LiquidVolume = UberShader.GetProperty(67);

	// Token: 0x0400520D RID: 21005
	public static UberShaderProperty LiquidFill = UberShader.GetProperty(68);

	// Token: 0x0400520E RID: 21006
	public static UberShaderProperty LiquidFillNormal = UberShader.GetProperty(69);

	// Token: 0x0400520F RID: 21007
	public static UberShaderProperty LiquidSurfaceColor = UberShader.GetProperty(70);

	// Token: 0x04005210 RID: 21008
	public static UberShaderProperty LiquidSwayX = UberShader.GetProperty(71);

	// Token: 0x04005211 RID: 21009
	public static UberShaderProperty LiquidSwayY = UberShader.GetProperty(72);

	// Token: 0x04005212 RID: 21010
	public static UberShaderProperty LiquidContainer = UberShader.GetProperty(73);

	// Token: 0x04005213 RID: 21011
	public static UberShaderProperty LiquidPlanePosition = UberShader.GetProperty(74);

	// Token: 0x04005214 RID: 21012
	public static UberShaderProperty LiquidPlaneNormal = UberShader.GetProperty(75);

	// Token: 0x04005215 RID: 21013
	public static UberShaderProperty VertexFlapToggle = UberShader.GetProperty(76);

	// Token: 0x04005216 RID: 21014
	public static UberShaderProperty VertexFlapAxis = UberShader.GetProperty(77);

	// Token: 0x04005217 RID: 21015
	public static UberShaderProperty VertexFlapDegreesMinMax = UberShader.GetProperty(78);

	// Token: 0x04005218 RID: 21016
	public static UberShaderProperty VertexFlapSpeed = UberShader.GetProperty(79);

	// Token: 0x04005219 RID: 21017
	public static UberShaderProperty VertexFlapPhaseOffset = UberShader.GetProperty(80);

	// Token: 0x0400521A RID: 21018
	public static UberShaderProperty VertexWaveToggle = UberShader.GetProperty(81);

	// Token: 0x0400521B RID: 21019
	public static UberShaderProperty VertexWaveDebug = UberShader.GetProperty(82);

	// Token: 0x0400521C RID: 21020
	public static UberShaderProperty VertexWaveEnd = UberShader.GetProperty(83);

	// Token: 0x0400521D RID: 21021
	public static UberShaderProperty VertexWaveParams = UberShader.GetProperty(84);

	// Token: 0x0400521E RID: 21022
	public static UberShaderProperty VertexWaveFalloff = UberShader.GetProperty(85);

	// Token: 0x0400521F RID: 21023
	public static UberShaderProperty VertexWaveSphereMask = UberShader.GetProperty(86);

	// Token: 0x04005220 RID: 21024
	public static UberShaderProperty VertexWavePhaseOffset = UberShader.GetProperty(87);

	// Token: 0x04005221 RID: 21025
	public static UberShaderProperty VertexWaveAxes = UberShader.GetProperty(88);

	// Token: 0x04005222 RID: 21026
	public static UberShaderProperty VertexRotateToggle = UberShader.GetProperty(89);

	// Token: 0x04005223 RID: 21027
	public static UberShaderProperty VertexRotateAngles = UberShader.GetProperty(90);

	// Token: 0x04005224 RID: 21028
	public static UberShaderProperty VertexRotateAnim = UberShader.GetProperty(91);

	// Token: 0x04005225 RID: 21029
	public static UberShaderProperty VertexLightToggle = UberShader.GetProperty(92);

	// Token: 0x04005226 RID: 21030
	public static UberShaderProperty InnerGlowOn = UberShader.GetProperty(93);

	// Token: 0x04005227 RID: 21031
	public static UberShaderProperty InnerGlowColor = UberShader.GetProperty(94);

	// Token: 0x04005228 RID: 21032
	public static UberShaderProperty InnerGlowParams = UberShader.GetProperty(95);

	// Token: 0x04005229 RID: 21033
	public static UberShaderProperty InnerGlowTap = UberShader.GetProperty(96);

	// Token: 0x0400522A RID: 21034
	public static UberShaderProperty InnerGlowSine = UberShader.GetProperty(97);

	// Token: 0x0400522B RID: 21035
	public static UberShaderProperty InnerGlowSinePeriod = UberShader.GetProperty(98);

	// Token: 0x0400522C RID: 21036
	public static UberShaderProperty InnerGlowSinePhaseShift = UberShader.GetProperty(99);

	// Token: 0x0400522D RID: 21037
	public static UberShaderProperty StealthEffectOn = UberShader.GetProperty(100);

	// Token: 0x0400522E RID: 21038
	public static UberShaderProperty UseEyeTracking = UberShader.GetProperty(101);

	// Token: 0x0400522F RID: 21039
	public static UberShaderProperty EyeTileOffsetUV = UberShader.GetProperty(102);

	// Token: 0x04005230 RID: 21040
	public static UberShaderProperty EyeOverrideUV = UberShader.GetProperty(103);

	// Token: 0x04005231 RID: 21041
	public static UberShaderProperty EyeOverrideUVTransform = UberShader.GetProperty(104);

	// Token: 0x04005232 RID: 21042
	public static UberShaderProperty UseMouthFlap = UberShader.GetProperty(105);

	// Token: 0x04005233 RID: 21043
	public static UberShaderProperty MouthMap = UberShader.GetProperty(106);

	// Token: 0x04005234 RID: 21044
	public static UberShaderProperty MouthMap_Atlas = UberShader.GetProperty(107);

	// Token: 0x04005235 RID: 21045
	public static UberShaderProperty MouthMap_AtlasSlice = UberShader.GetProperty(108);

	// Token: 0x04005236 RID: 21046
	public static UberShaderProperty UseVertexColor = UberShader.GetProperty(109);

	// Token: 0x04005237 RID: 21047
	public static UberShaderProperty WaterEffect = UberShader.GetProperty(110);

	// Token: 0x04005238 RID: 21048
	public static UberShaderProperty HeightBasedWaterEffect = UberShader.GetProperty(111);

	// Token: 0x04005239 RID: 21049
	public static UberShaderProperty UseDayNightLightmap = UberShader.GetProperty(112);

	// Token: 0x0400523A RID: 21050
	public static UberShaderProperty UseSpecular = UberShader.GetProperty(113);

	// Token: 0x0400523B RID: 21051
	public static UberShaderProperty UseSpecularAlphaChannel = UberShader.GetProperty(114);

	// Token: 0x0400523C RID: 21052
	public static UberShaderProperty Smoothness = UberShader.GetProperty(115);

	// Token: 0x0400523D RID: 21053
	public static UberShaderProperty UseSpecHighlight = UberShader.GetProperty(116);

	// Token: 0x0400523E RID: 21054
	public static UberShaderProperty SpecularDir = UberShader.GetProperty(117);

	// Token: 0x0400523F RID: 21055
	public static UberShaderProperty SpecularPowerIntensity = UberShader.GetProperty(118);

	// Token: 0x04005240 RID: 21056
	public static UberShaderProperty SpecularColor = UberShader.GetProperty(119);

	// Token: 0x04005241 RID: 21057
	public static UberShaderProperty SpecularUseDiffuseColor = UberShader.GetProperty(120);

	// Token: 0x04005242 RID: 21058
	public static UberShaderProperty EmissionToggle = UberShader.GetProperty(121);

	// Token: 0x04005243 RID: 21059
	public static UberShaderProperty EmissionColor = UberShader.GetProperty(122);

	// Token: 0x04005244 RID: 21060
	public static UberShaderProperty EmissionMap = UberShader.GetProperty(123);

	// Token: 0x04005245 RID: 21061
	public static UberShaderProperty EmissionMaskByBaseMapAlpha = UberShader.GetProperty(124);

	// Token: 0x04005246 RID: 21062
	public static UberShaderProperty EmissionUVScrollSpeed = UberShader.GetProperty(125);

	// Token: 0x04005247 RID: 21063
	public static UberShaderProperty EmissionDissolveProgress = UberShader.GetProperty(126);

	// Token: 0x04005248 RID: 21064
	public static UberShaderProperty EmissionDissolveAnimation = UberShader.GetProperty(127);

	// Token: 0x04005249 RID: 21065
	public static UberShaderProperty EmissionDissolveEdgeSize = UberShader.GetProperty(128);

	// Token: 0x0400524A RID: 21066
	public static UberShaderProperty EmissionUseUVWaveWarp = UberShader.GetProperty(129);

	// Token: 0x0400524B RID: 21067
	public static UberShaderProperty GreyZoneException = UberShader.GetProperty(130);

	// Token: 0x0400524C RID: 21068
	public static UberShaderProperty Cull = UberShader.GetProperty(131);

	// Token: 0x0400524D RID: 21069
	public static UberShaderProperty StencilReference = UberShader.GetProperty(132);

	// Token: 0x0400524E RID: 21070
	public static UberShaderProperty StencilComparison = UberShader.GetProperty(133);

	// Token: 0x0400524F RID: 21071
	public static UberShaderProperty StencilPassFront = UberShader.GetProperty(134);

	// Token: 0x04005250 RID: 21072
	public static UberShaderProperty USE_DEFORM_MAP = UberShader.GetProperty(135);

	// Token: 0x04005251 RID: 21073
	public static UberShaderProperty DeformMap = UberShader.GetProperty(136);

	// Token: 0x04005252 RID: 21074
	public static UberShaderProperty DeformMapIntensity = UberShader.GetProperty(137);

	// Token: 0x04005253 RID: 21075
	public static UberShaderProperty DeformMapMaskByVertColorRAmount = UberShader.GetProperty(138);

	// Token: 0x04005254 RID: 21076
	public static UberShaderProperty DeformMapScrollSpeed = UberShader.GetProperty(139);

	// Token: 0x04005255 RID: 21077
	public static UberShaderProperty DeformMapUV0Influence = UberShader.GetProperty(140);

	// Token: 0x04005256 RID: 21078
	public static UberShaderProperty DeformMapObjectSpaceOffsetsU = UberShader.GetProperty(141);

	// Token: 0x04005257 RID: 21079
	public static UberShaderProperty DeformMapObjectSpaceOffsetsV = UberShader.GetProperty(142);

	// Token: 0x04005258 RID: 21080
	public static UberShaderProperty DeformMapWorldSpaceOffsetsU = UberShader.GetProperty(143);

	// Token: 0x04005259 RID: 21081
	public static UberShaderProperty DeformMapWorldSpaceOffsetsV = UberShader.GetProperty(144);

	// Token: 0x0400525A RID: 21082
	public static UberShaderProperty RotateOnYAxisBySinTime = UberShader.GetProperty(145);

	// Token: 0x0400525B RID: 21083
	public static UberShaderProperty USE_TEX_ARRAY_ATLAS = UberShader.GetProperty(146);

	// Token: 0x0400525C RID: 21084
	public static UberShaderProperty BaseMap_Atlas = UberShader.GetProperty(147);

	// Token: 0x0400525D RID: 21085
	public static UberShaderProperty BaseMap_AtlasSlice = UberShader.GetProperty(148);

	// Token: 0x0400525E RID: 21086
	public static UberShaderProperty EmissionMap_Atlas = UberShader.GetProperty(149);

	// Token: 0x0400525F RID: 21087
	public static UberShaderProperty EmissionMap_AtlasSlice = UberShader.GetProperty(150);

	// Token: 0x04005260 RID: 21088
	public static UberShaderProperty DeformMap_Atlas = UberShader.GetProperty(151);

	// Token: 0x04005261 RID: 21089
	public static UberShaderProperty DeformMap_AtlasSlice = UberShader.GetProperty(152);

	// Token: 0x04005262 RID: 21090
	public static UberShaderProperty DEBUG_PAWN_DATA = UberShader.GetProperty(153);

	// Token: 0x04005263 RID: 21091
	public static UberShaderProperty SrcBlend = UberShader.GetProperty(154);

	// Token: 0x04005264 RID: 21092
	public static UberShaderProperty DstBlend = UberShader.GetProperty(155);

	// Token: 0x04005265 RID: 21093
	public static UberShaderProperty SrcBlendAlpha = UberShader.GetProperty(156);

	// Token: 0x04005266 RID: 21094
	public static UberShaderProperty DstBlendAlpha = UberShader.GetProperty(157);

	// Token: 0x04005267 RID: 21095
	public static UberShaderProperty ZWrite = UberShader.GetProperty(158);

	// Token: 0x04005268 RID: 21096
	public static UberShaderProperty AlphaToMask = UberShader.GetProperty(159);

	// Token: 0x04005269 RID: 21097
	public static UberShaderProperty Color = UberShader.GetProperty(160);

	// Token: 0x0400526A RID: 21098
	public static UberShaderProperty Surface = UberShader.GetProperty(161);

	// Token: 0x0400526B RID: 21099
	public static UberShaderProperty Metallic = UberShader.GetProperty(162);

	// Token: 0x0400526C RID: 21100
	public static UberShaderProperty SpecColor = UberShader.GetProperty(163);

	// Token: 0x0400526D RID: 21101
	public static UberShaderProperty DayNightLightmapArray = UberShader.GetProperty(164);

	// Token: 0x0400526E RID: 21102
	public static UberShaderProperty DayNightLightmapArray_AtlasSlice = UberShader.GetProperty(165);

	// Token: 0x0400526F RID: 21103
	public static UberShaderProperty SingleLightmap = UberShader.GetProperty(166);
}
