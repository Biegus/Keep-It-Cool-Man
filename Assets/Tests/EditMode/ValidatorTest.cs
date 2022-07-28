using System.Runtime.Serialization;
using LetterBattle;
using LetterBattle.Utility;
using UnityEngine;
using UnityEngine.Assertions;
namespace Tests.EditMode
{
    public class ValidatorTest
    {
        public class TestObj
        {
            [ValidateNotNull] public UnityEngine.Object Field;
            
        }
        [NUnit.Framework.Test]
        public void NullDetectionTest()
        {
            TestObj obj = new TestObj();
            var res = ValidatorSystem.Validate(obj);
            Assert.IsFalse(res.IsOk);
            Assert.AreEqual(1,res.Descr.Count);
        }
        [NUnit.Framework.Test]
        public void CorrectObjectTest()
        {
            TestObj obj = new TestObj();
            obj.Field = new GameObject();
            var res = ValidatorSystem.Validate(obj);
            Assert.IsTrue(res.IsOk);
            UnityEngine.Object.DestroyImmediate(obj.Field);
        }
        
    }
}