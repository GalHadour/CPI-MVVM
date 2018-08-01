﻿using MahApps.Metro.Controls;
using System.Windows;


namespace CPI.Views
{
    class CustomMenuIconItem : HamburgerMenuIconItem
    {
        public static readonly DependencyProperty ToolTipProperty = DependencyProperty.Register(
            "ToolTip",
            typeof(object),
            typeof(CustomMenuIconItem),
            new PropertyMetadata(null));

        public object ToolTip
        {
            get { return (object)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }
    }
}
