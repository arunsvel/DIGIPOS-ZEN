using System;
using System.IO;
using Microsoft.VisualBasic;

public class ClsFileOperation
{
    public string FileOperation(string filename, bool blnIsRead, string WriteString = "", bool AppendFile = false)
    {
        string ReadString = "";

        try
        {
            if (blnIsRead == true)
            {
                if (File.Exists(filename))
                {
                    StreamReader r = new StreamReader(filename);
                    ReadString = r.ReadToEnd();
                    r.Close();
                    r.Dispose();
                }
                else
                    Interaction.MsgBox("File Not Exist");
            }
            else if (AppendFile == true)
            {
                StreamWriter w = new StreamWriter(filename, true);
                w.Write(WriteString);
                w.Close();
                w.Dispose();
            }
            else
            {
                StreamWriter w = new StreamWriter(filename, false);
                w.Write(WriteString);
                w.Close();
                w.Dispose();
            }
        }
        catch (Exception ex)
        {
            Interaction.MsgBox(ex.Message);
        }
        return ReadString;
    }
}
