using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using MathBase;

namespace MathPro.ProblemBuilder
{
    public class Keyboard: WrapPanel
    {
        #region Properties

        #endregion Properties

        #region Constructor

        public Keyboard(ref EventHandler handler)
        {
            this.Width = 730;
            this.Height = 290;
            this.Margin = new Thickness(43, 45, 0, 0);
            this.Children.Add(GetInner(ref handler));
        }

        #endregion Constructor

        #region Methods

        private ListBox GetInner(ref EventHandler handler)
        {
            ListBox box = new ListBox();
            box.Items.Add(GetNumbersVa(ref handler));
            box.Items.Add(GetSigns(ref handler));
            box.Items.Add(GetOther(ref handler));
            box.Items.Add(GetNumbersVa2(ref handler));
            box.Items.Add(GetOtherTwo(ref handler));
            return box;
        }

        private WrapPanel GetSigns(ref EventHandler handler)
        {
            WrapPanel panel = Hoster();

            string[] values = new string[] { "=", "+", "-", "*", "<", "<=", ">", ">=", "(", ")" };

            foreach (string value in values)
            {
                KeyboardSign curr = new KeyboardSign(value);
                curr.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(handler);
                panel.Children.Add(curr);
            }
            return panel;
        }
        private WrapPanel GetNumbersVa(ref EventHandler handler)
        {
            char[] arrayV = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            WrapPanel panel = Hoster();

            foreach (char myChar in arrayV)
            {
                NumberVar curr = new NumberVar(myChar);
                curr.Tap +=new EventHandler<System.Windows.Input.GestureEventArgs>(handler);
                panel.Children.Add(curr);
            }
            return panel;

        }
        private WrapPanel GetNumbersVa2(ref EventHandler handler)
        {
            char[] arrayV = new char[] { 'a', 'b',  't', 'u', 'v', 'w', 'x'};
            WrapPanel panel = Hoster();

            foreach (char myChar in arrayV)
            {
                NumberVar curr = new NumberVar(myChar);
                curr.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(handler);
                panel.Children.Add(curr);
            }
            return panel;

        }
        private WrapPanel Hoster()
        {
            WrapPanel panel = new WrapPanel();
            panel.Height = 57;
            //panel.Width = 730;
            return panel;
        }
        private WrapPanel GetOther(ref EventHandler handler)
        {
            WrapPanel panel = Hoster();

            KeyboardItem[] items = new KeyboardItem[] {KeyboardItem.Up, KeyboardItem.Down, KeyboardItem.DownSmall, KeyboardItem.UpSmall, KeyboardItem.Undo, KeyboardItem.f};

            foreach (KeyboardItem item in items)
            {
                Other curr = new Other(item);
                curr.Tap +=new EventHandler<System.Windows.Input.GestureEventArgs>(handler);
                panel.Children.Add(curr);
            }
                return panel;
        }
        private WrapPanel GetOtherTwo(ref EventHandler handler)
        {
            WrapPanel panel = Hoster();

           // Log, e, Cos, Sin, Tan, Cot, Sec, Scs
            KeyboardItem[] items = new KeyboardItem[] {KeyboardItem.Log, KeyboardItem.e, KeyboardItem.Sin, KeyboardItem.Cos, KeyboardItem.Tan,
                                                       KeyboardItem.Cot, KeyboardItem.Sec, KeyboardItem.Scs};

            foreach (KeyboardItem item in items)
            {
                Other curr = new Other(item);
                curr.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(handler);
                panel.Children.Add(curr);
            }
            return panel;
        }

        #endregion Methods

    }
    public class NumberVar : Grid, IKeyboardItemID
    {
        #region Properties

        char _myVariableNumberValue;

        public char MyVariableNumberValue
        {
            get { return _myVariableNumberValue; }
            set { _myVariableNumberValue = value; }
        }

        #endregion Properties

        #region Constructor

        public NumberVar(char var)
        {
            this._myVariableNumberValue = var;
            this.Height = 50;
            this.Width = 53;
            this.Background = GetBackGround("boarder");
            this.Children.Add(GetInner(var.ToString()));
        }

        #endregion Constructor

        #region Methods

        private Grid GetInner(string background)
        {
            Grid grid = new Grid();
            grid.Height = 26;
            grid.Width = 28;
            grid.Background = GetBackGround(background);
            return grid;
        }
        private ImageBrush GetBackGround(string name)
        {
            ImageBrush bi = new ImageBrush();
            System.Uri uri = new Uri(@"../Images/MathToolSet/" + name + ".png", UriKind.Relative);

            if (uri == null)// what if its not null but has no image?
            {
                return null;
            }

            bi.ImageSource = new BitmapImage(uri);
            return bi;
        }

        #endregion Methods

        public KeyBoardItemIDType GetID()
        {
            return KeyBoardItemIDType.NumberVar;
        }
    }
    public class KeyboardSign : Grid, IKeyboardItemID
    {
        #region Properties

        string myKey;

        public string MyKey
        {
            get { return myKey; }
            set { myKey = value; }
        }

        #endregion Properties

        #region Constructor

        public KeyboardSign(string key)
        {
            this.myKey = key;
            this.Height = 50;
            this.Width = 53;            
            this.Background = GetBackGround("boarder");
            this.Children.Add(GetInner(key));
        }

        #endregion Constructor

        #region Methods

        private Grid GetInner(string background)
        {
            Grid grid = new Grid();
            grid.Height = 26;
            grid.Width = 28;

            if (myKey == "-")
                grid.Height = 8;

            if (myKey != ")" && myKey != "(")
                grid.Background = GetBackGround(FixSign(new Sign(myKey)));
            else
            {
                if (myKey == "(")
                    grid.Background = GetBackGround("openBrace");
                else grid.Background = GetBackGround("closeBrace");
            }
            return grid;
        }

        private string FixSign(Sign signType)
        {
            if (signType.SignType == SignType.Add)
                return "plus";
            else if (signType.SignType == SignType.Subtract)
                return "minus";
            else if (signType.SignType == SignType.Divide)
                return "divide";
            else if (signType.SignType == SignType.equal)
                return "equal";
            else if (signType.SignType == SignType.greater)
                return "greaterthan";
            else if (signType.SignType == SignType.greaterEqual)
                return "greaterOrEqual";
            else if (signType.SignType == SignType.Multiply)
                return "multiply";
            else if (signType.SignType == SignType.OR)
                return "or";
            else if (signType.SignType == SignType.smaller)
                return "smallerthan";
            else if (signType.SignType == SignType.smallerEqual)
                return "smallerOrEqual";
            throw new NotImplementedException();
        }

        private ImageBrush GetBackGround(string name)
        {
            ImageBrush bi = new ImageBrush();
            System.Uri uri = new Uri(@"../Images/MathToolSet/" + name + ".png", UriKind.Relative);

            if (uri == null)// what if its not null but has no image?
            {
                return null;
            }

            bi.ImageSource = new BitmapImage(uri);
            return bi;
        }

        #endregion Methods

        public KeyBoardItemIDType GetID()
        {
            return KeyBoardItemIDType.KeyboardSign;
        }
    }
    public class Other : Grid, IKeyboardItemID
    {
        #region Properties
        KeyboardItem _myKeyboardItem;

        public KeyboardItem MyKeyboardItem
        {
            get { return _myKeyboardItem; }
            set { _myKeyboardItem = value; }
        }
        #endregion Properties

        #region Constructor

        public Other(KeyboardItem item)
        {
            this.Height = 50;
            this.Width = 53;
            this._myKeyboardItem = item;
            this.Background = GetBackGround("boarder");

            if (InLogTrigList(item))
            {
                this.Children.Add(GetInner(GetBackGround(handForLopTrigItem(item))));
            }
            else if (_myKeyboardItem == KeyboardItem.DownSmall ||
                _myKeyboardItem == KeyboardItem.UpSmall)
                this.Children.Add(GetInner());
            else if (_myKeyboardItem == KeyboardItem.Undo)
            {
                this.Children.Add(GetInner(GetBackGround(item.ToString().ToLower())));
            }
            else if (_myKeyboardItem == KeyboardItem.f)
            {
                this.Children.Add(GetInner(GetBackGround(item.ToString().ToLower())));
            }
            else
                this.Children.Add(GetInner(GetBackGround("arrow" + item.ToString().ToLower())));
        }

        #endregion Constructor

        #region Methods

        private Grid GetInner()
        {
            Grid grid = new Grid();
            grid.Height = 18;
            grid.Width = 18;
            if (_myKeyboardItem == KeyboardItem.UpSmall)
                grid.Background = GetBackGround("arrowUp");
            else grid.Background = GetBackGround("arrowDown");
            return grid;
        }
        private Grid GetInner(ImageBrush brush)
        {
            Grid grid = new Grid();
            grid.Height = 25;
            grid.Width = 25;
            grid.Background = brush;
            return grid;
        }
        private ImageBrush GetBackGround(string name)
        {
            ImageBrush bi = new ImageBrush();
            System.Uri uri = new Uri(@"../Images/MathToolSet/" + name + ".png", UriKind.Relative);

            if (uri == null)// what if its not null but has no image?
            {
                return null;
            }

            bi.ImageSource = new BitmapImage(uri);
            return bi;
        }
        #endregion Methods

        public bool InLogTrigList(KeyboardItem compare)
        {
            KeyboardItem[] items = new KeyboardItem[] {KeyboardItem.Log, KeyboardItem.e, KeyboardItem.Sin, KeyboardItem.Cos, KeyboardItem.Tan,
                                                       KeyboardItem.Cot, KeyboardItem.Sec, KeyboardItem.Scs};

            foreach (KeyboardItem item in items)
            {
                if (item == compare)
                    return true;
            }
            return false;
        }
        public string handForLopTrigItem(KeyboardItem item)
        {
            if (item == KeyboardItem.Log)
                return "log";
            else if (item == KeyboardItem.e)
                return "e";
            else if (item == KeyboardItem.Sin)
                return "sin";
            else if (item == KeyboardItem.Cos)
                return "cos";
            else if (item == KeyboardItem.Tan)
                return "tan";
            else if (item == KeyboardItem.Cot)
                return "cot";
            else if (item == KeyboardItem.Sec)
                return "sec";
            else if (item == KeyboardItem.Scs)
                return "cosec";
            else throw new NotImplementedException();
        }

        public KeyBoardItemIDType GetID()
        {
            return KeyBoardItemIDType.Other;
        }
    }
    public enum KeyboardItem
    {
        Left, Right, Up, Down, UpSmall, DownSmall, Undo, Log, e, Cos, Sin, Tan, Cot, Sec, Scs, f
    }
    public interface IKeyboardItemID
    {
        KeyBoardItemIDType GetID();
    }
    public enum KeyBoardItemIDType
    {
        Other, KeyboardSign, NumberVar
    }
}
