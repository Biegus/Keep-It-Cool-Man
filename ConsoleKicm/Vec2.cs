namespace ConsoleKicm;


public struct Vec2
{
    public float X, Y;// so even tho i got pixels, it's kinda helpful to have floats
    //like for instance imagine usually they don't do even single move in one frame
    // so with only ints they would be stuck
    public int XI => (int)Math.Round( X); // caching would be waste btw if you wonder
    public int YI => (int)Math.Round( Y);
    
    public Vec2(float x, float y)
    {
        this.X = x;
        this.Y = y;
    }
    //the methods below are the only ones i needed
    
    public static Vec2 operator-(Vec2 a,Vec2 b)
    {
        return new Vec2(a.X - b.X, a.Y - b.Y);
    }
    //oh no it's sqrt, why not X*X+Y*Y, you could deal with that
    //yy good point, also logic code takes like 0.02% of time, rest is used up for rendering so...
    public float Len()
    {
        return (float) Math.Sqrt(X * X + Y * Y);
    }
}