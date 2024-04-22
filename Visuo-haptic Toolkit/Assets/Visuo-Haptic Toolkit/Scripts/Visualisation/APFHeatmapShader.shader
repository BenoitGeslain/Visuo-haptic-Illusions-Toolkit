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

            float _DensityTable[4096];
            float _MaxDen;
            float _UnitsPerSide;

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
                float r,g,b; // red, green, blue
                int x = (i.uv.x * _UnitsPerSide); // x-position on quad
                int y = (i.uv.y * _UnitsPerSide); // y-position on quad
                float d = _DensityTable[x+(y*_UnitsPerSide)]; // value at position x, y

                float val = d / _MaxDen;

                if (val < 0.25)
                {
                    r = 0;
                    g = 0;
                    b = val * 3;
                }
                else if (val < 0.5)
                {
                    float tmp = val - 0.25; // [0;0.25]
                    r = 0;
                    g = tmp * 3;
                    b = 0.75 - tmp * 3;
                }
                else if (val < 0.75)
                {
                    float tmp = val - 0.5; // [0;0.25]
                    r = tmp * 4;
                    g = 0.75;
                    b = 0;
                }
                else if (val <= 1)
                {
                    float tmp = val - 0.75; // [0;0.25]
                    r = 1;
                    g = 0.75 - tmp * 3;
                    b = 0;
                }

                return fixed4(r,g,b,0);

            }
            ENDCG
        }
    }
}
