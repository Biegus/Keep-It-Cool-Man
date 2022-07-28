Shader "Custom/AskewShader"
{
	
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
			_Color ("Tint", Color) = (1,1,1,1)
		_Amount("Amount",int)=1
		_Div("Div",float)=2
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
			uniform float _Amount;
			uniform float _Div;
			

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
			float equation(fixed2 pos, float v1, float v2)
			{
    
      
				if(v1<v2)
				{
					if(pos.x>v1 && pos.x<  v2)
					return 1.;
					else return 0.;
				}
				else
				if(pos.x>v2 && pos.x<  v1)
					return 1.;
				else return 0.;
			}
            float func(float x, float x1, bool inv)
			{
                if(x1<0.5&& x<x1)
                	return 0.;
    			if(x1>=0.5 && x>x1)
       				 return 0.;
      			float a= 0.5/(0.5-x1);
    			if(inv) a=-a;
    			float b=-a*x1;
   				 return  a*x+b;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
				fixed2 uv = IN.texcoord;
				uv.y*=_Amount;
				uv.y= fmod(uv.y,1);
				  uv.x-=0.5;
				float x1= ((sin(_Time*100)/_Div)+1.)/2.;

			 return  c
				*((equation(uv,func(uv.y-0.2,x1,false),func(uv.y+0.2,x1,false)))+
				(equation(uv,func(uv.y-0.2,x1,true),func(uv.y+0.2,x1,true))));
				return c;
			}
		ENDCG
		}
	}
}