using System;
using MonoGameLibrary.Graphics;

namespace BeatThemUp.GameObjects;

public class Weapon
{
    private Sprite _sprite;

    public static float Damage;

    public static float FireRate;
    
    private float _damageMultiplier;
    
    private float _fireRateMultiplier;
    
    public float DamageMultiplier
    {
        get { return _damageMultiplier; }
        set
        {
            if (value < 0)
                throw new ArgumentException("Damage Multiplier cannot be negative.");
            _damageMultiplier = value;
        }
    }
    
    public float FireRateMultiplier
    {
        get => _fireRateMultiplier;
        set
        {
            if (value < 0)
                throw new ArgumentException("Fire Rate Multiplier cannot be negative.");
            _fireRateMultiplier = value;
        }
    }

    // Get final weapon damage (base damage * multiplier)
    public float GetDamage()
    {
        return Damage * DamageMultiplier;
    }

    // Get final weapon fire rate (base fire rate * multiplier)
    public float GetFireRate()
    {
        return FireRate * FireRateMultiplier;
    }
}