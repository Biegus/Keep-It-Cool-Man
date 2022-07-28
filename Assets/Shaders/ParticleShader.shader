Shader "Custom/ParticelShader"
{
	
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
	
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


			
			const float pi =3.14;
			float f=0.;
			void draw(bool value)
			{
				if(value)
					f=1.;
    
				}
			float square(float a)
			{
				return a*a;
			}

			fixed4 SampleSpriteTexture (float2 uv)
			{
				const float r=0.125;
				uv.x-=0.5;
				uv.y-=0.5;
				
				float k=_Time*70.;
				float x= uv.x;
				float y= uv.y;
				for(int i=0;i<4;i++)
				{
					float z=float(i)*pi/2.;
					float d=((sin(_Time+z*2)+1.)/2.+1.)*0.6;
					draw(square(x-cos(k*float(i+1)/2.+pi/2. * float(i))*r) + square(y-sin(k*float(i+1)/2.+pi/2. * float(i))*r) <= r*r*d*d);

				}
     
				float dist= 1.- length(uv-fixed2(0.,0.))/0.5;

				
			
				fixed4 color = fixed4( f*fixed3(1,1,1), f*dist);
				color*=_Color;				
				
				color*=color.a;
				return color;
			}

	

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
				
				return c;
			}
		ENDCG
		}
	}
}