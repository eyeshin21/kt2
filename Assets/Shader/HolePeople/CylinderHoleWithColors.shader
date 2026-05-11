Shader "Custom/CylinderHoleWithColors"
{
    Properties
    {
        _BaseColor       ("Base Color",    Color) = (1,1,1,1)
        _BorderColor     ("Border Color",  Color) = (0.8,0.4,0,1)
        _BorderThickness ("Border Thickness", Range(0,0.5)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
        ZWrite On
        ZTest LEqual

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _BaseColor;
            fixed4 _BorderColor;
            float  _BorderThickness;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float3 localPos : TEXCOORD0;
                float3 normal   : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos      = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz;
                o.normal   = v.normal;      // local-space normal
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 1) tính bán kính lỗ
                float holeR = 0.5 - _BorderThickness;
                // 2) vứt bỏ pixel nằm trong lỗ
                if (length(i.localPos.xz) < holeR)
                    discard;

                // 3) phân biệt mặt trên (top cap) và các mặt còn lại
                float3 n = normalize(i.normal);
                if (n.y > 0.95)        // gần như thẳng lên => top face
                    return _BorderColor;
                else                   // sườn hoặc đáy
                    return _BaseColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
