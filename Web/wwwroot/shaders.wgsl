struct VertexOut {
    @builtin(position) position : vec4f,
    @location(0) texcoord : vec2f
}

struct MVP {
    modelMatrix: mat4x4<f32>,
    viewMatrix: mat4x4<f32>,
    projectionMatrix: mat4x4<f32>,
}

@binding(0)
@group(0)
var<uniform> mvp : MVP;

@group(1) @binding(0) var ourSampler: sampler;
@group(1) @binding(1) var ourTexture: texture_2d<f32>;

@vertex
fn vertex_main(@location(0) position: vec4f, @location(1) texcoord: vec2f) -> VertexOut
{
    var output : VertexOut;

    output.position = mvp.projectionMatrix * mvp.viewMatrix * mvp.modelMatrix * position;
    output.texcoord = texcoord;
    return output;
}

@fragment
fn fragment_main(fragData: VertexOut) -> @location(0) vec4f
{
    return textureSample(ourTexture, ourSampler, fragData.texcoord);
}