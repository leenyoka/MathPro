using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MathBase;

namespace MathPro.Display
{
    public class TermUtility
    {
        public int FixSign(Sign signType)
        {
            if (signType.SignType == SignType.Add)
                return Resource.Drawable.plus;
            if (signType.SignType == SignType.Subtract)
                return Resource.Drawable.minus;
            if (signType.SignType == SignType.Divide)
                return Resource.Drawable.divide;
            if (signType.SignType == SignType.equal)
                return Resource.Drawable.equal;
            if (signType.SignType == SignType.greater)
                return Resource.Drawable.greaterthan;
            if (signType.SignType == SignType.greaterEqual)
                return Resource.Drawable.greaterOrEqual;
            if (signType.SignType == SignType.Multiply)
                return Resource.Drawable.multiply;
            if (signType.SignType == SignType.Or)
                return Resource.Drawable.or;
            if (signType.SignType == SignType.smaller)
                return Resource.Drawable.smallerthan;
            if (signType.SignType == SignType.smallerEqual)
                return Resource.Drawable.SmallerOrEqual;
            throw new NotImplementedException();
        }
        public int GetBackgroundResourceNumOrChar(char value)
        {
            switch (value)
            {
                case 'a':
                    return Resource.Drawable.A;
                case 'b':
                    return Resource.Drawable.B;
                case 'c':
                    return Resource.Drawable.C;
                case 'd':
                    return Resource.Drawable.D;
                case 'e':
                    return Resource.Drawable.E;
                case 'f':
                    return Resource.Drawable.F;
                case 'g':
                    return Resource.Drawable.G;
                case 'h':
                    return Resource.Drawable.H;
                case 'i':
                    return Resource.Drawable.I;
                case 'j':
                    return Resource.Drawable.J;
                case 'k':
                    return Resource.Drawable.K;
                case 'l':
                    return Resource.Drawable.L;
                case 'm':
                    return Resource.Drawable.M;
                case 'n':
                    return Resource.Drawable.N;
                case 'o':
                    return Resource.Drawable.O;
                case 'p':
                    return Resource.Drawable.P;
                case 'q':
                    return Resource.Drawable.Q;
                case 'r':
                    return Resource.Drawable.R;
                case 's':
                    return Resource.Drawable.S;
                case 't':
                    return Resource.Drawable.T;
                case 'u':
                    return Resource.Drawable.U;
                case 'v':
                    return Resource.Drawable.V;
                case 'w':
                    return Resource.Drawable.W;
                case 'x':
                    return Resource.Drawable.X;
                case '1':
                    return Resource.Drawable.One;
                case '2':
                    return Resource.Drawable.Two;
                case '3':
                    return Resource.Drawable.Three;
                case '4':
                    return Resource.Drawable.Four;
                case '5':
                    return Resource.Drawable.Five;
                case '6':
                    return Resource.Drawable.Six;
                case '7':
                    return Resource.Drawable.Seven;
                case '8':
                    return Resource.Drawable.Eight;
                case '9':
                    return Resource.Drawable.Nine;
                case '0':
                    return Resource.Drawable.Zero;
                default:
                    return -1;

            }
        }

	    public int GetBackgroundResourceNumOrChar(string value)
	    {
		    switch (value.ToLower())
		    {
			    case "a":
				    return Resource.Drawable.A;
			    case "b":
				    return Resource.Drawable.B;
			    case "c":
				    return Resource.Drawable.C;
			    case "d":
				    return Resource.Drawable.D;
			    case "e":
				    return Resource.Drawable.E;
			    case "f":
				    return Resource.Drawable.F;
			    case "g":
				    return Resource.Drawable.G;
			    case "h":
				    return Resource.Drawable.H;
			    case "i":
				    return Resource.Drawable.I;
			    case "j":
				    return Resource.Drawable.J;
			    case "k":
				    return Resource.Drawable.K;
			    case "l":
				    return Resource.Drawable.L;
			    case "m":
				    return Resource.Drawable.M;
			    case "n":
				    return Resource.Drawable.N;
			    case "o":
				    return Resource.Drawable.O;
			    case "p":
				    return Resource.Drawable.P;
			    case "q":
				    return Resource.Drawable.Q;
			    case "r":
				    return Resource.Drawable.R;
			    case "s":
				    return Resource.Drawable.S;
			    case "t":
				    return Resource.Drawable.T;
			    case "u":
				    return Resource.Drawable.U;
			    case "v":
				    return Resource.Drawable.V;
			    case "w":
				    return Resource.Drawable.W;
			    case "x":
				    return Resource.Drawable.X;
			    case "1":
				    return Resource.Drawable.One;
			    case "2":
				    return Resource.Drawable.Two;
			    case "3":
				    return Resource.Drawable.Three;
			    case "4":
				    return Resource.Drawable.Four;
			    case "5":
				    return Resource.Drawable.Five;
			    case "6":
				    return Resource.Drawable.Six;
			    case "7":
				    return Resource.Drawable.Seven;
			    case "8":
				    return Resource.Drawable.Eight;
			    case "9":
				    return Resource.Drawable.Nine;
			    case "0":
				    return Resource.Drawable.Zero;
				case "arrowdown":
					return Resource.Drawable.ArrowDown;
				case "arrowleft":
					return Resource.Drawable.ArrowLeft;
				case "arrowright":
					return Resource.Drawable.ArrowRight;
				case "arrowup":
					return Resource.Drawable.ArrowUp;
				case "closebrace":
					return Resource.Drawable.closeBrace;
				case "closebracesquare":
					return Resource.Drawable.closeBraceSquare;
				case "cos":
					return Resource.Drawable.cos;
				case "cosec":
					return Resource.Drawable.cosec;
				case "cot":
					return Resource.Drawable.cot;
				case "cursor":
					return Resource.Drawable.Cursor;
				case "divide":
					return Resource.Drawable.divide;
				case "equal":
					return Resource.Drawable.equal;
				case "greaterorequal":
					return Resource.Drawable.greaterOrEqual;
				case "greaterthan":
					return Resource.Drawable.greaterthan;
				case "line":
					return Resource.Drawable.Line;
				case "minus":
					return Resource.Drawable.minus;
				case "multiply":
					return Resource.Drawable.multiply;
				case "openbrace":
					return Resource.Drawable.openBrace;
				case "plus":
					return Resource.Drawable.plus;
				case "plusminus":
					return Resource.Drawable.plusMinus;
				case "root":
					return Resource.Drawable.root;
				case "sec":
					return Resource.Drawable.sec;
				case "sin":
					return Resource.Drawable.sin;
				case "smallerorequal":
					return Resource.Drawable.SmallerOrEqual;
				case "smallerthan":
					return Resource.Drawable.smallerthan;
				case "tan":
					return Resource.Drawable.tan;
				case "termmultiply":
					return Resource.Drawable.termMultiply;
				case "undo":
					return Resource.Drawable.undo;
				case  "boarder":
				    return Resource.Drawable.border;
				case "log":
					return Resource.Drawable.Log;
					
			    default:
				    return -1;

		    }

	    }
    }
}