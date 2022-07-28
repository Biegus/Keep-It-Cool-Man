using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cyberultimate;
using Cyberultimate.Unity;
using LetterBattle;
using LetterBattle.Utility;

using UnityEngine;
//TODO refactor
public class DebugGUI : MonoBehaviour
{
    public enum Mode
    {
        Hidden=0,
        HiddenWithButton=1,
        Shown
    
    }
  
    private const string TIME_FORMAT= @"mm\:ss\:ff";
    public static Mode ActiveMode { get;  set; }
    

    [SerializeField]
    private EventSystemHandler eventSystemHandler = null;

    private GUIStyle buttonStyle;
    private GUIStyle labelStyle;
    private float? rememberedHp = null;
    private float timeAtStart;
    private List<(object who, float dmg, float time)> dmgesAtTime = new List<(object who, float dmg, float time)>();
    private bool paused = false;
    private int hits = 0;
    static DebugGUI()
    {
        #if UNITY_EDITOR
        ActiveMode = Mode.Shown;
        #elif DEVELOPMENT_BUILD
        ActiveMode=Mode.HiddenWithButton; 
        #else 
        ActiveMode= Mode.Hidden;
        #endif
    }

   
    private void Start()
    {
        timeAtStart = Time.time;
        if (MonoCyberConsole.Current != null)
            MonoCyberConsole.Current.gameObject.SetActive(false);
        #if UNITY_EDITOR
        LevelManager.Current.Hp.OnValueChanged += OnValueChanged;
        #endif
        LEvents.Base.OnLetterDestroyed.Raw += OnLetterDestroyed;
    }
    private void OnDestroy()
    {
        Unregister();
    }
    private void OnLetterDestroyed(object sender, ActionLetter e)
    {
        if (e.DeathReason == ActionLetter.DeathType.KilledByPlayer)
            hits++;
    }


    private void OnGUI()
    {
        
        buttonStyle ??= new GUIStyle("button")
        {
            fontSize = 30
        };
        labelStyle ??= new GUIStyle("label")
        {
            fontSize = 30
        };

        switch (ActiveMode)
        {
            case Mode.Shown:
                DrawAll(); break;
            case Mode.HiddenWithButton:
                DrawEnableButton(); break;
        }
    }
    private void OnValueChanged(object sender, LockValue<float>.AnyValueChangedArgs e)
    {
        dmgesAtTime.Add((e.From, e.Actual-e.Last, Time.time - timeAtStart));
    }
    
    private void Unregister()
    {
        TimeScaling.Status.Unregister(this);
        if(InputReader.Current!=null)
            InputReader.Current.DeafStatus.Unregister(this);
        if (MonoCyberConsole.Current != null)
            MonoCyberConsole.Current.gameObject.SetActive(false);
        LEvents.Base.OnLetterDestroyed.Raw -= OnLetterDestroyed;
    }
    private string FormatTime(float seconds)
    {
        return TimeSpan.FromSeconds(seconds).ToString(TIME_FORMAT);
    }
    private void DrawAll()
    {

        GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
        {
            GUILayout.BeginVertical(GUILayout.Width(Screen.width / 4f));
            {
                DrawLeftBar();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }
    private static void DrawEnableButton()
    {
        if (GUILayout.Button("Debug menu:"))
        {
            ActiveMode = Mode.Shown;
        }
    }
    private void DrawLeftBar()
    {

        GUILayout.BeginHorizontal();
        {
            DrawHidePauseOptions();
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        {
            DrawCheatButtons();
        }
        GUILayout.EndHorizontal();
        
        DrawBaseStatistics();

        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical();
            {
                DrawDmgHistory();
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            {
                GUILayout.Label($"Letters :\n{ActionLetter.AliveOnes.Aggregate(new StringBuilder(), (b, v) => b.Append(v.Letter))}", labelStyle);
            }
            GUILayout.EndVertical();
        } GUILayout.EndHorizontal();
        
        
    }

    private void DrawHidePauseOptions()
    {

        if (GUILayout.Button("Hide all", buttonStyle))
        {
            ActiveMode = Mode.HiddenWithButton;
        }

        if (GUILayout.Button("Debug pause", buttonStyle))
        {
            if (!paused)
            {
                TimeScaling.Status.Register(this, 0);
                if (InputReader.Current != null)
                    InputReader.Current.DeafStatus.RegisterObj(this);
                
                MonoCyberConsole.Current.gameObject.SetActive(true);

                paused = true;
            }
            else
            {
                Unregister();
                paused = false;
            }

            eventSystemHandler.gameObject.SetActive(!paused);

        }
    }
    private void DrawCheatButtons()
    {

        if (GUILayout.Button("Reset", buttonStyle))
        {
            GameManager.FinalizeLevel(GameManager.CurrentLevelNumber,false,true);
        }
        
        if (GUILayout.Button("Next level", buttonStyle))
        {
            GameManager.FinalizeLevel(-1, true);
        }
        if (GUILayout.Button("Before level", buttonStyle))
        {
            if (GameManager.CurrentLevelNumber != 0)
                GameManager.FinalizeLevel(GameManager.CurrentLevelNumber - 1, true);
        }
        GUILayout.EndHorizontal();
        ;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("AutoPilot", buttonStyle))
        {
            AutoPilot.InvertState();
        }

        if (GUILayout.Button("Ice Age", buttonStyle))
        {
            if (rememberedHp == null)
            {
                rememberedHp = LevelManager.Current.Hp.Max;
                LevelManager.Current.Hp.SetMax(4000, "Ice Age Mode");
                LevelManager.Current.Hp.SetValue(4000, "Ice Age Mode");
            }
            else
            {
                LevelManager.Current.Hp.SetMax(rememberedHp.Value);
                LevelManager.Current.Hp.SetValue(rememberedHp.Value);
                rememberedHp = null;
            }

        }
        if (GUILayout.Button("Kill all", buttonStyle))
        {
            foreach (var letter in ActionLetter.AliveOnes)
            {
                letter.Kill();
            }
        }
    }
    
    private void DrawBaseStatistics()
    {
         void Label(string text)
        {
                GUILayout.Label(text,labelStyle);
        }
        Label($"Hp:{LevelManager.Current.Hp}");
        Label($"Perfect:{ScoreSystem.Current.PerfectPercent}");
        Label($"Time elapsed : {FormatTime(Time.time-timeAtStart)} / {FormatTime(GameManager.CurrentLevel?.Time ?? 0)}");
        #if UNITY_EDITOR
        Label($"Current Phase :{LevelManager.Current.CurrentPhaseIndex}{(LevelManager.Current.GetPhase()?.PhaseName ?? "null")}");
        #endif 
        Label($"Fps : {Math.Round(1f / Time.deltaTime)}");
        Label($"Letters on the screen : {ActionLetter.AliveOnes.Count}");
        Label($"State : {LevelManager.Current.LevelStatus}");
        Label($"Wpm : {(hits/5f)/((Time.time-timeAtStart)/60f)}");
        
    }
    private void DrawDmgHistory()
    {

        GUILayout.Label("Dmg history:", labelStyle);
        foreach (var element in dmgesAtTime)
        {
            GUILayout.Label($"{element.who} ({element.dmg}) : {FormatTime(element.time)}");
        }
    }
    
}
