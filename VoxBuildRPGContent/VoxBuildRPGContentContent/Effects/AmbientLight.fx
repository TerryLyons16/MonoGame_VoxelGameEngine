float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity;

struct VertexShaderInput
{
	float4 Position :  SV_POSITION;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
	float4 Position :  SV_POSITION;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

     float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    return output;

}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.

	float4 result = float4(AmbientColor.rgb * AmbientIntensity, AmbientColor.a);

	return result;//float4(1, 0, 0, 1);
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

		VertexShader = compile vs_4_0_level_9_3 VertexShaderFunction();
		PixelShader = compile ps_4_0_level_9_3 PixelShaderFunction();
    }
}
