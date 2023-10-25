using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum PlatingType
{
    None,
    Jade,
    Amber,
    Amethyst
}
[Serializable]
public class Plating
{
    public Gradient ColorScheme;
    public PlatingType PlatingType;
}
