using System.Media;

namespace ConsoleKicm;

public class FlyingLetter: SymbolEntity
{
    public Vec2 Target { get; set; }
    public float Speed { get; set; } = 3;

    public Vec2 Center { get; set; }
    public event Action<FlyingLetter> OnKilled = delegate { }; 
    public override void Update()
    {
        base.Update();

        Vec2 line = Target - this.Pos;
        float len = line.Len();
        Vec2 direction = new(line.X / len, line.Y / len);
        float scaler = Speed * System.Delta;
        this.Pos = new(this.Pos.X + direction.X * scaler, this.Pos.Y + direction.Y * scaler);
        if (System.Input.WasPressed(Symbol, true) && Zone.Instance.InZone(Pos) )
        {
            this.System.Remove(this);
            OnKilled(this);
            
        }
    }
    
 
}