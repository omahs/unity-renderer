using DCL;
using UnityEngine;

public class DisablePassportModifier : IAvatarModifier
{

    public void ApplyModifier(GameObject avatar)
    {
        if (!avatar.TryGetComponent(out IHidePassportAreaHandler handler))
            return;
        handler.DisableHidePassportModifier();
    }

    public void RemoveModifier(GameObject avatar)
    {
        if (!avatar.TryGetComponent(out IHidePassportAreaHandler handler))
            return;
        handler.EnableHidePassportModifier();
    }
    
    public string GetWarningDescription()
    {
        return "\u2022  Passports can not be opened";
    }
}