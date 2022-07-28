using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleKicm;

public enum Context
{
    Undefined=0,
    Rendering,
    Updating,
}
//Class responsible for main loop includng drawing and entity system
//this class doesn't contain anything specific to game logic.
public class GameSystem
{
    public IGameLogic Logic { get; }
    public Context Context { get; private set; }
    public InputMangager Input { get; } = new InputMangager();
    public float Delta = 0.01f;// Time between frames, it's by default set to 0.01f just because it's easy to deal with than 0
    // for instance you may divide by 0 or something, 0.01 will not change that much
    public Vec2 RenderSize { get; }
    
    public IReadOnlyCollection<Entity> Entities => entities;
   
    private readonly List<Entity> entities = new(); //list of alive entities
    private readonly Buffer buffer; // technically it's buffer that later writes into other buffer.. funny
    private readonly Queue<Entity> cleanup = new (); //list of entity that will die at the end of frame (can't modify the list while middle of a frame)
    private ConsoleColor startColor;
 
    private ConsoleColor currentConsoleColor;// why remembering this manually? reading via getter from Console.Foreground console
    private bool inited = false;
    private int frames = 0;//how many frames has passed
    private bool originalCursorVisibility;
    private bool instantClose = false;
    private bool terminating=false;
    private string terminationText="closed";
    
    public void Add(Entity entity)
    {
        entities.Add(entity);
    }
    public GameSystem(Vec2 renderSize, IGameLogic logic)
    {
        this.RenderSize = renderSize;
        buffer = new Buffer(renderSize);
        logic.InitSystem( this);
        this.Logic = logic;

    }
    public void GameLoop()
    {
        Stopwatch watch = new();
        Init();
        while (!terminating)
        {
            if (frames > 0)
                Delta = watch.ElapsedTicks / 10_000_000f;
            watch.Restart();
            Update();
            Context = Context.Rendering;
           
            Render();
            Context = Context.Undefined;
            frames++;
        }
    }
    private void Update()
    {
        Console.CursorVisible = false;//it sometimes actives on its own
        Context = Context.Updating;
        Input.Refresh();
        foreach (Entity entity in entities.ToArray())
        {
            entity.Update();
        }
        foreach (Entity entity in cleanup)
        {
            entities.Remove(entity);
        }
        Logic.Update();
    }
    private void Init()
    {
        
        if (inited)
            throw new InvalidOperationException("Init was called twice");

        originalCursorVisibility = Console.CursorVisible;
        inited = true;
        currentConsoleColor = Console.ForegroundColor;
        Console.CancelKeyPress += OnCancelKey;   
        startColor = Console.ForegroundColor;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WindowHeight = Math.Max(this.RenderSize.YI+1,Console.WindowHeight);
            Console.WindowWidth = Math.Max(this.RenderSize.XI+1,Console.WindowWidth);
        }
        Logic.Start();
    }

    private void OnCancelKey(object? sender, ConsoleCancelEventArgs e)
    {
        if (terminating)
            return;
        Terminate("Interrupted",true);
        e.Cancel = true;
        // will do manually, better to stop at the end of a frame
    }

    private void MoveCursorBack()
    {
        Console.CursorTop -= Math.Min(Console.CursorTop, RenderSize.YI);
        Console.CursorLeft -= Math.Min(Console.CursorLeft, RenderSize.XI);    
    }
    public void Remove(Entity entity)
    {
        entity.InternalKill();
        cleanup.Enqueue(entity);
    }
    private void Render()
    {
        buffer.ClearBuffer();
        foreach (Entity entity in entities)
        {
            entity.Render(buffer);
        }
        Logic.Render(buffer);
   
        if (frames > 0)
        {
            MoveCursorBack();
        }
        //To change color i have to stop writing to output and use Console.Foregroundcolor.
        // there's no other option and console.write is slow.
        StringBuilder cached = new StringBuilder();

        for ( int yRenderPos = 0; yRenderPos < RenderSize.YI; yRenderPos++)
        {
            for ( int xRenderPos = 0; xRenderPos < RenderSize.XI; xRenderPos++)
            {

                BufferElement el = buffer.Get(new(xRenderPos, yRenderPos));
                ConsoleColor nw = el.Color ?? startColor;
                if (nw != currentConsoleColor) // why not just Console.ForegroundColor? why have fun with caching?
                // surprisingly Console.ForegroundColor {get;} is expensive as heck. Using this funny caching boosted speed 6 times
                {

                    Console.Out.Write(cached);
                    cached.Clear();
                    ConsoleColor color = el.Color ?? startColor;
                    Console.ForegroundColor = color;
                    currentConsoleColor = color;
                    cached.Append(el.Symbol);
                }
                else
                {
                    cached.Append(el.Symbol);
                }
              
            }
            cached.Append('\n');
        }
        Console.Write(cached);
    }


    //cursorVisible property sometimes could be left with false
    public void Dispose()
    {
        
        Console.CursorVisible = originalCursorVisibility;
        Console.CancelKeyPress -= OnCancelKey;
    }
    public void InvokeGoodbye()
    {
        Console.WriteLine(terminationText);
    }

    public void FinishWithOutput()
    {
        InvokeGoodbye();
        Dispose();
        if (!instantClose)
        {
            Console.WriteLine("Press any key");
            Console.ReadKey(true);
        }
    }
    public  void Terminate(string text,bool instant)
    {
        if (terminating)
            return;
        this.terminationText = text;
        terminating = true;
        this.instantClose = instant;
    }
}