using System.Drawing;
using System.Media;

namespace ConsoleKicm;


//represents abstract game logic base. Such game logic should only contain directly game related things
public interface IGameLogic
{
    public GameSystem System { get; }
    void InitSystem(GameSystem system);
    void Start();
    void Update();
    void Render(Buffer buffer);

}
//this class is similar to GameSystem but this holds only logic directly related with the game
//could be replaced with any else as long as it derives from IGameLogic
public class GameLogic : IGameLogic
{
    public int Score { get; set; }
    public int Hp { get; set; } = 5;
    public GameSystem System { get; private set; }

    private float delay = 1f; // current delay between spawns
    private float speed = 5; // current speed of new letter
    private float spawnCounter; // time for next spawn
    private Vec2 center; // center of area (excluding ui)
    private UiEntity ui; // whole ui
    private readonly Random random = new Random(2137);// yea it's seeded, why not
    private int soundWheel;
    private SoundPlayer sound = new SoundPlayer();

    public void InitSystem(GameSystem system)
    {
        if (this.System == null)
            this.System = system;
        else throw new InvalidOperationException("System cannot be changed once it's set");
    }
    public void Start()
    {
        spawnCounter = delay;
        System.Add(ui=new UiEntity()
        {
            XSize = 13,
            System = System //yy technically it looks a little wrong, it kinda is
        });
        center = new((System.RenderSize.X-ui.XSize) / 2 - 1, this.System.RenderSize.Y / 2 - 1);

        System.Add(new Planet()
        {
            
            Layer = Layers.STATIC,
            Pos=center,
            System=System

        });
        System.Add(new Zone()
        {
            Pos=center,
            Layer = Layers.ZONE,
            System =System,
            R=6
        });
    }

    private void Spawn()
    {
        int mode = random.Next(0, 4);//  sides like left, down
        Vec2 pos = mode switch
        {
            0 => new(random.Next(1, System.RenderSize.XI - ui.XSize), 1),
            1 => new(random.Next(1, System.RenderSize.XI - ui.XSize), System.RenderSize.Y-1),
            2 => new( System.RenderSize.X-ui.XSize,random.Next(1, System.RenderSize.YI)),
            3 => new( 1,random.Next(1, System.RenderSize.YI)),
        };
        FlyingLetter l = new FlyingLetter()
        {
            Speed = speed,
            Color = ConsoleColor.Red,
            Pos = pos,
            Target = center,
            Symbol = (char) (random.Next(0, 'z' - 'a' + 1) + 'a'), //any letter
            Layer = Layers.IMPORTANT,
            Center = center,
            System = System
        };
        l.OnKilled += OnLetterKilled;
        System.Add(l);
    }

    private void OnLetterKilled(FlyingLetter obj)
    {
        obj.OnKilled -= OnLetterKilled;
        this.Score++;
        System.Add(new Laser()
        {
            A=center,
            B=obj.Pos,
            System = System
        });
        sound.Stop();
        sound.Stream?.Dispose();
        sound.Stream = new FileStream($"Audio\\shoot{soundWheel + 1}.wav", FileMode.Open);
        sound.Play();
        soundWheel++;
        soundWheel %= 4;
    }


    public void Update()
    {
        spawnCounter -= System.Delta;
        if (spawnCounter < 0)
        {
            delay *= 0.99f;
            speed *= 1.004f;
            spawnCounter = delay;
            Spawn();
        }

        if (Hp <= 0)
        {
            System.Terminate("Game over",false);
        }
        
    }

    public void Render(Buffer buffer)
    {
        
        //why not '\n'? Write text doesn't support it kinda, it would behave weirdly then
        
        ui.WriteText("Keep ",buffer,color: ConsoleColor.White);
        ui.WriteText("It ",buffer,color: ConsoleColor.White);
        ui.WriteText("ASCI ",buffer,color: ConsoleColor.White);
        ui.WriteText("Man ",buffer,true,color: ConsoleColor.White);
        ui.WriteText(string.Empty,buffer,true);
        ui.WriteText($"Score {Score}",buffer,true,color:ConsoleColor.Yellow);
        ui.WriteText($"Hp {Hp}",buffer,true,color: ConsoleColor.Red);
        ui.WriteText($"Fps {1/System.Delta:000.0}",buffer,true);
        ui.WriteText(string.Empty,buffer,true);
        ui.WriteText(string.Empty,buffer,true);
        ui.WriteText($"Del {delay:00.0}",buffer,true);
        ui.WriteText($"Spd {speed:00.0}",buffer);
        
    }
    
}

