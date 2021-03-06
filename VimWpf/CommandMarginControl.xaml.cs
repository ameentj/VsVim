﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vim.UI.Wpf
{
    /// <summary>
    /// Interaction logic for CommandMarginControl.xaml
    /// </summary>
    public partial class CommandMarginControl : UserControl
    {
        public static readonly DependencyProperty StatusLineProperty = DependencyProperty.Register(
            "StatusLine", 
            typeof(string),
            typeof(CommandMarginControl));

        public static readonly DependencyProperty RightStatusLineProperty = DependencyProperty.Register(
            "RightStatusLine",
            typeof(string),
            typeof(CommandMarginControl));

        /// <summary>
        /// The primary status line for Vim
        /// </summary>
        public string StatusLine
        {
            get { return (string)GetValue(StatusLineProperty); }
            set { SetValue(StatusLineProperty, value); }
        }

        public string RightStatusLine
        {
            get { return (string)GetValue(RightStatusLineProperty); }
            set { SetValue(StatusLineProperty, value); }
        }

        public CommandMarginControl()
        {
            InitializeComponent();
        }
    }
}
