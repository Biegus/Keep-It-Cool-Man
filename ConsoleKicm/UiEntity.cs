namespace  ConsoleKicm;

//man just draws whole ui, what a chad
public class UiEntity : Entity
{
    public int XSize;
    private int xTextPos = 0;
    private int yTextPos = 0;
    
    public override void Update()
    {
        base.Update();
        yTextPos = 0;
        xTextPos = 0;
    }

    public override void Render(Buffer buffer)
    {
        const int LAYER = Layers.UI;
        int xStart = System.RenderSize.XI - XSize;
        const char COLUMN = '│';
        const char ROW = '─';
        for (int i = 0; i < System.RenderSize.YI;i++)
        {
            buffer.Write(new(xStart, i), COLUMN, LAYER);
            buffer.Write(new (System.RenderSize.XI-1,i),COLUMN,LAYER);
            buffer.Write(new (0,i),COLUMN,LAYER);
        }
        for (int i = 0; i < System.RenderSize.XI;i++)
        {
            buffer.Write(new(i,0), ROW, LAYER);
            buffer.Write(new(i, System.RenderSize.YI-1), ROW, LAYER);
        }
    }
    //doesn't support using '\n' 
    public void WriteText(string text,Buffer buffer,bool nextLine=false,ConsoleColor? color=null)
    {
        if (text.Length > XSize)
            throw new ArgumentException("too long");
        if (XSize - xTextPos -1 < text.Length)
        {
            yTextPos++;
            xTextPos = 0;
        }
        int xStart=this.System .RenderSize.XI - XSize+1;
        ;
        foreach(char el in text)
        {
            buffer.Write(new(xStart+ xTextPos,yTextPos+1),el,Layers.UI,color);
            xTextPos++;
        }

        if (nextLine)
        {
            yTextPos++;
            xTextPos = 0;
        }


    }
}

