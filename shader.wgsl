struct VertexOut {
  @builtin(position) position : vec4f,
  @location(0) color : vec4f
}

@binding(0)
@group(0)
var<uniform> viewMatrix : mat4x4<f32>;

@binding(1)
@group(0)
var<uniform> projectionMatrix : mat4x4<f32>;

@vertex
fn vertex_main(@location(0) position: vec4f, @location(1) color: vec4f) -> VertexOut
{
  var output : VertexOut;

  //model view projection

  output.position = projectionMatrix * viewMatrix * position;
  output.color = vec4f(1, 0, 0, 0);
  return output;
}

@fragment
fn fragment_main(fragData: VertexOut) -> @location(0) vec4f
{
  return vec4f(0, 1, 0, 0);
}
