// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import { dotnet } from './_framework/dotnet.js'

const { setModuleImports, runMain } = await dotnet.create();

setModuleImports('main.js', {
    GPURenderPassEncoder : {
        end: (renderPassEncoder) => renderPassEncoder.end(),
        draw: (renderPassEncoder, vertexCount) => renderPassEncoder.draw(vertexCount),
        setBindGroup: (renderPassEncoder, slot, bindingGroup) => renderPassEncoder.setBindGroup(slot, bindingGroup),
        setVertexBuffer: (renderPassEncoder, slot, buffer) => renderPassEncoder.setVertexBuffer(slot, buffer),
        setPipeline: (renderPassEncoder, renderPipeline) => renderPassEncoder.setPipeline(renderPipeline)
    },
    GPUBuffer: {
        destroy: (buffer) => buffer.destroy(),
    },
    GPUTexture: {
        createView: (texture) => texture.createView()
    },
    GPUQueue: {
        writeBuffer: (queue, buffer, bufferOffset, data, dataOffset, size) => queue.writeBuffer(buffer, bufferOffset, data, dataOffset, size),
        submit: (queue, commandBuffers) => queue.submit(commandBuffers)
    },
    GPUDevice: {
        createCommandEncoder: (device) => device.createCommandEncoder(),
        createBuffer: (device, json, references) => device.createBuffer(JsonToObjectWithReferences(json, references)),
        createRenderPipeline: (device, json, references) => device.createRenderPipeline(JsonToObjectWithReferences(json, references)),
        createBindGroup: (device, json, references) => device.createBindGroup(JsonToObjectWithReferences(json, references)),
        createShaderModule: (device, json, references) => device.createShaderModule(JsonToObjectWithReferences(json, references))
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
    GPU: {
        getPreferredCanvasFormat: () => navigator.gpu.getPreferredCanvasFormat(),
        requestAdapter: async () => await navigator.gpu.requestAdapter()
    },
    GPUAdapter: {
        requestDevice: async (gpuAdapter) => await gpuAdapter.requestDevice()
    },
    Canvas: {
        getContext: (canvas, contextId) => canvas.getContext(contextId)
    },
    Document: {
        querySelector: (selector) => document.querySelector(selector)
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
        if(propertyName.startsWith(prefix)){
            const num = propertyName.substring(0, prefix.length)
            obj[propertyName] = references[Number(num)]
        }
        else if(typeof obj[propertyName] === 'object'){
            ReplaceReferences(obj, references)
        }
    })
}

await runMain();

