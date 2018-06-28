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

float TimeInSeconds;

float4 wobbleDir;
float4 wobblePos;
float wobbleThickness;

float4 LightPos;
float LightIntensity;

float ambientIntensity;

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
	float3 Normal : NORMAL0;
	float2 TexCoords : TEXCOORD0;
	float3 Tangent : TANGENT0;
	float3 Binormal : BINORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoords : TEXCOORD0;
	float3x3 TangentToWorld : TEXCOORD3;
	float4 LightDir : TEXCOORD1;
	float dist : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	
	float wobble = sin(TimeInSeconds);
	float4 pointOnPlane = wobblePos + wobble * wobbleDir;

	float4 pos = input.Position;
	pos = mul(pos, World);
	
	output.dist = dot(pos - pointOnPlane, normalize(wobbleDir));
	output.LightDir = LightPos - pos;

	pos = mul(pos, View);
	output.Position = mul(pos, Projection);
	output.TexCoords = input.TexCoords;
	output.TangentToWorld[0] = mul(float4(normalize(input.Tangent), 0), World).xyz;
	output.TangentToWorld[1] = mul(float4(normalize(input.Binormal), 0), World).xyz;
	output.TangentToWorld[2] = mul(float4(normalize(input.Normal), 0), World).xyz;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = float4(1,1,1,1);
	/*if(abs(input.dist) < wobbleThickness){
		return float4(1,0,0,1);
	}else if(input.dist < 0) {
		color = tex2D(textureSampler, input.TexCoords);
	}*/

	float4 normal_color = tex2D(normalSampler, input.TexCoords * 50 % 1);
	float3 normal = 2.0*normal_color.xyz - 1.0;
	normal = normalize(mul(normal, input.TangentToWorld));

	float lightDist = length(input.LightDir);

	float intensity = LightIntensity / (lightDist*lightDist);
	intensity *= dot(normalize(input.LightDir), float4(normal, 1));
	intensity = clamp(intensity + ambientIntensity, 0.0, 1.0);
	return float4(color.xyz * intensity, color.w);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};