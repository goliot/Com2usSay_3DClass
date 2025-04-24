using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public EWeaponType WeaponType;
    public DamageInfo Damage;
    public float CoolTime;
    public int MaxAmmo;
    public int ReloadInterval;
    public float ExplodeRange;

    void Init()
    {
        Damage.From = GameObject.FindGameObjectWithTag("Player");
    }
}
