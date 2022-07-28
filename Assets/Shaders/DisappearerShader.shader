Shader "Custom/Disappearer"
{
	
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Size("Size",Range(0,1))=1// k
		_Elements("Elements",int)=3// l
		_Rotation("Rotation",Range(-3.14,3.14))=0// m
		_Deepness("Depness",Range(0,10))=0//s
		
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};


			//base
			uniform  fixed4 _Color;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform float _AlphaSplitEnabled;

			uniform float _Size;
			uniform int _Elements;
			uniform  float _Rotation;
			uniform  float _Deepness;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}




			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);
				return color;
			}
			float rand(in float2 uv)
			{
				float2 noise = (frac(sin(dot(uv ,float2(12.9898,78.233)*2.0)) * 43758.5453));
				return abs(noise.x + noise.y) * 0.5;
			}
	

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord);
				c.rgb *= c.a;
				
				float2 uv =IN.texcoord;
				uv.x-=0.5;
				uv.y-=0.5;
				float2 fUv;
				fUv.x=floor(uv.x*1000.)/1000.;
				fUv.y=floor(uv.y*1000.)/1000.;	
				float len= sqrt(uv.x*uv.x + uv.y*uv.y);
				float theta= atan2(uv.y,uv.x);
				float valueToBe=(_Elements-sin(_Elements*(theta+_Time))*_Deepness+_Size+rand(fUv)*20)/(2.*(_Elements+1.+_Size+rand(fUv)*20+_Deepness))*_Size ;
	
				if(len<valueToBe)
					return  c;
				else
					return fixed4(0,0,0,0);
				return c;
			}
		ENDCG
		}
	}
}