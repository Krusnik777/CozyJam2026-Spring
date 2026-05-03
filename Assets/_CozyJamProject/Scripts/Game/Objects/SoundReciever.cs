using CozySpringJam.Game.Services;
using R3;
using UnityEngine;

namespace CozySpringJam.Game.Objects
{
    public abstract class SoundReceiver : MonoBehaviour
    {
        public abstract void InitSoundService(SoundService soundService);
    }
}
