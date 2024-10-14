using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class TbTool
{
    public static string Read(string filename)
    {
        TextAsset ts = Resources.Load<TextAsset>("GameTb/"+ filename);
        return (ts == null) ? null : ts.text;
    }

    public static string ReadLevel(string filename)
    {
        TextAsset ts = Resources.Load<TextAsset>("Levels/" + filename);
        return (ts == null) ? null : ts.text;
    }
}
