using System;
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
      get { return false; }
    }

    protected override string Description
    {
      get { return "Save screenshots to local files."; }
    }
    
    protected override Output CreateOutput(IWin32Window Owner)
    {

      Output output = new Output(Name,
                                 "Screenshot",
                                 String.Empty);

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
                          edit.FileName,
                          edit.FileFormat);
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
      outputValues.Add(new OutputValue("FileName", Output.FileName));
      outputValues.Add(new OutputValue("FileFormat", Output.FileFormat));

      return outputValues;
      
    }

    protected override Output DeserializeOutput(OutputValueCollection OutputValues)
    {
      return new Output(OutputValues["Name", this.Name].Value,
                        OutputValues["FileName", "Screenshot"].Value,
                        OutputValues["FileFormat", ""].Value);
    }

    protected async override Task<V3.SendResult> Send(Output Output, V3.ImageData ImageData)
    {
      try
      {

        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
        {

          if (saveFileDialog.ShowDialog() == DialogResult.OK)
          {

            V3.FileData fileData = V3.FileHelper.GetFileData(Output.FileName, Output.FileFormat, ImageData);

            using (FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.ReadWrite))
            {
              file.Write(fileData.FileBytes, 0, fileData.FileBytes.Length);
              file.Close();
            }

          }
          else
          {
            return new V3.SendResult(V3.Result.Canceled);
          }
        }
       
        return new V3.SendResult(V3.Result.Success);

      }
      catch (Exception ex)
      {
        return new V3.SendResult(V3.Result.Failed, ex.Message);
      }

    }
      
  }

}