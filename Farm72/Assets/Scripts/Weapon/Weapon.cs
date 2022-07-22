using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "Create Weapon", order = 0)]
public class Weapon : ScriptableObject
{
    [field:SerializeField] public WeaponBase WeaponBase { get; private set; }
    [field:SerializeField] public Transform Position { get; private set; }

}

