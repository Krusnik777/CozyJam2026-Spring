using CozySpringJam.Game.Services;
using UnityEngine;

namespace CozySpringJam.Game.Player
{
    public class EventCollector : MonoBehaviour
    {
        public void OnStep()
        {
            SoundService.Instance.PlayFootstep();
        }
    }
}
