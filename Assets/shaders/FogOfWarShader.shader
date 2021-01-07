Shader "Custom/FogOfWarShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _SecondaryTex ("Secondary Texture", 2D) = "white" {}
        _SightLinesTex ("Sight Lines Texture", 2D) = "white" {}
        _DefaultColor ("Default Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags {
            "Queue" = "Transparent+1"
        }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _SecondaryTex;
            sampler2D _SightLinesTex;
            fixed4 _DefaultColor;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) + tex2D(_SecondaryTex, i.uv) + tex2D(_SightLinesTex, i.uv);
                // col = tex2D(_SightLinesTex, i.uv);
                // fixed4 col = tex2D(_SightLinesTex, i.uv);
                // r + b + g = 0
                // r + g = .5
                // r > 1
                // g > 1
                // r + b > 1
                // col.a = 2.0f - 1.5f * col.r - 0.5f * col.b + col.g;

                col.a = 2.0f - 1.5f * (col.r * col.g) - 0.5f * (col.r * col.b * col.g);
                return fixed4(_DefaultColor.r, _DefaultColor.g, _DefaultColor.b, col.a);
                // return col;
            }
            ENDCG
        }
    }
}
