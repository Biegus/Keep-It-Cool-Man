using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
namespace Cyberultimate.Unity
{
    public class CyberCommandSystem
    {
        public static ReadOnlyDictionary<string, CyberCommand> Commands => new ReadOnlyDictionary<string, CyberCommand>(commands);
        
        private static readonly Dictionary<string, CyberCommand> commands;
        public static event EventHandler<CallComendArgs> OnEnterCommand = delegate { };
         static CyberCommandSystem()
        {
            Debug.Log("Refreshing commands..");
            //kinda messy huh? I agree
            bool CheckIfHasAny(Type type)
            {
                try
                {
                    return type.GetCustomAttributes(true).OfType<CommandContainerAttribute>().Any();
                }
                catch
                {
                    return false;
                }
                 
            }
            Delegate CreateDelegate(MethodInfo method)
            {
                if (method.GetParameters().Length == 0)
                {
                    var temp= (Action) method.CreateDelegate(typeof(Action));
                    return (Action<string[]>)((_) =>
                    {
                        temp();
                    });

                }
                return method.CreateDelegate((method.ReturnType == typeof(string))
                    ? typeof(Func<string[], string>) : typeof(Action<string[]>));
            }
            var containers = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where CheckIfHasAny(type)
                select type).ToArray();
            
            commands = (
                    from type in containers
                    from method in type.GetMethods()
                    let atr = method.GetCustomAttributes(true)
                        .OfType<CyberCommandAttribute>().FirstOrDefault()
                    where atr != null
                    select  new CyberCommand(atr,CreateDelegate(method)))
                .Union((
                        from type in containers
                        from property in type.GetProperties( BindingFlags.Static |
                                                            BindingFlags.Public | BindingFlags.NonPublic)
                        let atr = property.GetCustomAttributes(true)
                            .OfType<CommandPropertyAttribute>().FirstOrDefault()
                        where atr != null
                        let  realName=string.IsNullOrEmpty(atr.Name)?$"{property.DeclaringType.Name}.{property.PropertyType}":
                            atr.Name
                        select new CyberCommand[]
                        {
                            (atr.Get)
                                ? new CyberCommand(new ManualMetaData((string) $"get_{realName}",(string[]) null,GameState.Both),
                                    new Func<string[],string>( (args)=> property.GetMethod.Invoke(null, null).ToString()))
                                : null,
                            (atr.Set)
                                ? new CyberCommand(new ManualMetaData((string) $"set_{realName}",
                                        new[] {"value"},GameState.Both),
                                    new Action<string[]>((args) =>
                                    {
                                        property.SetMethod.Invoke(null, new object[] {ParserHelper.Parse( args[0],property.PropertyType)});
                                    }))
                                : null,
                        }

                    )
                    .SelectMany(item => item)
                    .Where(item => item != null))
                .OrderBy(item=>item.MetaData.Name)
                .ToDictionary
                    (c => c.MetaData.Name, StringComparer.OrdinalIgnoreCase);

        }
          public static string CallCommand(string code)
        {
            StringBuilder builder = new StringBuilder();
            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }
            
            int index = code.IndexOf(' ');
            if (index == -1)
                index = code.Length;
            string commandText = code.Substring(0,index);
          
            string? errorCode = null;
            Exception? exception = null;
            CyberCommand? commandClass = null;
            string[]? args = null;
            if (commands.ContainsKey(commandText) == false)
            {
                errorCode = "Not found";
            }
            else
            {
                
                try
                {
                 
                    string argsInternal = code.Substring(commandText.Length, code.Length - commandText.Length);
                    Regex regex= new Regex("\".*?\"");
                     args = regex.Matches(argsInternal)
                        .OfType<object>().Select(item=>
                        {
                            string txt = item.ToString();
                            return txt.Substring(1, txt.Length - 2);
                        }).ToArray();
                    
                    commandClass = commands[commandText];
                    if ((GameStateHelper.GetGameState() & commandClass.MetaData.GameState )== GameState.None)
                    {
                        errorCode = "Wrong game state";
                    }
                    else
                    {
                        string result = commandClass.Call(args);
                        if (!string.IsNullOrEmpty(result))
                            builder.Append($"{(result)}\n");
                    }
                   
                }
                catch (TargetInvocationException e)
                {
                    errorCode = e.InnerException!.Message;
                    exception = e;
                }
                catch (Exception e)
                {
                    errorCode = e.Message;
                    exception = e;
                }
                finally
                {
                    OnEnterCommand(null, new CallComendArgs(commandClass, code, args!, exception, errorCode));
                }


            }

            if (errorCode != null)
                builder.AppendLine($"<color=red>{errorCode}</color>");
            return builder.ToString();
        }
          public static IEnumerable<CyberCommand > GetTips(string command)
          {
              string upper = command.ToUpper();

              return from element in CyberCommandSystem.Commands
                  let upperKey = element.Key.ToUpper()
                  let spaceIndex = upper.IndexOf(' ')
                  where upperKey.Contains(upper) || (spaceIndex != -1 && upperKey == upper.Substring(0, spaceIndex))
                  select element.Value;
          }
          public static CyberCommand TakeCommand(string code)
          {
              if (code is null)
              {
                  throw new ArgumentNullException(nameof(code));
              }

              return commands[code];
          }
    }
}