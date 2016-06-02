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
using MathBase;
using System.Collections.Generic;
using Microsoft.Phone.Controls;


namespace DisplayEngine
{
    public enum DisplayTermProperty
    {
        IgnoreRoot, IgnoreSign, First, Divisor
    }
    public class DisplayTerm : Microsoft.Phone.Controls.WrapPanel 
    {
        #region Properties
        
        Term _source;
        DisplayTerm _divisor;
        List<TextBlock> _innerPieces;
        List<DisplayTermProperty>  _displayProperties;
        #endregion Properties

        #region Constructor

        public DisplayTerm(Term source, params DisplayTermProperty [] displayProperties)
        {
            this._source = source;

            this._displayProperties = new List<DisplayTermProperty>(displayProperties);

            List<DisplayTermProperty> divisorProperties = new List<DisplayTermProperty>(displayProperties);

            divisorProperties.Add(DisplayTermProperty.Divisor);
            if (source.Devisor != null && source.Devisor.Root > 1 && source.Root > 1)
                divisorProperties.Add(DisplayTermProperty.IgnoreRoot);


        }

        #endregion Constructor

        #region Method

        private void Prepare()
        {

        }

        #endregion Method
    }
}
