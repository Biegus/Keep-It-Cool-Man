namespace ConsoleKicm;

public class Laser : Entity
{
    public Vec2 A;
    public Vec2 B;
    private float time = 0.1f;
    public override void Update()
    {
        base.Update();
        time -= System.Delta;
        if(time<0)
            System.Remove(this);
    }
    public override void Render(Buffer buffer)
    {
        //It works similar to walking, just moves by vector of len one as many times as needed
        //every move it uses rounded up values to write to buffer.
        Vec2 current = A;
        while (current.XI != B.XI || current.YI!=B.YI)
        {
            buffer.Write(new(current.X,current.Y), 'x', Layers.LASER, ConsoleColor.Red);
            Vec2 to = B - current;
            float len = to.Len();
            if (MathF.Abs(len) < 1) break;//if we are that close i don't think we need to carry anymore
            
            Vec2 normalized = new(to.X / len, to.Y / len);
            current.X += normalized.X;
            current.Y += normalized.Y;
            
        }
    }
}