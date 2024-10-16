import { dotnet } from './_framework/dotnet.js'

const { setModuleImports, runMain } = await dotnet.create();

setModuleImports('main.js', {
    GPURenderPassEncoder : {
        end: (renderPassEncoder) => renderPassEncoder.end(),
        draw: (renderPassEncoder, vertexCount) => renderPassEncoder.draw(vertexCount),
        setBindGroup0: (renderPassEncoder, index, bindingGroup) => renderPassEncoder.setBindGroup(index, bindingGroup),
        setBindGroup1: (renderPassEncoder, index, bindingGroup, dynamicOffsets, dynamicOffsetsStart, dynamicOffsetsLength) => renderPassEncoder.setBindGroup(index, bindingGroup, new Uint32Array(dynamicOffsets), dynamicOffsetsStart, dynamicOffsetsLength),
        setVertexBuffer: (renderPassEncoder, slot, buffer) => renderPassEncoder.setVertexBuffer(slot, buffer),
        setPipeline: (renderPassEncoder, renderPipeline) => renderPassEncoder.setPipeline(renderPipeline)
    },
    GPUBuffer: {
        destroy: (buffer) => buffer.destroy(),
    },
    GPUTexture: {
        createView: (texture) => texture.createView(),
        destroy: (texture) => texture.destroy()
    },
    GPUQueue: {
        writeBuffer: (queue, buffer, bufferOffset, data, dataOffset, size) => queue.writeBuffer(buffer, bufferOffset, new Float32Array(data), dataOffset, size),
        writeTexture: (queue, destinationJson, destinationReferences, data, dataLayoutJson, sizeJson) =>
            queue.writeTexture(
                JsonToObjectWithReferences(destinationJson, destinationReferences),
                new Uint8Array(data),
                JSON.parse(dataLayoutJson),
                JSON.parse(sizeJson)),
        copyExternalImageToTexture: (queue, sourceJson, sourceReferences, destinationJson, destinationReferences, copySizeJson) =>
            {
                queue.copyExternalImageToTexture(
                    JsonToObjectWithReferences(sourceJson, sourceReferences),
                    JsonToObjectWithReferences(destinationJson, destinationReferences),
                    JSON.parse(copySizeJson)
                );
            },
        submit: (queue, commandBuffers) => queue.submit(commandBuffers)
    },
    GPUDevice: {
        createCommandEncoder: (device) => device.createCommandEncoder(),
        createBuffer: (device, json, references) => device.createBuffer(JsonToObjectWithReferences(json, references)),
        createRenderPipeline: (device, json, references) => device.createRenderPipeline(JsonToObjectWithReferences(json, references)),
        createBindGroup: (device, json, references) => device.createBindGroup(JsonToObjectWithReferences(json, references)),
        createBindGroupLayout: (device, json, references) => device.createBindGroupLayout(JsonToObjectWithReferences(json, references)),
        createShaderModule: (device, json, references) => device.createShaderModule(JsonToObjectWithReferences(json, references)),
        createTexture: (device, json, references) => device.createTexture(JsonToObjectWithReferences(json, references)),
        createPipelineLayout: (device, json, references) => device.createPipelineLayout(JsonToObjectWithReferences(json, references)),
        createSampler: (device, jsonDescriptor) => device.createSampler(JSON.parse(jsonDescriptor))
    },
    GPUCommandEncoder: {
        beginRenderPass: (commandEncoder, json, references) => commandEncoder.beginRenderPass(JsonToObjectWithReferences(json, references)),
        finish: (commandEncoder) => commandEncoder.finish()
    },
    GPUCanvasContext: {
        getCurrentTexture: (canvasContext) => canvasContext.getCurrentTexture(),
        configure: (canvasContext, json, references) => canvasContext.configure(JsonToObjectWithReferences(json, references))
    },
    GPURenderPipeline: {
        getBindGroupLayout: (renderPipeline, index) => renderPipeline.getBindGroupLayout(index)
    },
    GPUAdapter: {
        requestDevice: async (gpuAdapter) => await gpuAdapter.requestDevice()
    },
    Canvas: {
        getContext: (canvas, contextId) => canvas.getContext(contextId),
        addEventListener: (canvas, type, callback) => canvas.addEventListener(type, (e) => {
            callback(JSON.stringify({ movementX: e.movementX, movementY: e.movementY }));
        })
    },
    Document: {
        addEventListener: (type, callback) => document.addEventListener(type, (e) => {
            callback(JSON.stringify({ code: e.code }));
        })
    },
    Window: {
        createImageBitmap: (image, options) => createImageBitmap(new Blob([new Uint8Array(image)]), JSON.parse(options)),
        getLocation: () => window.location.href
    }
});

function JsonToObjectWithReferences(json, references) {
    const obj = JSON.parse(json);

    ReplaceReferences(obj, references)

    return obj
}

function ReplaceReferences(obj, references) {
    Object.keys(obj).forEach(propertyName => {
        const prefix = "JSObject_Reference_";
        if(typeof obj[propertyName] === 'string' && obj[propertyName].startsWith(prefix)){
            const num = obj[propertyName].substring(prefix.length);
            obj[propertyName] = references[Number(num)]
        }
        else if(typeof obj[propertyName] === 'object'){
            ReplaceReferences(obj[propertyName], references)
        }
    })
}

const canvas = document.querySelector("#gpuCanvas")

canvas.width = canvas.parentElement.clientWidth;
canvas.height = canvas.parentElement.clientHeight;

canvas.addEventListener("click", function () {
    canvas.requestPointerLock();
});

await runMain();
