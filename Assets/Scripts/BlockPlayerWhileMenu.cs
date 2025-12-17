using UnityEngine;

public class BlockPlayerWhileMenu : MonoBehaviour
{
    MonoBehaviour[] scripts;

    void Awake()
    {
        scripts = GetComponents<MonoBehaviour>();
    }

    void Update()
    {
        if (OpenWorldState.menuAberto)
        {
            // Menu aberto → bloqueia player
            foreach (var s in scripts)
            {
                if (s != this && s.enabled)
                    s.enabled = false;
            }
        }
        else
        {
            // Menu fechado → libera player
            foreach (var s in scripts)
            {
                if (s != this && !s.enabled)
                    s.enabled = true;
            }
        }
    }
}
