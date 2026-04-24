using UnityEngine;

public interface IWeaponUser
{
    Transform AimOrigin { get; }
    Vector3 AimDirection { get; }
    Actor Owner { get; }
}