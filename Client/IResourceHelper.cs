namespace GameEngine;

public interface IResourceHelper
{
    Task<Texture> LoadTexture(string name);
    Task<Stream> LoadStream(string name);
}