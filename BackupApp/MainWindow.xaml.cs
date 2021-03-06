﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Forms;
using BackupApp.Backup;
using System.IO;

namespace BackupApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackupMaker backup;

        public MainWindow()   
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            dialog.SelectedPath = "F:\\";
            dialog.Description = "Selecteer USB folder (verwisselbare schijf)";
            DialogResult result = dialog.ShowDialog();
            if (result ==  System.Windows.Forms.DialogResult.OK)
            {
                if (dialog.SelectedPath.StartsWith("C:"))
                {
                    System.Windows.MessageBox.Show("Selecteer USB folder (verwisselbare schijf).", "Niet toegelaten", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                btnCopy.IsEnabled = false;
                btnCancel.IsEnabled = true;
                backup = new BackupMaker(Log);
                backup.Start(dialog.SelectedPath);
            }
        }

        private void Log(string text)
        {
            Dispatcher.Invoke(() =>
            {
                tbLog.AppendText(DateTime.Now.ToString(CultureInfo.CurrentCulture) + ": " + text + Environment.NewLine);
                tbLog.ScrollToEnd();
            });
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (backup  != null)
            {
                backup.Cancel();
                backup = null;
            }

            btnCopy.IsEnabled = true;
            btnCancel.IsEnabled = false;
        }
    }
}
