namespace ConsoleKicm;

//it's just wrapper for BufferElement[][] with some writing method that considers layers
public class Buffer
{
    /// <summary>[Y][X]</summary>
    private BufferElement[][] ar; 
    private static readonly BufferElement DEFAULT = new BufferElement() {Symbol = ' ', Layer = 0};
    public BufferElement Get(Vec2 pos)
    {
        return ar[pos.YI][pos.XI];
    }
    public Buffer(Vec2 size)
    {
        this.ar = new  BufferElement[size.YI][];
        for (int i = 0; i < size.YI; i++)
            this.ar[i] = new BufferElement[size.XI];
        ClearBuffer();
    }
    public void Write(Vec2 pos,char symbol, int layer,ConsoleColor? color=null)
    {
        // out of buffer? i don't care, why throw exception when you can simply not care
        if (pos.YI < 0 || pos.YI>=this.ar.Length || pos.XI<0 || pos.XI >= this.ar[0].Length) return; 
        ref BufferElement el = ref this.ar[pos.YI][pos.XI]; // this is the only place in the whole code that uses c# ref feature
        if (layer>el.Layer)// higher layer wins over lower one. So if there's already non empty object there just throw him out if you can
        {
            el = new BufferElement() {Layer = layer, Symbol = symbol,Color = color};
        }
        
    }

    public void ClearBuffer()
    {
        
        for(int i=0;i<ar.Length;i++)
        for (int j = 0; j < ar[0].Length; j++)
            this.ar[i][j] = DEFAULT;
    }
}