namespace ConsoleKicm;

public class InputMangager
{
    //why dict not hashset?  so if there was two pressed in one frame of the same key (highly unlikely) it would work 
    //as expected
    private readonly Dictionary<char,int> pressed= new Dictionary<char,int>();
    private readonly Queue<char> requests = new Queue<char>();
    //Should be called just before update
    public void Refresh()
    {
        pressed.Clear();
        while (Console.KeyAvailable)
        {
            char key = ((char) Console.ReadKey(true).KeyChar);
            if (pressed.ContainsKey( (key)))
                pressed[key]++;
            else
                pressed.Add(key,1);

        }
    }
    public bool WasPressed(char key,bool unregister=false)
    {
        bool result= pressed.ContainsKey(key);
        if (result && unregister)
        {
            if (pressed[key] == 1)
                pressed.Remove(key);
            else
                pressed[key]--;
        }

        return result;
    }
 
 

    
}