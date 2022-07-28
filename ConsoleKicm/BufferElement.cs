namespace ConsoleKicm;

public struct BufferElement
{
    /// <summary>don't use layers below 1, layer 0 is NONE layer</summary>
    public int Layer;
    public char Symbol;
    public ConsoleColor? Color;
}