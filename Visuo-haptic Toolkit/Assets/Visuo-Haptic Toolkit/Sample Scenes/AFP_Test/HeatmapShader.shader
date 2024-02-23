Shader "Unlit/HeatmapShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            float _DensityTable[3600];
            float _MaxDen;

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (
                float4 vertex : POSITION, // vertex position input
                float2 uv : TEXCOORD0 // first texture coordinate input
                )
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.uv = uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                int x = (i.uv.x * 60);
                int y = (i.uv.y * 60);
                float d = _DensityTable[x+(y*60)];

                float ratio = 1/_MaxDen;
                float val = d * ratio;

                float r = max(0, val);
                float g = max(0, (val-(1/3))*(3/2));
                float b = max(0, (val-(2/3))*(3/1));

                return fixed4(r,g,b,0);

            }
            ENDCG
        }
    }
}
