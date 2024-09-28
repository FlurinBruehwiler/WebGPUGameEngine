class Vector3 {
    constructor(x, y, z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

class Vector4 {
    constructor(x, y, z, w) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
}



const gameInfo = {
    vertices : [],
    renderPipeline: undefined,
    device: undefined,
    context: undefined
}

await Init();

async function Init() {
    const canvas = document.querySelector("#gpuCanvas")
    canvas.width = canvas.parentElement.clientWidth;
    canvas.height = canvas.parentElement.clientHeight;

    await InitRenderer(canvas)

    let frameCount = 0;
    let camera = {
        position: new Vector3(10, 0, 10),
        rotation: new Vector3(0, 0, 1)
    }

    requestAnimationFrame(Frame)

    function Frame(){
        frameCount++;

        StartFrame();

        DrawRectangle(
            {x: 0, y: 0, z: 10},
            {x: 0, y: 10, z: 10},
            {x: 10, y: 10, z: 10},
            {x: 10, y: 0, z: 10},
        );

        EndFrame(camera);

        requestAnimationFrame(Frame);
    }
}

//model: position, rotation, scale of an object
//view: inverse of the cameras own model matrix
//projection: to clip space

function StartFrame() {
    gameInfo.vertices = [];
}

async function InitRenderer(canvas) {
    const adapter = await navigator.gpu.requestAdapter();
    gameInfo.device = await adapter.requestDevice();

    const shaderDownloadResponse = await fetch("shader.wgsl");
    const shaders = await shaderDownloadResponse.text();

    const shaderModule = gameInfo.device.createShaderModule({
        code: shaders
    })

    gameInfo.context = canvas.getContext("webgpu")
    gameInfo.context.configure({
        device: gameInfo.device,
        format: navigator.gpu.getPreferredCanvasFormat(),
        alphaMode: "premultiplied"
    })

    const vertexBuffers = [
        {
            attributes: [
                {
                    shaderLocation: 0,
                    offset: 0,
                    format: "float32x4"
                },
                {
                    shaderLocation: 1,
                    offset: 16,
                    format: "float32x4"
                }
            ],
            arrayStride: 32,
            stepMode: "vertex"
        }
    ]

    const pipelineDescriptor = {
        vertex: {
            module: shaderModule,
            entryPoint: "vertex_main",
            buffers: vertexBuffers
        },
        fragment: {
            module: shaderModule,
            entryPoint: "fragment_main",
            targets: [
                {
                    format: navigator.gpu.getPreferredCanvasFormat()
                }
            ]
        },
        primitive: {
            topology: "triangle-list"
        },
        layout: "auto"
    }

    gameInfo.renderPipeline = gameInfo.device.createRenderPipeline(pipelineDescriptor);
}

function DrawRectangle(v1, v2, v3, v4) {
    DrawTriangle(v1, v2, v3);
    DrawTriangle(v1, v3, v4);
}

function DrawTriangle(v1, v2, v3) {
    gameInfo.vertices.push(v1, v2, v3)
}

function EndFrame(camera) {

    const commandEncoder = gameInfo.device.createCommandEncoder();
    const clearColor = { r: 0.0, g: 0.5, b: 1.0, a: 1.0 };

    const renderPassDescriptor = {
        colorAttachments: [
            {
                clearValue: clearColor,
                loadOp: "clear",
                storeOp: "store",
                view: gameInfo.context.getCurrentTexture().createView()
            }
        ]
    }

    //vertices
    const vertices = new Float32Array(gameInfo.vertices.flatMap(v =>
                                                    [
                                                        v.x, v.y, v.z, 1,
                                                        0, 1, 0, 0
                                                    ]))

    const vertexBuffer = gameInfo.device.createBuffer({
        size: vertices.byteLength,
        usage: GPUBufferUsage.VERTEX | GPUBufferUsage.COPY_DST
    });

    gameInfo.device.queue.writeBuffer(vertexBuffer, 0, vertices, 0, vertices.length);

    const projectionMatrix = CreateCameraProjectionMatrix(90, 1, 0.1, 1000);
    const viewMatrix = Matrix4x4.Translation(camera.position);

    const mvpMatrix = projectionMatrix.multiply(viewMatrix);

    //model - view - projection
    const mvpData = mvpMatrix.toFloat32Array();

    const uniformBuffer = gameInfo.device.createBuffer({
        size: 16*4, //4x4 matrix
        usage: GPUBufferUsage.UNIFORM | GPUBufferUsage.COPY_DST
    });

    gameInfo.device.queue.writeBuffer(uniformBuffer, 0, mvpData, 0, mvpData.length);

    const bindGroup = gameInfo.device.createBindGroup({
        layout: gameInfo.renderPipeline.getBindGroupLayout(0),
        entries: [
            { binding: 0, resource: { buffer: uniformBuffer } }
        ]
    })

    const passEncoder = commandEncoder.beginRenderPass(renderPassDescriptor);

    passEncoder.setPipeline(gameInfo.renderPipeline);
    passEncoder.setVertexBuffer(0, vertexBuffer)
    passEncoder.setBindGroup(0, bindGroup);
    passEncoder.draw(gameInfo.vertices.length) //vertex count

    passEncoder.end();
    const commandBuffer = commandEncoder.finish();
    gameInfo.device.queue.submit([commandBuffer]);

    vertexBuffer.destroy(); //do I need this?
}

class Matrix4x4{
    constructor(m0, m4, m8, m12,
                m1, m5, m9, m13,
                m2, m6, m10, m14,
                m3, m7, m11, m15) {
        this.m0 = m0;
        this.m1 = m1;
        this.m2 = m2;
        this.m3 = m3;
        this.m4 = m4;
        this.m5 = m5;
        this.m6 = m6;
        this.m7 = m7;
        this.m8 = m8;
        this.m9 = m9;
        this.m10 = m10;
        this.m11 = m11;
        this.m12 = m12;
        this.m13 = m13;
        this.m14 = m14;
        this.m15 = m15;
    }

    static Identity(){
        return new Matrix4x4(1,0, 0, 0,
                            0,  1, 0, 0,
                            0,  0, 1, 0,
                            0,  0, 0, 1)
    }

    static Translation(position){
        const matrix = this.Identity();
        matrix.setW(new Vector4(position.x, position.y, position.z, 1));
        return matrix;
    }

    multiply(other){
        return new Matrix4x4(
DotProduct(this.row1(), other.column1()), DotProduct(this.row1(), other.column2()), DotProduct(this.row1(), other.column3()), DotProduct(this.row1(), other.column4()),
DotProduct(this.row2(), other.column1()), DotProduct(this.row2(), other.column2()), DotProduct(this.row2(), other.column3()), DotProduct(this.row2(), other.column4()),
DotProduct(this.row3(), other.column1()), DotProduct(this.row3(), other.column2()), DotProduct(this.row3(), other.column3()), DotProduct(this.row3(), other.column4()),
DotProduct(this.row4(), other.column1()), DotProduct(this.row4(), other.column2()), DotProduct(this.row4(), other.column3()), DotProduct(this.row4(), other.column4()));
    }

    row1(){
        return new Vector4(this.m0, this.m4, this.m8, this.m12)
    }

    row2(){
        return new Vector4(this.m1, this.m5, this.m9, this.m13)
    }

    row3(){
        return new Vector4(this.m2, this.m6, this.m10, this.m14)
    }

    row4(){
        return new Vector4(this.m3, this.m7, this.m11, this.m15)
    }

    column1(){
        return new Vector4(this.m0, this.m1, this.m2, this.m3)
    }

    column2(){
        return new Vector4(this.m4, this.m5, this.m6, this.m7)
    }

    column3(){
        return new Vector4(this.m8, this.m9, this.m10, this.m11)
    }

    column4(){
        return new Vector4(this.m12, this.m13, this.m14, this.m15)
    }

    setX(v){
        this.m0 = v.x;
        this.m1 = v.y;
        this.m2 = v.z;
        this.m3 = v.w;
    }

    setY(v){
        this.m4 = v.x;
        this.m5 = v.y;
        this.m6 = v.z;
        this.m7 = v.w;
    }

    setZ(v){
        this.m8 = v.x;
        this.m9 = v.y;
        this.m10 = v.z;
        this.m11 = v.w;
    }

    setW(v){
        this.m12 = v.x;
        this.m13 = v.y;
        this.m14 = v.z;
        this.m15 = v.w;
    }

    toFloat32Array(){
        return new Float32Array([
            this.m0, this.m1, this.m2, this.m3,
            this.m4, this.m5, this.m6, this.m7,
            this.m8, this.m9, this.m10, this.m11,
            this.m12, this.m13, this.m14, this.m15,
        ])
    }
}

function CreateCameraProjectionMatrix(fov, aspectRatio, nearDistance, farDistance) {
    const height = 1 / Math.tan(fov * 0.5);
    const width = height / aspectRatio;
    const range = farDistance / (nearDistance - farDistance);

    const matrix = new Matrix4x4();
    matrix.setX(new Vector4(width, 0, 0, 0));
    matrix.setY(new Vector4(0, height, 0, 0));
    matrix.setZ(new Vector4(0, 0, range, 0));
    matrix.setW(new Vector4(0, 0, 0, range * nearDistance));
    return matrix;
}

function DotProduct(v1, v2) {
    return v1.x * v2.x
        + v1.y * v2.y
        + v1.z * v2.z
        + v1.w * v2.w;
}
