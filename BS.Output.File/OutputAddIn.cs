using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace BS.Output.File
{
  public class OutputAddIn: V3.OutputAddIn<Output>
  {

    protected override string Name
    {
      get { return "Save to file"; }
    }

    protected override Image Image64
    {
      get  { return Properties.Resources.logo_64; }
    }

    protected override Image Image16
    {
      get { return Properties.Resources.logo_16 ; }
    }

    protected override bool Editable
    {
      get { return true; }
    }

    protected override string Description
    {
      get { return "Save screenshots to local files."; }
    }
    
    protected override Output CreateOutput(IWin32Window Owner)
    {

      Output output = new Output(Name,
                                 string.Empty,     
                                 "Screenshot",
                                 String.Empty,
                                 false);

      return EditOutput(Owner, output);

    }

    protected override Output EditOutput(IWin32Window Owner, Output Output)
    {

      Edit edit = new Edit(Output);

      var ownerHelper = new System.Windows.Interop.WindowInteropHelper(edit);
      ownerHelper.Owner = Owner.Handle;

      if (edit.ShowDialog() == true)
      {

        return new Output(edit.OutputName,
                          edit.Directory,
                          edit.FileName,
                          edit.FileFormat,
                          edit.SaveAutomatically);
      }
      else
      {
        return null;
      }

    }

    protected override OutputValueCollection SerializeOutput(Output Output)
    {

      OutputValueCollection outputValues = new OutputValueCollection();

      outputValues.Add(new OutputValue("Name", Output.Name));
      outputValues.Add(new OutputValue("Directory", Output.Directory));
      outputValues.Add(new OutputValue("FileName", Output.FileName));
      outputValues.Add(new OutputValue("FileFormat", Output.FileFormat));
      outputValues.Add(new OutputValue("SaveAutomatically", Output.SaveAutomatically.ToString()));

      return outputValues;
      
    }

    protected override Output DeserializeOutput(OutputValueCollection OutputValues)
    {
      return new Output(OutputValues["Name", this.Name].Value,
                        OutputValues["Directory", ""].Value,
                        OutputValues["FileName", "Screenshot"].Value,
                        OutputValues["FileFormat", ""].Value,
                        Convert.ToBoolean(OutputValues["SaveAutomatically", false.ToString()].Value));
    }

    protected async override Task<V3.SendResult> Send(IWin32Window Owner, Output Output, V3.ImageData ImageData)
    {
      try
      {

        string fileFormat = Output.FileFormat;
        string fileName = V3.FileHelper.GetFileName(Output.FileName, fileFormat, ImageData); ;
        string filePath;

        if (Output.SaveAutomatically)
        {
          filePath = Path.Combine(Output.Directory, fileName + "." + V3.FileHelper.GetFileExtention(fileFormat));
        }
        else
        {
          
          using (SaveFileDialog saveFileDialog = new SaveFileDialog())
          {

            List<string> fileFormats = V3.FileHelper.GetFileFormats();

            saveFileDialog.InitialDirectory = Output.Directory;
            saveFileDialog.FileName = fileName;

            List<string> filter = new List<string>();
            foreach (string filterFileFormat in fileFormats)
            {
              string fileExtention = V3.FileHelper.GetFileExtention(filterFileFormat);
              filter.Add(filterFileFormat + "-File (*." + fileExtention + ")|*." + fileExtention);
            }
            saveFileDialog.Filter = string.Join("|", filter);
            saveFileDialog.FilterIndex = fileFormats.IndexOf(fileFormat) + 1;

            if (saveFileDialog.ShowDialog(Owner) != DialogResult.OK)
              return new V3.SendResult(V3.Result.Canceled);

            fileFormat = fileFormats[saveFileDialog.FilterIndex - 1];
            filePath = saveFileDialog.FileName;

          }
        }

        Byte[] fileBytes = V3.FileHelper.GetFileBytes(fileFormat, ImageData);

        using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
        {
          file.Write(fileBytes, 0, fileBytes.Length);
          file.Close();
        }

        return new V3.SendResult(V3.Result.Success, filePath);

      }
      catch (Exception ex)
      {
        return new V3.SendResult(V3.Result.Failed, ex.Message);
      }

    }
      
  }

}