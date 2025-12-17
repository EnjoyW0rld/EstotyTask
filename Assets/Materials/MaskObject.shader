Shader "Unlit/MaskObject"
{
    Properties{
        [IntRange] _StencilID ("Stencil ID",Range(0,255)) = 0
        }
    SubShader
    {
       Tags {"Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque"}
       ColorMask 0

       Pass
       {
           Blend Zero One
            ZWrite Off

            Stencil
            {
                Ref [_StencilID]
                Comp Always
                Pass Replace
                Fail Keep
            }
       }
    }
}
