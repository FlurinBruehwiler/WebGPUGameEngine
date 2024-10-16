﻿using Client;
using Client.WebGPU;
using Web.WebGPU;

namespace Web;

public class Helper
{
    public static IGPUTexture CreateTextureFromBitmap(ImageBitmap imageBitmap)
    {
        var texture = Game.GameInfo.Device.CreateTexture(new TextureDescriptor
        {
            Format = GPUTextureFormat.Rgba8Unorm,
            Usage = GPUTextureUsage.TEXTURE_BINDING |
                    GPUTextureUsage.COPY_DST |
                    GPUTextureUsage.RENDER_ATTACHMENT,
            Size = new SizeObject
            {
                Width = imageBitmap.Width,
                Height = imageBitmap.Height,
                DepthOrArrayLayers = 1
            }
        });

        ((GPUQueue)Game.GameInfo.Device.Queue).CopyExternalImageToTexture(
            new ImageSource
            {
                Source = imageBitmap,
                FlipY = true
            },
            new TextureDestination
            {
                Texture = texture
            },
            new TextureSize
            {
                Width = imageBitmap.Width,
                Height = imageBitmap.Height
            });

        return texture;
    }

}