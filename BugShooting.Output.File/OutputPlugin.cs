using BS.Plugin.V3.Common;
using BS.Plugin.V3.Output;
using BS.Plugin.V3.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BugShooting.Output.File
{
  public class OutputPlugin: OutputPlugin<Output>
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
                                 FileHelper.GetFileFormats().First().ID,
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
                          edit.FileFormatID,
                          edit.SaveAutomatically);
      }
      else
      {
        return null;
      }

    }

    protected override OutputValues SerializeOutput(Output Output)
    {

      OutputValues outputValues = new OutputValues();

      outputValues.Add("Name", Output.Name);
      outputValues.Add("Directory", Output.Directory);
      outputValues.Add("FileName", Output.FileName);
      outputValues.Add("FileFormatID", Output.FileFormatID.ToString());
      outputValues.Add("SaveAutomatically", Output.SaveAutomatically.ToString());

      return outputValues;
      
    }

    protected override Output DeserializeOutput(OutputValues OutputValues)
    {
      return new Output(OutputValues["Name", this.Name],
                        OutputValues["Directory", ""],
                        OutputValues["FileName", "Screenshot"],
                        new Guid(OutputValues["FileFormatID", ""]),
                        Convert.ToBoolean(OutputValues["SaveAutomatically", false.ToString()]));
    }

    protected async override Task<SendResult> Send(IWin32Window Owner, Output Output, ImageData ImageData)
    {
      try
      {

        IFileFormat fileFormat = FileHelper.GetFileFormat(Output.FileFormatID);
        string fileName = AttributeHelper.ReplaceAttributes(Output.FileName,  ImageData); ;
        string filePath;

        if (Output.SaveAutomatically)
        {
          filePath = Path.Combine(Output.Directory, fileName + "." + fileFormat.FileExtension);
        }
        else
        {
          
          using (SaveFileDialog saveFileDialog = new SaveFileDialog())
          {

            List<IFileFormat> fileFormats = new List<IFileFormat>(FileHelper.GetFileFormats());

            saveFileDialog.InitialDirectory = Output.Directory;
            saveFileDialog.FileName = fileName;

            List<string> filter = new List<string>();
            foreach (IFileFormat filterFileFormat in fileFormats)
            {
              filter.Add(filterFileFormat.Name + "-File (*." + filterFileFormat.FileExtension + ")|*." + filterFileFormat.FileExtension);
            }
            saveFileDialog.Filter = string.Join("|", filter);
            saveFileDialog.FilterIndex = fileFormats.IndexOf(fileFormat) + 1;

            if (saveFileDialog.ShowDialog(Owner) != DialogResult.OK)
              return new SendResult(Result.Canceled);

            fileFormat = fileFormats[saveFileDialog.FilterIndex - 1];
            filePath = saveFileDialog.FileName;

          }
        }

        Byte[] fileBytes = FileHelper.GetFileBytes(fileFormat.ID, ImageData);

        using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
        {
          file.Write(fileBytes, 0, fileBytes.Length);
          file.Close();
        }

        return new SendResult(Result.Success, filePath);

      }
      catch (Exception ex)
      {
        return new SendResult(Result.Failed, ex.Message);
      }

    }
      
  }

}