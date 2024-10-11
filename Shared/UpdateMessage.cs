using System.Numerics;
using MemoryPack;

namespace Shared;

[MemoryPackable]
[MemoryPackUnion(0, typeof(UpdateMessage))]
[MemoryPackUnion(1, typeof(CreateMessage))]
public partial interface IMessage;

[MemoryPackable]
public partial class UpdateMessage : IMessage
{
    public Guid EntityId;
    public NetworkTransform Transform;
}

[MemoryPackable]
public partial class CreateMessage : IMessage
{
    public Guid EntityId;
    public NetworkTransform Transform;
    public required string ModelId;
    //todo model, material etc.
}

public struct NetworkTransform //should this be separate???
{
    public Vector3 Position;
    public Vector3 Scale;
    public Vector3 Rotation;
}