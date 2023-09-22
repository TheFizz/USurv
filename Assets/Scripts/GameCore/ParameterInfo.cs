using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ParameterInfo
{
    public string Name;
    public Sprite Image;
    public string ShortName;
    public ParameterInfo(string name, string shortName)
    {
        Name = name;
        ShortName = shortName;
        var path = Globals.PARAMICONLOC + "/" + shortName;
        Image = Resources.Load<Sprite>(path);
    }
}
