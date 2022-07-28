#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
namespace Cyberultimate.Unity
{
    /// <summary>
    /// <see cref="Color"/> version for easy expresions. 
    /// </summary>
    [Serializable]
    public struct Clr
    {
        public float R;
        public float G;
        public float B;
        public float A;
        public Clr(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        public Clr(float r, float g, float b)
            : this(r, g, b, 1)
        {

        }
        public Clr(Clr other)
            : this(other.R, other.G, other.B, other.A)
        {

        }
        public Clr(Clr other, float alpha)
            : this(other)
        {
            A = alpha;
        }
        public Clr((float r, float g, float b, float a) tuple)
            : this(tuple.r, tuple.g, tuple.b, tuple.a)
        {

        }
        public void Deconstruct(out float r, out float g, out float b, out float a)
        {
            r = R;
            g = G;
            b = B;
            a = A;
        }
        public float[] GetArrays()
        {
            return new float[] { R, G, B, A };
        }
       
        public Vector4 GetVector4()
        {
            return new Vector4(R, G, B, A);
        }
        public Vector3 GetVector3()
        {
            return new Vector3(R, G, B);
        }
        public Clr CR(float r)
        {
            return new Clr(this) { R=r };
        }
        public Clr CG(float g)
        {
            return new Clr(this) { G=g };
        }
        public Clr CB(float b)
        {
            return new Clr(this) { B = b };
        }
        public Clr CA(float a)
        {
            return new Clr(this) { A = a };
        }
        public static implicit operator Color(Clr clr)
        {
            return new Color(clr.R, clr.G, clr.B, clr.A);
        }
        public static implicit operator Clr(Color color)
        {
            return new Clr(color.r, color.g, color.b, color.a);
        }
        public static implicit operator Clr((float r, float g, float b, float a) color)
        {
            return new Clr(color.r, color.g, color.b, color.a);
        }
        public static implicit operator Clr((float r, float g, float b) color)
        {
            return new Clr(color.r, color.g, color.b, 1);
        }


    }
}
