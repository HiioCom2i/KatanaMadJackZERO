using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject katana;
    public GameObject gun;

    private GameObject activeWeapon;

    void Start()
    {
        // Começa com a katana equipada
        EquipWeapon(katana);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(katana);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeapon(gun);
        }
    }

    void EquipWeapon(GameObject weaponToEquip)
    {
        if (activeWeapon == weaponToEquip) return;

        // Desativa todas as armas
        katana.SetActive(false);
        gun.SetActive(false);

        // Ativa a arma escolhida
        weaponToEquip.SetActive(true);
        activeWeapon = weaponToEquip;
    }
}
