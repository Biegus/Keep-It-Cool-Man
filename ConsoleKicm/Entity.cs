namespace ConsoleKicm;



public class Entity
{
    public bool Alive { get; private set; }
    public GameSystem System { get; init; }
    public Vec2 Pos { get; set; }
    public int Layer { get; set; }
    
    public virtual void Update(){}
    public void InternalKill()
    {
        Alive = false;
    }
    public virtual  void Render(Buffer buffer){}
    
   
}