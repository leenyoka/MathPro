﻿#pragma checksum "C:\Users\Linda\Desktop\MathPro\MathPro\Geometry\GeometryProblemBuilder.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5FF9A782EC98878FA6BCE2D9F90EDE72"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace MathPro.Geometry {
    
    
    public partial class GeometryProblemBuilder : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.ListBox PointsList;
        
        internal System.Windows.Controls.TextBlock txtAdded;
        
        internal System.Windows.Controls.RadioButton rdReflection;
        
        internal System.Windows.Controls.RadioButton rdRotation;
        
        internal System.Windows.Controls.RadioButton rdTranslation;
        
        internal System.Windows.Controls.RadioButton rdEnlargement;
        
        internal Microsoft.Phone.Controls.WrapPanel panelReflection;
        
        internal System.Windows.Controls.RadioButton rdXAxis;
        
        internal System.Windows.Controls.RadioButton rdYAxis;
        
        internal System.Windows.Controls.RadioButton rdXequalsY;
        
        internal Microsoft.Phone.Controls.WrapPanel panelRotation;
        
        internal System.Windows.Controls.RadioButton rdNinty;
        
        internal System.Windows.Controls.RadioButton rdHundredAndEighty;
        
        internal System.Windows.Controls.RadioButton rdClockwise;
        
        internal System.Windows.Controls.RadioButton rdAntiClockwise;
        
        internal Microsoft.Phone.Controls.WrapPanel panelEnlargement;
        
        internal System.Windows.Controls.TextBox txtEnlargement;
        
        internal Microsoft.Phone.Controls.WrapPanel panelTranslation;
        
        internal System.Windows.Controls.TextBox txtVertical;
        
        internal System.Windows.Controls.TextBox txtHorizontal;
        
        internal System.Windows.Controls.Button btnAdd;
        
        internal System.Windows.Controls.Button btnScetch;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/MathPro;component/Geometry/GeometryProblemBuilder.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.PointsList = ((System.Windows.Controls.ListBox)(this.FindName("PointsList")));
            this.txtAdded = ((System.Windows.Controls.TextBlock)(this.FindName("txtAdded")));
            this.rdReflection = ((System.Windows.Controls.RadioButton)(this.FindName("rdReflection")));
            this.rdRotation = ((System.Windows.Controls.RadioButton)(this.FindName("rdRotation")));
            this.rdTranslation = ((System.Windows.Controls.RadioButton)(this.FindName("rdTranslation")));
            this.rdEnlargement = ((System.Windows.Controls.RadioButton)(this.FindName("rdEnlargement")));
            this.panelReflection = ((Microsoft.Phone.Controls.WrapPanel)(this.FindName("panelReflection")));
            this.rdXAxis = ((System.Windows.Controls.RadioButton)(this.FindName("rdXAxis")));
            this.rdYAxis = ((System.Windows.Controls.RadioButton)(this.FindName("rdYAxis")));
            this.rdXequalsY = ((System.Windows.Controls.RadioButton)(this.FindName("rdXequalsY")));
            this.panelRotation = ((Microsoft.Phone.Controls.WrapPanel)(this.FindName("panelRotation")));
            this.rdNinty = ((System.Windows.Controls.RadioButton)(this.FindName("rdNinty")));
            this.rdHundredAndEighty = ((System.Windows.Controls.RadioButton)(this.FindName("rdHundredAndEighty")));
            this.rdClockwise = ((System.Windows.Controls.RadioButton)(this.FindName("rdClockwise")));
            this.rdAntiClockwise = ((System.Windows.Controls.RadioButton)(this.FindName("rdAntiClockwise")));
            this.panelEnlargement = ((Microsoft.Phone.Controls.WrapPanel)(this.FindName("panelEnlargement")));
            this.txtEnlargement = ((System.Windows.Controls.TextBox)(this.FindName("txtEnlargement")));
            this.panelTranslation = ((Microsoft.Phone.Controls.WrapPanel)(this.FindName("panelTranslation")));
            this.txtVertical = ((System.Windows.Controls.TextBox)(this.FindName("txtVertical")));
            this.txtHorizontal = ((System.Windows.Controls.TextBox)(this.FindName("txtHorizontal")));
            this.btnAdd = ((System.Windows.Controls.Button)(this.FindName("btnAdd")));
            this.btnScetch = ((System.Windows.Controls.Button)(this.FindName("btnScetch")));
        }
    }
}

