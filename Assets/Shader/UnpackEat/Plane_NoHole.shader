Shader "Custom/Plane_NoHole_Unlit"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        // Vẫn để Queue=Geometry để vẽ sau hole mask (Geometry-10) nhưng trước stickman (Geometry+1)
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            // Chỉ vẽ khi stencil != 1 (ngoại trừ vùng hole)
            Stencil
            {
                Ref 1
                ReadMask 255
                Comp NotEqual
                Pass Keep
            }
            // Unlit màu đơn giản
            ColorMask RGBA
            // Tắt mọi tính năng lighting, depth test vẫn OK
            Lighting Off
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}
