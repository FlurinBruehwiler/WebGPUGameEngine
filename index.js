const gameInfo = {
    vertices : [],
    bindGroup: undefined,
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

    requestAnimationFrame(Frame)

    function Frame(){
        frameCount++;

        StartFrame();

        let pos = frameCount % 200;
        pos = pos / 200;

        DrawRectangle(
            {x: 0, y: 0, z: 0},
            {x: 0, y: pos -  0.5, z: 0},
            {x: pos - 0.5, y: pos -  0.5, z: 0},
            {x: pos -  0.5, y: 0, z: 0},
        );

        EndFrame();

        requestAnimationFrame(Frame);
    }
}

//model: position, rotation, scale of an object
//view: inverse of the cameras own model matrix

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

    const mvpData = new Float32Array([
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 1
    ])

    const uniformBuffer = gameInfo.device.createBuffer({
        size: 16*4, //4x4 matrix
        usage: GPUBufferUsage.UNIFORM | GPUBufferUsage.COPY_DST
    });

    gameInfo.device.queue.writeBuffer(uniformBuffer, 0, mvpData, 0, mvpData.length);

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

    gameInfo.bindGroup = gameInfo.device.createBindGroup({
        layout: gameInfo.renderPipeline.getBindGroupLayout(0),
        entries: [
            { binding: 0, resource: { buffer: uniformBuffer } }
        ]
    })
}

function DrawRectangle(v1, v2, v3, v4) {
    DrawTriangle(v1, v2, v3);
    DrawTriangle(v1, v3, v4);
}

function DrawTriangle(v1, v2, v3) {
    gameInfo.vertices.push(v1, v2, v3)
}

function EndFrame() {

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

    const passEncoder = commandEncoder.beginRenderPass(renderPassDescriptor);

    passEncoder.setPipeline(gameInfo.renderPipeline);
    passEncoder.setVertexBuffer(0, vertexBuffer)
    passEncoder.setBindGroup(0, gameInfo.bindGroup);
    passEncoder.draw(gameInfo.vertices.length) //vertex count

    passEncoder.end();
    const commandBuffer = commandEncoder.finish();
    gameInfo.device.queue.submit([commandBuffer]);

    vertexBuffer.destroy(); //do I need this?
}
