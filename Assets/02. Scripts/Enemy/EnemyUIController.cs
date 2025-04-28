using UnityEngine;
using UnityEngine.UI;

public class EnemyUIController : MonoBehaviour
{
    [SerializeField] private Enemy _owner;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Slider _hpBar;

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
        if(_canvas == null)
        {
            foreach (Transform child in transform)
            {
                _canvas = child.GetComponentInChildren<Canvas>();
                if (_canvas != null)
                {
                    break;
                }
            }
        }
        _canvas.worldCamera = _camera;

        if(_hpBar == null)
        {
            foreach (Transform child in transform)
            {
                _hpBar = child.GetComponentInChildren<Slider>();
                if (_hpBar != null)
                {
                    break;
                }
            }
        }
        _owner.OnHpChanged += SetHpSlider;
    }

    private void Update()
    {
        _hpBar.transform.LookAt(transform.position + _camera.transform.forward);
    }

    private void SetHpSlider(float hp, float maxHp)
    {
        _hpBar.value = hp / maxHp;
    }
}
