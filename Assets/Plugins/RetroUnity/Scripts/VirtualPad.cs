using UnityEngine;

public class VirtualPad : MonoBehaviour
{
    public GameObject _virtualPad;

    private void Awake()
    {
        _virtualPad = gameObject;
    }

    public void toogle()
    {
        bool on = _virtualPad.active;
        on = !on;
        _virtualPad.SetActive(on);
    }
}
