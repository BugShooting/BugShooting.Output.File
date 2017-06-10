namespace BS.Output.File
{

  public class Output: IOutput 
  {
    
    string name;
    string directory;
    string fileName;
    string fileFormat;
    bool saveAutomatically;

    public Output(string name,
                  string directory,
                  string fileName,
                  string fileFormat,
                  bool saveAutomatically)
    {
      this.name = name;
      this.directory = directory;
      this.fileName = fileName;
      this.fileFormat = fileFormat;
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

    public string FileFormat
    {
      get { return fileFormat; }
    }

    public bool SaveAutomatically
    {
      get { return saveAutomatically; }
    }

  }
}
