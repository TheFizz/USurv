using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "PlatingPresets")]
public class PlatingPresetsSO : ScriptableObject
{
    public List<Plating> Platings;
}
