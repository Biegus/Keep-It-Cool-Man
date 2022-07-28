namespace ConsoleKicm;



public class SymbolEntity : Entity
{
    public char Symbol { get; set; }
    public ConsoleColor? Color { get; set; }
    public override void Render(Buffer buffer)
    {
        base.Render(buffer);
        buffer.Write(this.Pos,Symbol,Layer,Color);
    }
}