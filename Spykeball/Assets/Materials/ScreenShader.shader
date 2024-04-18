Shader "Unlit/ScreenShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        
        // noise property questions for projection effect
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseStrength ("Noise Strength", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _Color;
            float _NoiseStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // hash functions for projector effect
            float3 hash(uint3 x)
            {
                uint k = 0x6C078965;

                x = ((x >> 8) ^ x.yzx) * k;
                x = ((x >> 8) ^ x.yzx) * k;
                x = ((x >> 8) ^ x.yzx) * k;
                
                return float3(x) * (1.0 /float (0xffffffff));
            }

            // where the processing occurs
            fixed4 frag (v2f i) : SV_Target
            {
                // create random offset based on fbm function below
                uint fspd = 10;
                uint3 offset = uint3(i.uv, _Time.y * fspd);
                float3 noiseOffset = hash(offset);

                // add time offset to animate the noise pass
                // float2 noiseUV = i.uv + _Time.y * 0.1;
                float2 noiseUV = i.uv + noiseOffset;

                fixed4 col = tex2D(_MainTex, i.uv);
                
                // noise porperties
                fixed4 noise = tex2D(_NoiseTex, noiseUV);
                col.rgb += noise.rgb * _NoiseStrength;

                return col * _Color;
            }
            ENDCG
        }
    }
}
