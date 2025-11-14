Shader "Sprites/Outline with Anti-Aliasing"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

        [MaterialToggle] _IsOutlineEnabled("Enable Outline", float) = 0
        [HDR] _OutlineColor("Outline Color", Color) = (1,1,1,1)
        _OutlineSize("Outline Size", Range(0, 100)) = 1
        _AlphaThreshold("Alpha Threshold", Range(0, 1)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex ComputeVertex
            #pragma fragment ComputeFragment
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #pragma multi_compile _ SPRITE_OUTLINE_OUTSIDE

            sampler2D _MainTex, _AlphaTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;

            float _OutlineSize;
            float _AlphaThreshold;
            fixed4 _OutlineColor;

            struct VertexInput
            {
                float4 Vertex : POSITION;
                float4 Color : COLOR;
                float2 TexCoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 Vertex : SV_POSITION;
                fixed4 Color : COLOR;
                float2 TexCoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            VertexOutput ComputeVertex(VertexInput vertexInput)
            {
                VertexOutput vertexOutput;

                UNITY_SETUP_INSTANCE_ID(vertexInput);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(vertexOutput);

                vertexOutput.Vertex = UnityObjectToClipPos(vertexInput.Vertex);
                vertexOutput.TexCoord = vertexInput.TexCoord;
                vertexOutput.Color = vertexInput.Color * _Color;

                #ifdef PIXELSNAP_ON
                vertexOutput.Vertex = UnityPixelSnap(vertexOutput.Vertex);
                #endif

                return vertexOutput;
            }

            fixed4 SampleSpriteTexture(float2 uv)
            {
                return tex2D(_MainTex, uv);
            }

            float GetOutlineFactor(float alpha, float distance)
            {
                // Линейная интерполяция для сглаживания (плавный переход к обводке)
                return saturate((distance - alpha) / _OutlineSize);
            }

            fixed4 ComputeFragment(VertexOutput vertexOutput) : SV_Target
            {
                fixed4 color = SampleSpriteTexture(vertexOutput.TexCoord) * vertexOutput.Color;
                color.rgb *= color.a;

                float outline = 0.0;
                float2 texDdx = ddx(vertexOutput.TexCoord);
                float2 texDdy = ddy(vertexOutput.TexCoord);

                // Поиск прозрачных пикселей вокруг текущего
                for (int i = 1; i <= _OutlineSize; i++)
                {
                    float2 offset = float2(i * _MainTex_TexelSize.x, i * _MainTex_TexelSize.y);

                    // Получаем альфа-канал соседних пикселей
                    float upAlpha = tex2D(_MainTex, vertexOutput.TexCoord + offset).a;
                    float downAlpha = tex2D(_MainTex, vertexOutput.TexCoord - offset).a;
                    float leftAlpha = tex2D(_MainTex, vertexOutput.TexCoord - float2(offset.x, 0)).a;
                    float rightAlpha = tex2D(_MainTex, vertexOutput.TexCoord + float2(offset.x, 0)).a;

                    // Учитываем вклад в обводку
                    outline += 1.0 - upAlpha;
                    outline += 1.0 - downAlpha;
                    outline += 1.0 - leftAlpha;
                    outline += 1.0 - rightAlpha;
                }

                outline = saturate(outline / (_OutlineSize * 4)); // Нормализация результата

                // Наложение обводки только на прозрачные области
                float blend = outline * step(_AlphaThreshold, color.a);
                color.rgb = lerp(color.rgb, _OutlineColor.rgb, blend);

                return color;
            }

            ENDCG
        }
    }
}
