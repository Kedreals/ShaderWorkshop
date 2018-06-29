#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix World;
matrix View;
matrix Projection;

float TexRepetitions;
float NormalTexRepetitions;

float3 LightDirection;

float AmbientLightEll;

float4 AmbientLightColor = float4(1.0, 1.0, 1.0, 1.0);

texture Texture;
sampler2D textureSampler = sampler_state {
	Texture = (Texture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

texture NormalMap;
sampler2D normalSampler = sampler_state {
	Texture = (NormalMap);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoords : TEXCOORD0;
	float3 Normal : NORMAL0;
	float3 BiNormal : BINORMAL0;
	float3 Tangent : TANGENT0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoords : TEXCOORD0;
	float3x3 TangentToWorld : TEXCOORD1;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float4 worldPos = mul(input.Position, World);

	output.Position = mul(worldPos, mul(View, Projection));
	output.TexCoords = input.TexCoords;
	output.TangentToWorld[0] = normalize(mul(float4(input.Tangent, 0), World).xyz);
	output.TangentToWorld[1] = normalize(mul(float4(input.BiNormal, 0), World).xyz);
	output.TangentToWorld[2] = normalize(mul(float4(input.Normal, 0), World).xyz);

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 normalInTangentSpace = tex2D(normalSampler, input.TexCoords*NormalTexRepetitions % 1) * 2.0 - 1.0;
	float3 normal = mul(normalInTangentSpace.xyz, input.TangentToWorld);

	float4 color = tex2D(textureSampler, input.TexCoords*TexRepetitions % 1);

	float ell = dot(normalize(normal), normalize(LightDirection));

	return (ell + AmbientLightEll * AmbientLightColor)* color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};