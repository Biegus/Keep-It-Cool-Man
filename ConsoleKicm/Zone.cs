namespace ConsoleKicm;

public class Zone : Entity
{
    public static Zone Instance { get; private set; }//singleton -_-
    public int R;
    public override void Update()
    {
        base.Update();
        Instance = this;
    }

    public override void Render(Buffer buffer)
    {
        base.Render(buffer);
        Utility.DrawBean(Pos,R,buffer,'.',ConsoleColor.Gray,Layers.ZONE);//beans..
    }

    public bool InZone(Vec2 pos)
    {
        Vec2 to = this.Pos - pos;
        int len = (int)Math.Round(Math.Sqrt(to.X * to.X * 0.25 + to.Y * to.Y));
        return len <=R;
    }
    
}

