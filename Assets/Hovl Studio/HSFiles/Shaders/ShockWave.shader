Shader "Hovl/Particles/ShockWave"
{
	Properties
	{
		[MaterialToggle] _Usedepth ("Use depth?", Float ) = 0
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_MainTexture("Main Texture", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_Flow("Flow", 2D) = "white" {}
		_DistortionSpeedXYPowerZ("Distortion Speed XY Power Z", Vector) = (0,0,0,0)
		_NoiseSpeedXYPowerZ("Noise Speed XY Power Z", Vector) = (0,0,1,0)
		_Emission("Emission", Float) = 2
		_Mask("Mask", 2D) = "white" {}
		_NoiseOpacityLerp("Noise Opacity Lerp", Range( 0 , 1)) = 0
		[Toggle]_UV2Tswitch("UV2Tswitch", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	Category 
	{
		SubShader
		{
		LOD 0
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {		
				CGPROGRAM
				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif		
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"
				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					float4 ase_texcoord1 : TEXCOORD1;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					float4 ase_texcoord3 : TEXCOORD3;
				};
					
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform fixed _Usedepth;
				uniform float _InvFade;
				uniform float _Emission;
				uniform sampler2D _Noise;
				uniform float4 _NoiseSpeedXYPowerZ;
				uniform float4 _Noise_ST;
				uniform float _NoiseOpacityLerp;
				uniform sampler2D _MainTexture;
				uniform sampler2D _Flow;
				uniform float4 _DistortionSpeedXYPowerZ;
				uniform float4 _Flow_ST;
				uniform float _UV2Tswitch;
				uniform float4 _MainTexture_ST;
				uniform sampler2D _Mask;
				uniform float4 _Mask_ST;


				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					o.ase_texcoord3 = v.ase_texcoord1;

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( i );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

					float lp = 1;
					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						lp *= lerp(1, fade, _Usedepth);
						i.color.a *= lp;
					#endif

					float2 appendResult49 = (float2(_NoiseSpeedXYPowerZ.x , _NoiseSpeedXYPowerZ.y));
					float2 uv_Noise = i.texcoord.xy * _Noise_ST.xy + _Noise_ST.zw;
					float2 panner50 = ( 1.0 * _Time.y * appendResult49 + (uv_Noise).xy);
					float4 tex2DNode45 = tex2D( _Noise, panner50 );
					float lerpResult81 = lerp( tex2DNode45.a , 1.0 , _NoiseOpacityLerp);
					float temp_output_66_0 = (( 1.0 - _NoiseSpeedXYPowerZ.z ) + (lerpResult81 - 0.0) * (_NoiseSpeedXYPowerZ.z - ( 1.0 - _NoiseSpeedXYPowerZ.z )) / (1.0 - 0.0));
					float2 appendResult29 = (float2(_DistortionSpeedXYPowerZ.x , _DistortionSpeedXYPowerZ.y));
					float4 uv2s4_Flow = i.ase_texcoord3;
					uv2s4_Flow.xy = i.ase_texcoord3.xy * _Flow_ST.xy + _Flow_ST.zw;
					float2 panner31 = ( 1.0 * _Time.y * appendResult29 + (uv2s4_Flow).xy);
					float Flowpower33 = _DistortionSpeedXYPowerZ.z;
					float T75 = uv2s4_Flow.w;
					float4 uvs4_MainTexture = i.texcoord;
					uvs4_MainTexture.xy = i.texcoord.xy * _MainTexture_ST.xy + _MainTexture_ST.zw;
					float2 appendResult27 = (float2(uvs4_MainTexture.z , uvs4_MainTexture.w));
					float2 temp_output_11_0 = ( (uvs4_MainTexture).xy + appendResult27 );
					float4 tex2DNode2 = tex2D( _MainTexture, ( ( tex2D( _Flow, panner31 ).a * Flowpower33 * (( _UV2Tswitch )?( T75 ):( 1.0 )) ) + temp_output_11_0 ) );
					float4 temp_cast_0 = ((( _UV2Tswitch )?( 1.0 ):( T75 ))).xxxx;
					float2 break15 = temp_output_11_0;
					float ifLocalVar18 = 0;
					if( 1.0 == ceil( break15.x ) )
					ifLocalVar18 = 0.0;
					else
					ifLocalVar18 = 1.0;
					float ifLocalVar19 = 0;
					if( 1.0 == ceil( break15.y ) )
					ifLocalVar19 = 0.0;
					else
					ifLocalVar19 = 1.0;
					float2 uv_Mask = i.texcoord.xy * _Mask_ST.xy + _Mask_ST.zw;
					float W70 = uv2s4_Flow.z;
					float4 appendResult3 = (float4(( _Emission * ( temp_output_66_0 * saturate( pow( tex2DNode2 , temp_cast_0 ) ) ) * i.color * tex2DNode45 ).rgb , saturate( ( ( tex2DNode2.a * ( 1.0 - max( ifLocalVar18 , ifLocalVar19 ) ) * i.color.a * tex2D( _Mask, uv_Mask ).a * temp_output_66_0 ) * W70 ) )));
					

					fixed4 col = appendResult3;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	
	
	Fallback Off
}
/*ASEBEGIN
Version=19701
Node;AmplifyShaderEditor.Vector4Node;28;-2996.185,-380.9642;Float;False;Property;_DistortionSpeedXYPowerZ;Distortion Speed XY Power Z;3;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;39;-2939.424,-544.927;Inherit;False;1;32;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-2165.69,-99.53895;Inherit;False;0;2;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;30;-2612.881,-431.1774;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;29;-2542.003,-360.5725;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;26;-1919.608,-102.1762;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-2616.226,-502.9784;Float;False;T;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;27;-1878.088,-25.08011;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;31;-2368.475,-401.1438;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;33;-2614.632,-267.7177;Float;False;Flowpower;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;-2049.449,-227.7967;Inherit;False;75;T;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;46;-2011.202,-636.227;Float;False;Property;_NoiseSpeedXYPowerZ;Noise Speed XY Power Z;4;0;Create;True;0;0;0;False;0;False;0,0,1,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;47;-2037.707,-752.6404;Inherit;False;0;45;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;11;-1696.929,-98.51338;Inherit;True;2;2;0;FLOAT2;-0.5,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;32;-2182.217,-429.697;Inherit;True;Property;_Flow;Flow;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.DynamicAppendNode;49;-1646.071,-616.0042;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;15;-1479.739,-98.09904;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ToggleSwitchNode;84;-1854.575,-252.3281;Inherit;False;Property;_UV2Tswitch;UV2Tswitch;8;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;-1835.169,-343.2113;Inherit;False;33;Flowpower;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;48;-1716.95,-686.6091;Inherit;False;True;True;False;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CeilOpNode;16;-1180.116,-246.2539;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CeilOpNode;17;-1184.57,-31.67378;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-988.0012,-59.72949;Float;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;50;-1460.412,-680.8436;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-987.0012,-130.7295;Float;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-1619.096,-431.5284;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;19;-709.0012,-51.72949;Inherit;True;False;5;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;38;-1399.431,-430.4391;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ConditionalIfNode;18;-714.7638,-265.8079;Inherit;True;False;5;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-1178.645,-821.423;Float;False;Property;_NoiseOpacityLerp;Noise Opacity Lerp;7;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;45;-1253.117,-743.7463;Inherit;True;Property;_Noise;Noise;1;0;Create;True;0;0;0;False;0;False;-1;None;4cdecdfbc6f2fa34689b6691b5d55879;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode;76;-992,-384;Inherit;False;75;T;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;67;-1119.448,-552.2353;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;81;-929.8432,-670.223;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1262.116,-458.7045;Inherit;True;Property;_MainTexture;Main Texture;0;0;Create;True;0;0;0;False;0;False;-1;None;4cdecdfbc6f2fa34689b6691b5d55879;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMaxOpNode;23;-405.2362,-162.1472;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;83;-816,-384;Inherit;False;Property;_UV2Tswitch;UV2Tswitch;8;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;74;-604.5735,-449.0113;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;40;-69.31241,-428.1146;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;66;-756.352,-656.7134;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;70;-2621.827,-583.4098;Float;False;W;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;25;-174.4099,-160.1449;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;43;-400.7015,68.87635;Inherit;True;Property;_Mask;Mask;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode;71;158.5059,-62.45157;Inherit;False;70;W;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;78;-439.1419,-450.3915;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;136.674,-273.2871;Inherit;True;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-261.708,-496.9098;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;434.0134,-273.0399;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;16,-688;Float;False;Property;_Emission;Emission;5;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;73;640.913,-270.2352;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;224,-640;Inherit;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;3;830.7225,-433.4673;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;1076.553,-434.6862;Float;False;True;-1;2;;0;11;Hovl/Particles/ShockWave;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;False;True;2;5;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;False;0;False;;False;False;False;False;False;False;False;False;False;True;2;False;;True;3;False;;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;30;0;39;0
WireConnection;29;0;28;1
WireConnection;29;1;28;2
WireConnection;26;0;4;0
WireConnection;75;0;39;4
WireConnection;27;0;4;3
WireConnection;27;1;4;4
WireConnection;31;0;30;0
WireConnection;31;2;29;0
WireConnection;33;0;28;3
WireConnection;11;0;26;0
WireConnection;11;1;27;0
WireConnection;32;1;31;0
WireConnection;49;0;46;1
WireConnection;49;1;46;2
WireConnection;15;0;11;0
WireConnection;84;1;86;0
WireConnection;48;0;47;0
WireConnection;16;0;15;0
WireConnection;17;0;15;1
WireConnection;50;0;48;0
WireConnection;50;2;49;0
WireConnection;37;0;32;4
WireConnection;37;1;36;0
WireConnection;37;2;84;0
WireConnection;19;0;22;0
WireConnection;19;1;17;0
WireConnection;19;2;22;0
WireConnection;19;3;21;0
WireConnection;19;4;22;0
WireConnection;38;0;37;0
WireConnection;38;1;11;0
WireConnection;18;0;22;0
WireConnection;18;1;16;0
WireConnection;18;2;22;0
WireConnection;18;3;21;0
WireConnection;18;4;22;0
WireConnection;45;1;50;0
WireConnection;67;1;46;3
WireConnection;81;0;45;4
WireConnection;81;2;82;0
WireConnection;2;1;38;0
WireConnection;23;0;18;0
WireConnection;23;1;19;0
WireConnection;83;0;76;0
WireConnection;74;0;2;0
WireConnection;74;1;83;0
WireConnection;66;0;81;0
WireConnection;66;3;67;0
WireConnection;66;4;46;3
WireConnection;70;0;39;3
WireConnection;25;0;23;0
WireConnection;78;0;74;0
WireConnection;24;0;2;4
WireConnection;24;1;25;0
WireConnection;24;2;40;4
WireConnection;24;3;43;4
WireConnection;24;4;66;0
WireConnection;44;0;66;0
WireConnection;44;1;78;0
WireConnection;72;0;24;0
WireConnection;72;1;71;0
WireConnection;73;0;72;0
WireConnection;41;0;42;0
WireConnection;41;1;44;0
WireConnection;41;2;40;0
WireConnection;41;3;45;0
WireConnection;3;0;41;0
WireConnection;3;3;73;0
WireConnection;1;0;3;0
ASEEND*/
//CHKSM=C3558FBDF6F85BA1F8982FC169A036A524E8C9B0