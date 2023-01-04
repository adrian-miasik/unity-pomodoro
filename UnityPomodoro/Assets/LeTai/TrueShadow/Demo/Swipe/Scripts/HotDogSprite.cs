using UnityEngine;

namespace LeTai.TrueShadow.Demo
{
public struct HotDogSprite
{
    public Sprite sprite;
    public bool   isHotDog;

    public override string ToString()
    {
        var prefix = isHotDog ? "(Hot Dog) " : "";
        return prefix + sprite;
    }
}
}
