using System.Text;

namespace ConsoleKicm;

//so like where i need a global function and it doesn't really make sense to put it anywhere
//i will put it here and pretend that it makes sense
public static class Utility
{
    //beans?
    public static void DrawBean(Vec2 pos,int r,Buffer buffer,char symbol, ConsoleColor? color, int layer)
    {
        for(int y=-r*4+pos.YI;y<+r*4+pos.YI;y++)
        for(int x=-r*4+pos.XI;x<r*4+pos.XI;x++)
        {
            Vec2 dist = new(pos.XI - x, pos.YI - y);
            int len =  (int)Math.Round((Math.Sqrt(dist.XI * dist.XI*0.25f + dist.YI * dist.YI)));
            if (len <= r)
            {
                buffer.Write(new(x,y),symbol,layer,color);
            }
        }
    }
}