using ConsoleKicm;
using Buffer = ConsoleKicm.Buffer;

namespace ConsoleKicm;

public class Planet : Entity
{
    public int R = 2;
    public override void Render(Buffer buffer)
    {
        Utility.DrawBean(Pos,R,buffer,'#',ConsoleColor.Green,Layers.STATIC);//beans
    }

    public override void Update()
    {
        base.Update();
        foreach (Entity en in System.Entities)
        {
            if (en is FlyingLetter && (en.Pos-this.Pos).Len()<R-0.2f)
            {
                System.Remove(en);
                (System.Logic as GameLogic).Hp--;// don't judge me -_-

            }
        }
    }
}