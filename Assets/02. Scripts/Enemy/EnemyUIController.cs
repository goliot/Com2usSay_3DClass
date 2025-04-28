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
        _canvas.worldCamera = _camera;
        _owner.OnHpChanged += SetHpSlider;
    }

    private void Update()
    {
        transform.LookAt(transform.position + _camera.transform.forward);
    }

    private void SetHpSlider(float hp, float maxHp)
    {
        _hpBar.value = hp / maxHp;
    }
}
