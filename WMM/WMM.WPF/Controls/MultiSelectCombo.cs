﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WMM.WPF.Controls
{
    public class MultiSelectCombo : Control
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items", typeof(List<ISelectableItem>), typeof(MultiSelectCombo), new PropertyMetadata(default(List<ISelectableItem>)));

        public List<ISelectableItem> Items
        {
            get => (List<ISelectableItem>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(
            "DisplayText", typeof(string), typeof(MultiSelectCombo), new PropertyMetadata(default(string)));



    }
}