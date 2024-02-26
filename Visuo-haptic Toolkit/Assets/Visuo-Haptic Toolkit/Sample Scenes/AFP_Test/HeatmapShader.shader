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
                float r,g,b; // red, green, blue
                int x = (i.uv.x * 60); // position en X sur le quad
                int y = (i.uv.y * 60); // position en Y sur le quad
                float d = _DensityTable[x+(y*60)]; // valeur en position X, Y comprise entre 0 et 1

                float ratio = 1/_MaxDen;
                float val = d * ratio;

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
                    b = 0.75 - tmp*3;
                }
                else if (val < 0.75)
                {
                    float tmp = val - 0.5; // [0;0.25]
                    r = tmp*4;
                    g = 0.75;
                    b = 0;
                }
                else if (val <= 1)
                {
                    float tmp = val - 0.75; // [0;0.25]
                    r = 1;
                    g = 0.75 - tmp*3;
                    b = 0;
                }

                return fixed4(r,g,b,0);

            }
            ENDCG
        }
    }
}
