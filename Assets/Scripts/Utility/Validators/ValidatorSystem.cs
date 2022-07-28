using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cyberultimate;
using LetterBattle.Utility;
using UnityEngine;
using UObject = UnityEngine.Object;
namespace LetterBattle.Utility
{
    public class AlwaysValidateRecursively: Attribute{}
   
    public readonly struct ValidateResult
    {
        public static readonly ValidateResult Ok  = new ValidateResult(null);
        
        public const string ERROR_PRE_HEADER = "Validation failed: Details below:";
        public const string OK_PRE_HEADER = "Validation succeeded";

        public bool IsOk => Descr == null || Descr.Count == 0;
        public readonly IReadOnlyCollection<object> Descr;// can be null
        public ValidateResult( IReadOnlyCollection<object> descr)
        {
            this.Descr = descr;
        }
        public static ValidateResult One(object obj) => new ValidateResult(new object[]{obj});
        public override string ToString() =>
            IsOk 
                ? OK_PRE_HEADER 
                : $"{ERROR_PRE_HEADER}\n {GetDetails()}";

        public string GetDetails()
        {
            if (IsOk) return string.Empty;
            return (Descr.BuildString((value, id) =>
                {
                    if (!(value is ValidateResult))
                        return $"{value}\n";
                    else return $"{((ValidateResult)value).GetDetails()}\n";
                }
                , true));
        }
    }
    public class ValidatorSystem
    {
        public static ValidateResult Validate(object obj, string placeConnector="")
        {
            HashSet<object> set = new HashSet<object>();
            List<object> complains = new List<object>();
            Validate(obj, placeConnector, set, complains);
            if(complains.Count==0)
                return ValidateResult.Ok;
            else
                return new ValidateResult( complains);
        }
        public static bool ValidateAndPrint(object obj, string placeConnector)
        {
            var res = Validate(obj, placeConnector);
            if (!res.IsOk)
                Debug.LogError(res);
            return res.IsOk;
        }
        public static void Validate(object obj, string placeConnector, ISet<object> used, IList<object> complains)
        {

            if (used.Contains(obj)) return;
            if (obj == null) return;
            used.Add(obj);
            Type type = obj.GetType();
            
            string preName;
            if (placeConnector.Length > 0)
                preName = $"{placeConnector}.";
            else preName = string.Empty;

            if (obj is IValidator validator)
            {
                var result = validator.Validate(preName);
                if (!result.IsOk)
                    complains.Add(result);
            }
            
            foreach (var field in type.GetFields(BindingFlags.Instance| BindingFlags.Public| BindingFlags.NonPublic | BindingFlags.GetField))
            {
                string fullName = $"{preName}{field.Name}";
               ValidateNotNull notNullValidate= field.GetCustomAttribute<ValidateNotNull>();
                ValidateRecursively recursivelyValidate = field.GetCustomAttribute<ValidateRecursively>();
                AlwaysValidateRecursively insideAttribute = field.FieldType.GetCustomAttribute<AlwaysValidateRecursively>();
                
                if (notNullValidate == null && insideAttribute == null  && recursivelyValidate ==null) continue;

                // unity object for instance have special null rule that is defined in static ==
                object value = field.GetValue(obj);
                
                if ( notNullValidate!=null && ( value == null || 
                    (value as UnityEngine.Object  == null && value is UnityEngine.Object) ))
                {
                    complains.Add($"{fullName} was null, which is not allowed");
                    return;
                }
                
                if (!( value is IEnumerable))
                {
                    if (recursivelyValidate != null || insideAttribute != null)
                        Validate(value, fullName, used, complains);
                }
                else if (value is IEnumerable en) //yea it's the same thing twice, but ordering in this way makes it easier to read
                {
                    int index = 0;
                    Type generic = field.FieldType.IsArray ? field.FieldType.GetElementType() : field.FieldType.GenericTypeArguments[0];
                    
                    foreach (var element in en)
                    {
                        if ( notNullValidate!=null && ( value == null || 
                                                        (value as UnityEngine.Object  == null && value is UnityEngine.Object) ) )
                        {
                            complains.Add($"{fullName}[{index++}] was null");
                        }
                        else if (recursivelyValidate!=null || insideAttribute!=null) 
                        {
                            Validate(element, $"{fullName}[{index++}]", used, complains);
                        }
                    }
                }

                
            }
        }
    }
}