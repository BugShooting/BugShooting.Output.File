using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BS.Plugin.V3.Utilities;

namespace BugShooting.Output.File
{
  partial class Edit : Window
  {
    public Edit(Output output)
    {
      InitializeComponent();

      foreach (string fileNameReplacement in AttributeHelper.GetAttributeReplacements())
      {
        MenuItem item = new MenuItem();
        item.Header = new TextBlock() { Text = fileNameReplacement };
        item.Tag = fileNameReplacement;
        item.Click += FileNameReplacementItem_Click;
        FileNameReplacementList.Items.Add(item);
      }

      IEnumerable<string> fileFormats = FileHelper.GetFileFormats();
      foreach (string fileFormat in fileFormats)
      {
        ComboBoxItem item = new ComboBoxItem();
        item.Content = fileFormat;
        item.Tag = fileFormat;
        FileFormatComboBox.Items.Add(item);
      }

      NameTextBox.Text = output.Name;
      DirectoryTextBox.Text = output.Directory;
      FileNameTextBox.Text = output.FileName;

      if (fileFormats.Contains(output.FileFormat))
      {
        FileFormatComboBox.SelectedValue = output.FileFormat;
      }
      else
      {
        FileFormatComboBox.SelectedValue = fileFormats.First();
      }

      SaveAutomaticallyCheckBox.IsChecked = output.SaveAutomatically;

      NameTextBox.TextChanged += ValidateData;
      FileFormatComboBox.SelectionChanged += ValidateData;
      ValidateData(null, null);

      DirectoryTextBox.Focus();

    }

    public string OutputName
    {
      get { return NameTextBox.Text; }
    }
      
    public string Directory
    {
      get { return DirectoryTextBox.Text; }
    }
  
    public string FileName
    {
      get { return FileNameTextBox.Text; }
    }
  
    public string FileFormat
    {
      get { return (string)FileFormatComboBox.SelectedValue; }
    }
   
    public bool SaveAutomatically
    {
      get { return SaveAutomaticallyCheckBox.IsChecked.Value; }
    }
  
    private void SelectDirectory_Click(object sender, RoutedEventArgs e)
    {

      using (System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog())
      {

        folderBrowserDialog.SelectedPath = Directory;

        if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
          DirectoryTextBox.Text = folderBrowserDialog.SelectedPath;
        }
      }

    }

    private void FileNameReplacement_Click(object sender, RoutedEventArgs e)
    {
      FileNameReplacement.ContextMenu.IsEnabled = true;
      FileNameReplacement.ContextMenu.PlacementTarget = FileNameReplacement;
      FileNameReplacement.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
      FileNameReplacement.ContextMenu.IsOpen = true;
    }

    private void FileNameReplacementItem_Click(object sender, RoutedEventArgs e)
    {

      MenuItem item = (MenuItem)sender;

      int selectionStart = FileNameTextBox.SelectionStart;

      FileNameTextBox.Text = FileNameTextBox.Text.Substring(0, FileNameTextBox.SelectionStart) + item.Tag.ToString() + FileNameTextBox.Text.Substring(FileNameTextBox.SelectionStart, FileNameTextBox.Text.Length - FileNameTextBox.SelectionStart);

      FileNameTextBox.SelectionStart = selectionStart + item.Tag.ToString().Length;
      FileNameTextBox.Focus();

    }

    private void ValidateData(object sender, RoutedEventArgs e)
    {
      OK.IsEnabled = Validation.IsValid(NameTextBox) &&
                     Validation.IsValid(FileFormatComboBox);
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }
    
  }
}
