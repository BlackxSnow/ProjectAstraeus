using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipmentUI : Window
{
    public TextMeshProUGUI OwnerName;

    [HideInInspector]
    public Equipment RefEquipment;

    public void Init(Entity TargetEntity)
    {
        if (!TargetEntity.EntityFlags.HasFlag(Entity.EntityFlagsEnum.CanEquip))
        {
            throw new ArgumentException(string.Format("Target Entity '{0}' ({1}) does not have flag CanEquip", TargetEntity.Name, TargetEntity.name));
        }
        OwnerName.text = TargetEntity.Name;
        RefEquipment = TargetEntity.EntityComponents.Equipment;
    }
}
