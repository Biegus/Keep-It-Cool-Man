Shader "Custom/WoobleShader"
{
	
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_WaveScale( "Wave Size", float) = 0.2
		_Speed ("Speed", Float ) =10
		_StartTime( "Start Time", Float) =0
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

			uniform float _Speed;
			uniform  float _WaveScale;
			uniform float _StartTime;
			

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

	

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
				c.rgb *= c.a;
				float x=( IN.texcoord.x+ _Time.x* _Speed)* (3.14*5);
				float y=( IN.texcoord.y+ _Time.x* _Speed)* (3.14*5);
			
				float value= floor(abs((x%2)-1)*4)/4 *_WaveScale ;//(abs(((_Time.x* _Speed*20)% (2)) -1));
				float yValue=floor(abs(((y)%2)-1)*4)/4 *_WaveScale*5;
				
				if(IN.texcoord.y > 1-_WaveScale-value
					|| IN.texcoord.y < _WaveScale - value)
					c.rgba*=0;
				
				if(IN.texcoord.x< yValue/20 || IN.texcoord.x>1-yValue/20)
				{
					c.rgba*=0;
				}
				return c;
			}
		ENDCG
		}
	}
}