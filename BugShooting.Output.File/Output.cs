using BS.Plugin.V3.Output;
using System;

namespace BugShooting.Output.File
{

  public class Output: IOutput 
  {
    
    string name;
    string directory;
    string fileName;
    Guid fileFormatID;
    bool saveAutomatically;

    public Output(string name,
                  string directory,
                  string fileName,
                  Guid fileFormatID,
                  bool saveAutomatically)
    {
      this.name = name;
      this.directory = directory;
      this.fileName = fileName;
      this.fileFormatID = fileFormatID;
      this.saveAutomatically = saveAutomatically;
    }
    
    public string Name
    {
      get { return name; }
    }

    public string Directory
    {
      get { return directory; }
    }

    public string Information
    {
      get { return string.Empty; }
    }

    public string FileName
    {
      get { return fileName; }
    }

    public Guid FileFormatID
    {
      get { return fileFormatID; }
    }

    public bool SaveAutomatically
    {
      get { return saveAutomatically; }
    }

  }
}
