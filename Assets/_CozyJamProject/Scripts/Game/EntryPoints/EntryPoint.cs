using R3;
using UnityEngine;

namespace CozySpringJam.Game.EntryPoints
{
    public abstract class EntryPoint : MonoBehaviour
    {
        public abstract Observable<string> Run(/*Some Params*/);
    }
}
